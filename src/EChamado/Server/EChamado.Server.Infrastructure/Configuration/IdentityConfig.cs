using EChamado.Server.Domain.Domains.Identities;
using EChamado.Server.Infrastructure.OpenIddict;
using EChamado.Server.Infrastructure.Persistence;
using EChamado.Shared.Shared.Settings;
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
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

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
            // Usamos OpenIddictValidation como esquema de autenticação padrão,
            // e "External" para redirecionar para aplicação de login Blazor Server.
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = "External";
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

                        // Redireciona para a aplicação Blazor Server de Identity (localhost:7132)
                        var loginUrl = "https://localhost:7132/Account/Login";
                        var finalUrl = $"{loginUrl}?returnUrl={encodedReturnUrl}";

                        logger.LogInformation("OnRedirectToLogin: Final URL={FinalUrl}", finalUrl);

                        context.Response.Redirect(finalUrl);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Error in OnRedirectToLogin. RedirectUri={RedirectUri}", context.RedirectUri);
                        // Fallback: redireciona para login sem returnUrl
                        context.Response.Redirect("https://localhost:7132/Account/Login");
                    }

                    return Task.CompletedTask;
                };
            });

            // -------------------------
            // 6) CONFIGURAÇÃO OPENIDDICT
            // -------------------------
            services.AddOpenIddict()
                // -------- CORE --------
                .AddCore(options =>
                {
                    options.UseEntityFrameworkCore()
                           .UseDbContext<ApplicationDbContext>();
                })

                // -------- SERVER --------
                .AddServer(options =>
                {
                    // Endpoints de autorização e token
                    options.SetAuthorizationEndpointUris("/connect/authorize")
                           .SetTokenEndpointUris("/connect/token");

                    // Issuer definido em AppSettings.ValidOn
                    options.SetIssuer(new Uri(appSettings.ValidOn));

                    // Permitir fluxos
                    options.AllowAuthorizationCodeFlow()
                           .AllowRefreshTokenFlow()
                           .AllowClientCredentialsFlow()
                           .AllowPasswordFlow();

                    // Exigir PKCE no Authorization Code Flow
                    options.RequireProofKeyForCodeExchange();

                    // Chave de assinatura simétrica
                    options.AddSigningKey(new SymmetricSecurityKey(key));

                    // Registra escopos adicionais
                    options.RegisterScopes("openid", "profile", "email", "address", "phone", "roles", "api", "chamados");

                    // Certificados de desenvolvimento (opcional)
                    options.AddDevelopmentEncryptionCertificate()
                           .AddDevelopmentSigningCertificate();

                    // Integra com ASP.NET Core
                    options.UseAspNetCore()
                           .EnableAuthorizationEndpointPassthrough()
                           .EnableTokenEndpointPassthrough();
                })

                // -------- VALIDAÇÃO --------
                .AddValidation(options =>
                {
                    options.UseLocalServer();
                    options.UseSystemNetHttp();
                    options.UseAspNetCore();
                });

            // -------------------------
            // 7) SERVIÇO QUE CONFIGURA OS CLIENTES
            // -------------------------
            services.AddHostedService<OpenIddictWorker>();

            return services;
        }
    }
}
