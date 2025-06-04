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
            // Tenta encontrar o cliente "bwa-client"
            var blazorClient = await manager.FindByClientIdAsync("bwa-client", cancellationToken);
            if (blazorClient is null)
            {
                // Se não existir, cria com as permissões adequadas
                var descriptor = new OpenIddictApplicationDescriptor
                {
                    ClientId = "bwa-client",
                    DisplayName = "Cliente Web Blazor",
                    ClientType = OpenIddictConstants.ClientTypes.Public,
                    ConsentType = OpenIddictConstants.ConsentTypes.Explicit,
                    Permissions =
                    {
                        // Endpoints
                        OpenIddictConstants.Permissions.Endpoints.Authorization,
                        OpenIddictConstants.Permissions.Endpoints.Token,

                        // Fluxos permitidos
                        OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                        OpenIddictConstants.Permissions.GrantTypes.RefreshToken,

                        // Response types
                        OpenIddictConstants.Permissions.ResponseTypes.Code,

                        // Escopos principais
                        OpenIddictConstants.Scopes.OpenId,
                        OpenIddictConstants.Permissions.Scopes.Profile,
                        OpenIddictConstants.Permissions.Scopes.Email,
                        OpenIddictConstants.Permissions.Scopes.Address,
                        OpenIddictConstants.Permissions.Scopes.Phone,
                        OpenIddictConstants.Permissions.Scopes.Roles,

                        // Escopos personalizados
                        OpenIddictConstants.Permissions.Prefixes.Scope + "api",
                        OpenIddictConstants.Permissions.Prefixes.Scope + "chamados"
                    },
                    Requirements =
                    {
                        Requirements.Features.ProofKeyForCodeExchange
                    }
                };

                // URIs específicos para Blazor WASM
                descriptor.RedirectUris.Add(new Uri("https://localhost:7274/authentication/login-callback"));
                descriptor.PostLogoutRedirectUris.Add(new Uri("https://localhost:7274/"));

                await manager.CreateAsync(descriptor, cancellationToken);
            }
            else
            {
                // Atualiza cliente existente
                var descriptor = new OpenIddictApplicationDescriptor();
                await manager.PopulateAsync(descriptor, blazorClient, cancellationToken);

                descriptor.ClientType = OpenIddictConstants.ClientTypes.Public;
                descriptor.ConsentType = OpenIddictConstants.ConsentTypes.Explicit;

                descriptor.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Authorization);
                descriptor.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Token);
                descriptor.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode);
                descriptor.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.RefreshToken);
                descriptor.Permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.Code);
                descriptor.Permissions.Add(OpenIddictConstants.Scopes.OpenId);
                descriptor.Permissions.Add(OpenIddictConstants.Permissions.Scopes.Profile);
                descriptor.Permissions.Add(OpenIddictConstants.Permissions.Scopes.Email);
                descriptor.Permissions.Add(OpenIddictConstants.Permissions.Scopes.Address);
                descriptor.Permissions.Add(OpenIddictConstants.Permissions.Scopes.Phone);
                descriptor.Permissions.Add(OpenIddictConstants.Permissions.Scopes.Roles);

                descriptor.Permissions.Add(OpenIddictConstants.Permissions.Prefixes.Scope + "api");
                descriptor.Permissions.Add(OpenIddictConstants.Permissions.Prefixes.Scope + "chamados");

                descriptor.Requirements.Add(OpenIddictConstants.Requirements.Features.ProofKeyForCodeExchange);

                // Corrige as URIs (limpa primeiro)
                descriptor.RedirectUris.Clear();
                descriptor.PostLogoutRedirectUris.Clear();
                descriptor.RedirectUris.Add(new Uri("https://localhost:7274/authentication/login-callback"));
                descriptor.PostLogoutRedirectUris.Add(new Uri("https://localhost:7274/"));

                await manager.UpdateAsync(blazorClient, descriptor, cancellationToken);
            }
        }

        private async Task CreateOrUpdateMobileClientAsync(IOpenIddictApplicationManager manager, CancellationToken cancellationToken)
        {
            // Cliente para Mobile com fluxos Password + Refresh Token
            var mobileClient = await manager.FindByClientIdAsync("mobile-client", cancellationToken);
            if (mobileClient is null)
            {
                var descriptor = new OpenIddictApplicationDescriptor
                {
                    ClientId = "mobile-client",
                    DisplayName = "Mobile Client",
                    ClientType = OpenIddictConstants.ClientTypes.Public,
                    Permissions =
                    {
                        OpenIddictConstants.Permissions.Endpoints.Token,
                        OpenIddictConstants.Permissions.GrantTypes.Password,
                        OpenIddictConstants.Permissions.GrantTypes.RefreshToken,

                        OpenIddictConstants.Scopes.OpenId,
                        OpenIddictConstants.Permissions.Scopes.Profile,
                        OpenIddictConstants.Permissions.Scopes.Email,
                        OpenIddictConstants.Permissions.Scopes.Address,
                        OpenIddictConstants.Permissions.Scopes.Phone,
                        OpenIddictConstants.Permissions.Scopes.Roles,

                        OpenIddictConstants.Permissions.Prefixes.Scope + "api",
                        OpenIddictConstants.Permissions.Prefixes.Scope + "chamados"
                    }
                };

                await manager.CreateAsync(descriptor, cancellationToken);
            }
            else
            {
                // Atualiza configurações do mobile-client
                var descriptor = new OpenIddictApplicationDescriptor();
                await manager.PopulateAsync(descriptor, mobileClient, cancellationToken);

                descriptor.ClientType = OpenIddictConstants.ClientTypes.Public;
                descriptor.RedirectUris.Clear();
                descriptor.PostLogoutRedirectUris.Clear();

                descriptor.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Token);
                descriptor.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.Password);
                descriptor.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.RefreshToken);
                descriptor.Permissions.Add(OpenIddictConstants.Scopes.OpenId);
                descriptor.Permissions.Add(OpenIddictConstants.Permissions.Scopes.Profile);
                descriptor.Permissions.Add(OpenIddictConstants.Permissions.Scopes.Email);
                descriptor.Permissions.Add(OpenIddictConstants.Permissions.Scopes.Address);
                descriptor.Permissions.Add(OpenIddictConstants.Permissions.Scopes.Phone);
                descriptor.Permissions.Add(OpenIddictConstants.Permissions.Scopes.Roles);

                descriptor.Permissions.Add(OpenIddictConstants.Permissions.Prefixes.Scope + "api");
                descriptor.Permissions.Add(OpenIddictConstants.Permissions.Prefixes.Scope + "chamados");

                await manager.UpdateAsync(mobileClient, descriptor, cancellationToken);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
