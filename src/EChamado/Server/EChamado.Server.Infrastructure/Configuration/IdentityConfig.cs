using EChamado.Server.Domain.Domains.Identities;
using EChamado.Server.Infrastructure.OpenIddict;
using EChamado.Server.Infrastructure.Persistence;
using EChamado.Shared.Shared.Settings;
using Microsoft.AspNetCore.Authentication.Cookies;
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

                options.SignIn.RequireConfirmedAccount = true;

                options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%&*()_=?. ";
                options.User.RequireUniqueEmail = true;
            })
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
            // 4) CONFIGURAÇÃO DO AUTH
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
                options.Events.OnRedirectToLogin = context =>
                {
                    // Redireciona para a aplicação Blazor Server de Identity (localhost:7132)
                    var loginUrl = "https://localhost:7132/Account/Login";
                    var returnUrl = Uri.EscapeDataString(context.RedirectUri);
                    context.Response.Redirect($"{loginUrl}?returnUrl={returnUrl}");
                    return Task.CompletedTask;
                };
            });

            // -------------------------
            // 5) CONFIGURAÇÃO OPENIDDICT
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
                    options.UseAspNetCore();
                });

            // -------------------------
            // 6) SERVIÇO QUE CONFIGURA OS CLIENTES
            // -------------------------
            services.AddHostedService<OpenIddictWorker>();

            return services;
        }
    }
}
