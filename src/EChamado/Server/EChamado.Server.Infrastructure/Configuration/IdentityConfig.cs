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

namespace EChamado.Server.Infrastructure.Configuration;

public static class IdentityConfig
{
    public static IServiceCollection AddIdentityConfig(this IServiceCollection services, IConfiguration configuration)
    {
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

        var key = Encoding.ASCII.GetBytes(appSettings.Secret);        services.AddAuthentication(options =>
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

        services.AddOpenIddict()
            .AddCore(options =>
            {
                options.UseEntityFrameworkCore()
                       .UseDbContext<ApplicationDbContext>();
            })
            .AddServer(options =>
            {
                // Define os endpoints de autorização e token
                options.SetAuthorizationEndpointUris("/connect/authorize")
                       .SetTokenEndpointUris("/connect/token");

                // Define o issuer a partir do AppSettings
                options.SetIssuer(new Uri(appSettings.ValidOn));                // Permite fluxos: Authorization Code, Refresh Token, Client Credentials e Password
                options.AllowAuthorizationCodeFlow()
                       .AllowRefreshTokenFlow()
                       .AllowClientCredentialsFlow()
                       .AllowPasswordFlow();

                // Exige PKCE no Authorization Code Flow
                options.RequireProofKeyForCodeExchange();

                // Usa a mesma chave simétrica definida em AppSettings.Secret
                options.AddSigningKey(new SymmetricSecurityKey(key));

                // Registra escopos que poderão ser usados pelos clientes
                options.RegisterScopes("openid", "profile", "email", "address", "phone", "roles", "api", "chamados");

                // Certificados de desenvolvimento (opcional)
                options.AddDevelopmentEncryptionCertificate()
                       .AddDevelopmentSigningCertificate();

                // Integra com o ASP.NET Core
                options.UseAspNetCore()
                       .EnableAuthorizationEndpointPassthrough()
                       .EnableTokenEndpointPassthrough();
            })
            .AddValidation(options =>
            {
                // Valida localmente os tokens emitidos
                options.UseLocalServer();
                options.UseAspNetCore();
            });

        services.AddHostedService<OpenIddictWorker>();

        return services;
    }
}
