using EChamado.Server.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Echamado.Auth;

public class OpenIddictWorker : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OpenIddictWorker> _logger;

    public OpenIddictWorker(
        IServiceProvider serviceProvider,
        ILogger<OpenIddictWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        _logger.LogInformation("Ensuring database is created for OpenIddict...");
        await context.Database.EnsureCreatedAsync(cancellationToken);
        _logger.LogInformation("✅ Database ready for OpenIddict");

        var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();
        var scopeManager = scope.ServiceProvider.GetRequiredService<IOpenIddictScopeManager>();

        // Registra scopes personalizados
        await RegisterCustomScopesAsync(scopeManager, cancellationToken);

        // 1) Garante que o cliente Blazor (bwa-client) esteja configurado para Authorization Code + PKCE
        await CreateOrUpdateBlazorClientAsync(manager, cancellationToken);

        // 2) Garante que o cliente mobile (mobile-client) esteja configurado para Password Flow
        await CreateOrUpdateMobileClientAsync(manager, cancellationToken);

        // 3) Garante que o cliente de introspecção (introspection-client) esteja configurado
        await CreateOrUpdateIntrospectionClientAsync(manager, cancellationToken);

        _logger.LogInformation("✅ OpenIddict clients and scopes configured successfully");
    }

    private async Task CreateOrUpdateIntrospectionClientAsync(IOpenIddictApplicationManager manager, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Configuring introspection-client (for API Server token validation)...");

        var introspectionClient = await manager.FindByClientIdAsync("introspection-client", cancellationToken);
        if (introspectionClient is null)
        {
            var descriptor = new OpenIddictApplicationDescriptor
            {
                ClientId = "introspection-client",
                DisplayName = "Introspection Client (API Server)",
                ClientType = ClientTypes.Confidential,
                ClientSecret = "echamado_introspection_secret_2024",
                Permissions =
                {
                    Permissions.Endpoints.Introspection
                }
            };

            await manager.CreateAsync(descriptor, cancellationToken);
            _logger.LogInformation("✅ Client 'introspection-client' created");
        }
        else
        {
            _logger.LogInformation("Client 'introspection-client' already exists, updating...");

            var descriptor = new OpenIddictApplicationDescriptor();
            await manager.PopulateAsync(descriptor, introspectionClient, cancellationToken);

            descriptor.ClientType = ClientTypes.Confidential;
            descriptor.ClientSecret = "echamado_introspection_secret_2024";
            descriptor.Permissions.Clear();
            descriptor.Permissions.Add(Permissions.Endpoints.Introspection);

            await manager.UpdateAsync(introspectionClient, descriptor, cancellationToken);
            _logger.LogInformation("✅ Client 'introspection-client' updated");
        }
    }

    private async Task RegisterCustomScopesAsync(IOpenIddictScopeManager scopeManager, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Registering custom scopes...");

        // Scope "roles"
        if (await scopeManager.FindByNameAsync("roles", cancellationToken) is null)
        {
            await scopeManager.CreateAsync(new OpenIddictScopeDescriptor
            {
                Name = "roles",
                DisplayName = "User Roles",
                Description = "Allows access to user roles information",
                Resources = { "echamado_api" }
            }, cancellationToken);
            _logger.LogInformation("✅ Scope 'roles' registered");
        }

        // Scope "api"
        if (await scopeManager.FindByNameAsync("api", cancellationToken) is null)
        {
            await scopeManager.CreateAsync(new OpenIddictScopeDescriptor
            {
                Name = "api",
                DisplayName = "API Access",
                Description = "Allows access to the EChamado API",
                Resources = { "echamado_api" }
            }, cancellationToken);
            _logger.LogInformation("✅ Scope 'api' registered");
        }

        // Scope "chamados"
        if (await scopeManager.FindByNameAsync("chamados", cancellationToken) is null)
        {
            await scopeManager.CreateAsync(new OpenIddictScopeDescriptor
            {
                Name = "chamados",
                DisplayName = "Chamados Access",
                Description = "Allows full access to chamados (tickets)",
                Resources = { "echamado_api" }
            }, cancellationToken);
            _logger.LogInformation("✅ Scope 'chamados' registered");
        }

        _logger.LogInformation("✅ Custom scopes registration completed");
    }

    private async Task CreateOrUpdateBlazorClientAsync(IOpenIddictApplicationManager manager, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Configuring bwa-client (Blazor WebAssembly)...");

        // Tenta encontrar o cliente "bwa-client"
        var blazorClient = await manager.FindByClientIdAsync("bwa-client", cancellationToken);
        if (blazorClient is null)
        {
            // Se não existir, cria com as permissões adequadas
            var descriptor = new OpenIddictApplicationDescriptor
            {
                ClientId = "bwa-client",
                DisplayName = "Cliente Web Blazor",
                ClientType = ClientTypes.Public,
                ConsentType = ConsentTypes.Explicit,
                Permissions =
                {
                    // Endpoints
                    Permissions.Endpoints.Authorization,
                    Permissions.Endpoints.Token,

                    // Fluxos permitidos
                    Permissions.GrantTypes.AuthorizationCode,
                    Permissions.GrantTypes.RefreshToken,

                    // Response types
                    Permissions.ResponseTypes.Code,

                    // Escopos principais
                    Scopes.OpenId,
                    Permissions.Scopes.Profile,
                    Permissions.Scopes.Email,
                    Permissions.Scopes.Address,
                    Permissions.Scopes.Phone,
                    Permissions.Scopes.Roles,

                    // Escopos personalizados
                    Permissions.Prefixes.Scope + "api",
                    Permissions.Prefixes.Scope + "chamados"
                },
                Requirements =
                {
                    Requirements.Features.ProofKeyForCodeExchange
                }
            };

            // URIs específicos para Blazor WASM
            descriptor.RedirectUris.Add(new Uri("https://localhost:7274/authentication/login-callback"));
            descriptor.PostLogoutRedirectUris.Add(new Uri("https://localhost:7274/authentication/logout-callback"));

            await manager.CreateAsync(descriptor, cancellationToken);
            _logger.LogInformation("✅ Client 'bwa-client' created");
        }
        else
        {
            _logger.LogInformation("Client 'bwa-client' already exists, updating...");

            // Atualiza cliente existente
            var descriptor = new OpenIddictApplicationDescriptor();
            await manager.PopulateAsync(descriptor, blazorClient, cancellationToken);

            descriptor.ClientType = ClientTypes.Public;
            descriptor.ConsentType = ConsentTypes.Explicit;

            // Limpa e adiciona permissões
            descriptor.Permissions.Clear();
            descriptor.Permissions.Add(Permissions.Endpoints.Authorization);
            descriptor.Permissions.Add(Permissions.Endpoints.Token);
            descriptor.Permissions.Add(Permissions.GrantTypes.AuthorizationCode);
            descriptor.Permissions.Add(Permissions.GrantTypes.RefreshToken);
            descriptor.Permissions.Add(Permissions.ResponseTypes.Code);
            descriptor.Permissions.Add(Scopes.OpenId);
            descriptor.Permissions.Add(Permissions.Scopes.Profile);
            descriptor.Permissions.Add(Permissions.Scopes.Email);
            descriptor.Permissions.Add(Permissions.Scopes.Address);
            descriptor.Permissions.Add(Permissions.Scopes.Phone);
            descriptor.Permissions.Add(Permissions.Scopes.Roles);

            descriptor.Permissions.Add(Permissions.Prefixes.Scope + "api");
            descriptor.Permissions.Add(Permissions.Prefixes.Scope + "chamados");

            descriptor.Requirements.Clear();
            descriptor.Requirements.Add(Requirements.Features.ProofKeyForCodeExchange);

            // Corrige as URIs (limpa primeiro)
            descriptor.RedirectUris.Clear();
            descriptor.PostLogoutRedirectUris.Clear();
            descriptor.RedirectUris.Add(new Uri("https://localhost:7274/authentication/login-callback"));
            descriptor.PostLogoutRedirectUris.Add(new Uri("https://localhost:7274/authentication/logout-callback"));

            await manager.UpdateAsync(blazorClient, descriptor, cancellationToken);
            _logger.LogInformation("✅ Client 'bwa-client' updated");
        }
    }

    private async Task CreateOrUpdateMobileClientAsync(IOpenIddictApplicationManager manager, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Configuring mobile-client...");

        // Cliente para Mobile com fluxos Password + Refresh Token
        var mobileClient = await manager.FindByClientIdAsync("mobile-client", cancellationToken);
        if (mobileClient is null)
        {
            var descriptor = new OpenIddictApplicationDescriptor
            {
                ClientId = "mobile-client",
                DisplayName = "Mobile Client",
                ClientType = ClientTypes.Public,
                Permissions =
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
                }
            };

            await manager.CreateAsync(descriptor, cancellationToken);
            _logger.LogInformation("✅ Client 'mobile-client' created");
        }
        else
        {
            _logger.LogInformation("Client 'mobile-client' already exists, updating...");

            // Atualiza configurações do mobile-client
            var descriptor = new OpenIddictApplicationDescriptor();
            await manager.PopulateAsync(descriptor, mobileClient, cancellationToken);

            descriptor.ClientType = ClientTypes.Public;
            descriptor.RedirectUris.Clear();
            descriptor.PostLogoutRedirectUris.Clear();

            // Limpa e adiciona permissões
            descriptor.Permissions.Clear();
            descriptor.Permissions.Add(Permissions.Endpoints.Token);
            descriptor.Permissions.Add(Permissions.GrantTypes.Password);
            descriptor.Permissions.Add(Permissions.GrantTypes.RefreshToken);
            descriptor.Permissions.Add(Scopes.OpenId);
            descriptor.Permissions.Add(Permissions.Scopes.Profile);
            descriptor.Permissions.Add(Permissions.Scopes.Email);
            descriptor.Permissions.Add(Permissions.Scopes.Address);
            descriptor.Permissions.Add(Permissions.Scopes.Phone);
            descriptor.Permissions.Add(Permissions.Scopes.Roles);

            descriptor.Permissions.Add(Permissions.Prefixes.Scope + "api");
            descriptor.Permissions.Add(Permissions.Prefixes.Scope + "chamados");

            await manager.UpdateAsync(mobileClient, descriptor, cancellationToken);
            _logger.LogInformation("✅ Client 'mobile-client' updated");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
