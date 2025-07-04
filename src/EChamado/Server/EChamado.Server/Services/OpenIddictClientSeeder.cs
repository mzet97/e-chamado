using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace EChamado.Server.Services;

public class OpenIddictClientSeeder
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;

    public OpenIddictClientSeeder(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
    }

    public async Task SeedAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

        // Configurar cliente Blazor WebAssembly
        var blazorClientId = "bwa-client";
        if (await manager.FindByClientIdAsync(blazorClientId) is null)
        {
            await manager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = blazorClientId,
                ConsentType = ConsentTypes.Explicit,
                DisplayName = "EChamado Blazor WebAssembly Client",
                ClientType = ClientTypes.Public,
                PostLogoutRedirectUris =
                {
                    new Uri("https://localhost:7274/authentication/logout-callback")
                },
                RedirectUris =
                {
                    new Uri("https://localhost:7274/authentication/login-callback")
                },
                Permissions =
                {
                    Permissions.Endpoints.Authorization,
                    Permissions.Endpoints.Token,
                    Permissions.Endpoints.Revocation,
                    Permissions.GrantTypes.AuthorizationCode,
                    Permissions.GrantTypes.RefreshToken,
                    Permissions.ResponseTypes.Code,
                    Permissions.Scopes.Email,
                    Permissions.Scopes.Profile,
                    Permissions.Scopes.Roles,
                    Permissions.Prefixes.Scope + "api",
                    Permissions.Prefixes.Scope + "chamados"
                },
                Requirements =
                {
                    Requirements.Features.ProofKeyForCodeExchange
                }
            });
        }

        // Configurar scopes
        var scopeManager = scope.ServiceProvider.GetRequiredService<IOpenIddictScopeManager>();

        if (await scopeManager.FindByNameAsync("api") is null)
        {
            await scopeManager.CreateAsync(new OpenIddictScopeDescriptor
            {
                Name = "api",
                DisplayName = "EChamado API",
                Resources =
                {
                    "EChamado.Server"
                }
            });
        }

        if (await scopeManager.FindByNameAsync("chamados") is null)
        {
            await scopeManager.CreateAsync(new OpenIddictScopeDescriptor
            {
                Name = "chamados",
                DisplayName = "EChamado Chamados",
                Resources =
                {
                    "EChamado.Server"
                }
            });
        }
    }
}
