using EChamado.Server.Domain.Domains.Identities;
using EChamado.Server.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Validation.AspNetCore;

namespace EChamado.Server.Infrastructure.Configuration
{
    public static class IdentityConfig
    {
        public static IServiceCollection AddIdentityConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
                options.UseOpenIddict();
            });

            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Configura a autenticação para validar tokens JWT emitidos pelo servidor de autorização
            services.AddAuthentication(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);

            services.AddOpenIddict()
                .AddValidation(options =>
                {
                    // Aponta para o servidor de autorização para obter as chaves de validação
                    options.SetIssuer("https://localhost:7132/"); // URL do Echamado.Auth
                    options.UseAspNetCore();
                });

            return services;
        }
    }
}
