using EChamado.Server.Infrastructure.Persistence;
using EChamado.Shared.Shared.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace EChamado.Server.Infrastructure.OpenIddict
{
    public class OpenIddictWorker : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IOptions<ClientSettings> _clientSettings;

        public OpenIddictWorker(IServiceProvider serviceProvider, IOptions<ClientSettings> clientSettings)
        {
            _serviceProvider = serviceProvider;
            _clientSettings = clientSettings;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await context.Database.EnsureCreatedAsync(cancellationToken);

            var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

            // 1) Garante que o cliente Blazor (bwa-client) esteja configurado para Authorization Code + PKCE
            await CreateOrUpdateBlazorClientAsync(manager, cancellationToken);

            // 2) Garante que o cliente mobile (mobile-client) esteja configurado para Password Flow (opcional)
            await CreateOrUpdateMobileClientAsync(manager, cancellationToken);
        }

        private async Task CreateOrUpdateBlazorClientAsync(IOpenIddictApplicationManager manager, CancellationToken cancellationToken)
        {
            // A) Tenta encontrar o cliente "bwa-client"
            var blazorClient = await manager.FindByClientIdAsync("bwa-client", cancellationToken);
            if (blazorClient is null)
            {
                var descriptor = new OpenIddictApplicationDescriptor
                {
                    ClientId = "bwa-client",
                    DisplayName = "Cliente Web Blazor",
                    ClientType = ClientTypes.Public,
                    ConsentType = ConsentTypes.Explicit
                };

                descriptor.Permissions.UnionWith(new[]
                {
                    Permissions.Endpoints.Authorization,
                    Permissions.Endpoints.Token,
                    Permissions.GrantTypes.AuthorizationCode,
                    Permissions.GrantTypes.RefreshToken,
                    Permissions.ResponseTypes.Code,
                    Scopes.OpenId,
                    Permissions.Scopes.Profile,
                    Permissions.Scopes.Email,
                    Permissions.Scopes.Address,
                    Permissions.Scopes.Phone,
                    Permissions.Scopes.Roles,
                    Permissions.Prefixes.Scope + "api",
                    Permissions.Prefixes.Scope + "chamados"
                });

                descriptor.Requirements.Add(Requirements.Features.ProofKeyForCodeExchange);
                descriptor.RedirectUris.Add(new Uri("https://localhost:7274/authentication/login-callback"));
                descriptor.PostLogoutRedirectUris.Add(new Uri("https://localhost:7274/"));

                await manager.CreateAsync(descriptor, cancellationToken);
            }
            else
            {
                var descriptor = new OpenIddictApplicationDescriptor();
                await manager.PopulateAsync(descriptor, blazorClient, cancellationToken);

                descriptor.ClientType = ClientTypes.Public;
                descriptor.ConsentType = ConsentTypes.Explicit;

                descriptor.Permissions.Clear();
                descriptor.Permissions.UnionWith(new[]
                {
                    Permissions.Endpoints.Authorization,
                    Permissions.Endpoints.Token,
                    Permissions.GrantTypes.AuthorizationCode,
                    Permissions.GrantTypes.RefreshToken,
                    Permissions.ResponseTypes.Code,
                    Scopes.OpenId,
                    Permissions.Scopes.Profile,
                    Permissions.Scopes.Email,
                    Permissions.Scopes.Address,
                    Permissions.Scopes.Phone,
                    Permissions.Scopes.Roles,
                    Permissions.Prefixes.Scope + "api",
                    Permissions.Prefixes.Scope + "chamados"
                });

                descriptor.Requirements.Clear();
                descriptor.Requirements.Add(Requirements.Features.ProofKeyForCodeExchange);

                descriptor.RedirectUris.Clear();
                descriptor.PostLogoutRedirectUris.Clear();
                descriptor.RedirectUris.Add(new Uri("https://localhost:7274/authentication/login-callback"));
                descriptor.PostLogoutRedirectUris.Add(new Uri("https://localhost:7274/"));

                await manager.UpdateAsync(blazorClient, descriptor, cancellationToken);
            }
        }

        private async Task CreateOrUpdateMobileClientAsync(IOpenIddictApplicationManager manager, CancellationToken cancellationToken)
        {
            // Exemplo para um cliente mobile com fluxo Password
            var mobileClient = await manager.FindByClientIdAsync("mobile-client", cancellationToken);
            if (mobileClient is null)
            {
                var descriptor = new OpenIddictApplicationDescriptor
                {
                    ClientId = "mobile-client",
                    DisplayName = "Mobile Client",
                    ClientType = ClientTypes.Public
                };

                descriptor.Permissions.UnionWith(new[]
                {
                    Permissions.Endpoints.Token,
                    Permissions.GrantTypes.Password,
                    Permissions.GrantTypes.RefreshToken,
                    Scopes.OpenId,
                    Permissions.Scopes.Profile,
                    Permissions.Scopes.Email,
                    Permissions.Scopes.Address,
                    Permissions.Scopes.Phone,
                    Permissions.Scopes.Roles,
                    Permissions.Prefixes.Scope + "api",
                    Permissions.Prefixes.Scope + "chamados"
                });

                await manager.CreateAsync(descriptor, cancellationToken);
            }
            else
            {
                var descriptor = new OpenIddictApplicationDescriptor();
                await manager.PopulateAsync(descriptor, mobileClient, cancellationToken);

                descriptor.ClientType = ClientTypes.Public;
                descriptor.RedirectUris.Clear();
                descriptor.PostLogoutRedirectUris.Clear();

                descriptor.Permissions.Clear();
                descriptor.Permissions.UnionWith(new[]
                {
                    Permissions.Endpoints.Token,
                    Permissions.GrantTypes.Password,
                    Permissions.GrantTypes.RefreshToken,
                    Scopes.OpenId,
                    Permissions.Scopes.Profile,
                    Permissions.Scopes.Email,
                    Permissions.Scopes.Address,
                    Permissions.Scopes.Phone,
                    Permissions.Scopes.Roles,
                    Permissions.Prefixes.Scope + "api",
                    Permissions.Prefixes.Scope + "chamados"
                });

                await manager.UpdateAsync(mobileClient, descriptor, cancellationToken);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
