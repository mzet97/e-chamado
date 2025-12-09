using EChamado.Server.Domain.Domains.Identities;
using EChamado.Server.Infrastructure.OpenIddict;
using EChamado.Server.Infrastructure.Persistence;
using EChamado.Shared.Domain.Settings;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Validation.AspNetCore;
using System.Text;

namespace EChamado.Server.Infrastructure.Configuration
{
    public static class IdentityConfig
    {
        public static IServiceCollection AddIdentityConfig(this IServiceCollection services, IConfiguration configuration)
        {
            // -------------------------
            // 1) CONFIGURAÇÃO DB
            // -------------------------
            services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
            {
                var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
                options.UseLoggerFactory(loggerFactory);
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));

                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                {
                    options.EnableSensitiveDataLogging(true);
                    options.EnableDetailedErrors();
                }
            });

            // -------------------------
            // 2) CONFIGURAÇÃO IDENTITY
            // -------------------------
            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 12;
                options.Password.RequiredUniqueChars = 4;

                options.SignIn.RequireConfirmedAccount = false; // Alterado para false para facilitar testes

                options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%&*()_=?. ";
                options.User.RequireUniqueEmail = true;
            })
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            // -------------------------
            // 3) CONFIGURAÇÃO APPSETTINGS & CLIENTSETTINGS
            // -------------------------
            var appSettingsSection = configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            var appSettings = appSettingsSection.Get<AppSettings>();
            if (appSettings == null)
            {
                throw new ApplicationException("AppSettings not found");
            }

            var clientSettingsSection = configuration.GetSection("ClientSettings");
            services.Configure<ClientSettings>(clientSettingsSection);

            var clientSettings = clientSettingsSection.Get<ClientSettings>();
            if (clientSettings == null)
            {
                throw new ApplicationException("ClientSettings not found");
            }

            // Cria a chave simétrica a partir do segredo em AppSettings
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);

            // -------------------------
            // 4) CONFIGURAÇÃO DATA PROTECTION (para compartilhar cookies entre apps)
            // -------------------------
            var keysPath = Environment.GetEnvironmentVariable("DP_KEYS_PATH")
                             ?? Path.Combine(Path.GetTempPath(), "EChamado-DataProtection-Keys");
            Directory.CreateDirectory(keysPath);

            services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(keysPath))
                .SetApplicationName("EChamado");

            // -------------------------
            // 5) CONFIGURAÇÃO DO AUTH
            // -------------------------
            // Usamos OpenIddictValidation como esquema de autenticação padrão
            // Para APIs REST, retorna 401 Unauthorized
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
            })
            .AddCookie("External", options =>
            {
                options.Cookie.Name = "EChamado.External";
                options.Cookie.SameSite = SameSiteMode.None; // Permitir compartilhamento entre diferentes portas
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.LoginPath = "/Account/Login";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                options.SlidingExpiration = true;

                options.Events.OnRedirectToLogin = context =>
                {
                    var loggerFactory = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>();
                    var logger = loggerFactory.CreateLogger("EChamado.Server.Infrastructure.IdentityConfig");

                    try
                    {
                        // context.RedirectUri já contém o path completo com query string
                        // Ex: /Account/Login?ReturnUrl=/connect/authorize?params...
                        logger.LogInformation("OnRedirectToLogin: Original RedirectUri={RedirectUri}", context.RedirectUri);

                        // Extrai o ReturnUrl dos query params
                        var queryString = context.RedirectUri.Contains('?')
                            ? context.RedirectUri.Substring(context.RedirectUri.IndexOf('?'))
                            : "";

                        var returnUrl = "/connect/authorize";
                        if (!string.IsNullOrEmpty(queryString))
                        {
                            var query = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(queryString);
                            if (query.TryGetValue("ReturnUrl", out var value))
                            {
                                returnUrl = value.ToString();
                            }
                        }

                        logger.LogInformation("OnRedirectToLogin: Extracted ReturnUrl={ReturnUrl}", returnUrl);

                        // Constrói URL completa para o servidor OpenIddict (7296)
                        var fullReturnUrl = $"https://localhost:7296{returnUrl}";
                        var encodedReturnUrl = Uri.EscapeDataString(fullReturnUrl);

                        // Redireciona para a aplicação Blazor Server de Identity (localhost:7133)
                        var loginUrl = "https://localhost:7133/Account/Login";
                        var finalUrl = $"{loginUrl}?returnUrl={encodedReturnUrl}";

                        logger.LogInformation("OnRedirectToLogin: Final URL={FinalUrl}", finalUrl);

                        context.Response.Redirect(finalUrl);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Error in OnRedirectToLogin. RedirectUri={RedirectUri}", context.RedirectUri);
                        // Fallback: redireciona para login sem returnUrl
                        context.Response.Redirect("https://localhost:7133/Account/Login");
                    }

                    return Task.CompletedTask;
                };
            });

            // -------------------------
            // 6) CONFIGURAÇÃO OPENIDDICT (SÓ VALIDAÇÃO - API SERVER)
            // -------------------------
            services.AddOpenIddict()

                // -------- CORE --------
                .AddCore(options =>
                {
                    options.UseEntityFrameworkCore()
                           .UseDbContext<ApplicationDbContext>();
                })

                // -------- VALIDAÇÃO (Resource Server - só valida tokens) --------
                .AddValidation(options =>
                {
                    // Configura para validar tokens do Auth Server (porta 7133)
                    options.SetIssuer(new Uri("https://localhost:7133"));

                    // ✅ FORÇA uso de introspecção para tokens criptografados (JWE)
                    options.UseIntrospection();

                    // Use system HTTP client for token introspection
                    options.UseSystemNetHttp();
                    options.UseAspNetCore();

                    // Configure introspection client credentials
                    options.SetClientId("introspection-client");
                    options.SetClientSecret("echamado_introspection_secret_2024");
                });

            // -------------------------
            // 7) SERVIÇO QUE CONFIGURA OS CLIENTES
            // -------------------------
            // TEMPORARIAMENTE DESABILITADO PARA EVITAR CONCORRÊNCIA
            // O Auth Server deve ser a única fonte de verdade para configuração dos clientes
            // // services.AddHostedService<OpenIddictWorker>();

            return services;
        }
    }
}
