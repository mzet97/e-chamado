using EChamado.Server.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenIddict.Abstractions;

namespace EChamado.Server.Infrastructure.OpenIddict;

public class OpenIddictWorker : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public OpenIddictWorker(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await context.Database.EnsureCreatedAsync(cancellationToken);

        var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

        // Verifica se o cliente 'bwa-client' já foi criado
        var existingClient = await manager.FindByClientIdAsync("bwa-client", cancellationToken);
        if (existingClient is null)
        {
            // Se não existir, cria o cliente com as permissões desejadas
            var descriptor = new OpenIddictApplicationDescriptor
            {
                ClientId = "bwa-client",
                DisplayName = "Cliente Web Blazor",

                // Redirecionamentos (obrigatórios se usar Authorization Code Flow com PKCE)
                //RedirectUris = { new Uri("https://localhost:5002/authentication/login-callback") },
                //PostLogoutRedirectUris = { new Uri("https://localhost:5002/") },

                // Para clientes públicos (SPA/Blazor/Mobile), não definimos ClientSecret
                // ClientSecret = "...", // Somente se fosse uma aplicação confidencial

                Permissions =
                {
                    // Endpoints que essa aplicação pode acessar
                    OpenIddictConstants.Permissions.Endpoints.Authorization,
                    OpenIddictConstants.Permissions.Endpoints.Token,
                    // Caso precise: 
                    // OpenIddictConstants.Permissions.Endpoints.Logout,
                    // OpenIddictConstants.Permissions.Endpoints.Revocation,
                    // OpenIddictConstants.Permissions.Endpoints.Introspection,

                    // Fluxos habilitados para esse cliente
                    OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                    OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
                    OpenIddictConstants.Permissions.GrantTypes.Password, // Se quiser testar ROPC no Postman

                    // Escopos permitidos
                    // OpenIddictConstants.Permissions.Scopes.OpenId, // Se quiser 'openid' scope
                    OpenIddictConstants.Permissions.Scopes.Profile,
                    OpenIddictConstants.Permissions.Scopes.Email,
                }
            };

            await manager.CreateAsync(descriptor, cancellationToken);
        }
        else
        {
            // Se já existir, recuperamos o descriptor e atualizamos
            var descriptor = new OpenIddictApplicationDescriptor();
            await manager.PopulateAsync(descriptor, existingClient, cancellationToken);

            // Garante que os endpoints necessários estejam presentes
            descriptor.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Authorization);
            descriptor.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Token);

            // Garante que os fluxos desejados estejam habilitados
            descriptor.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode);
            descriptor.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.RefreshToken);
            descriptor.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.Password);

            // Garante que os escopos estejam presentes
            descriptor.Permissions.Add(OpenIddictConstants.Permissions.Scopes.Profile);
            descriptor.Permissions.Add(OpenIddictConstants.Permissions.Scopes.Email);
            // descriptor.Permissions.Add(OpenIddictConstants.Permissions.Scopes.OpenId);

            // Atualiza o cliente no banco
            await manager.UpdateAsync(existingClient, descriptor, cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
