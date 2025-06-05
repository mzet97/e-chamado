# Prompt para Codex

Você é um assistente que gera alterações C# diretamente em um repositório ASP.NET Core (.NET 9) hospedado em GitHub ([https://github.com/mzet97/e-chamado](https://github.com/mzet97/e-chamado)). Seu objetivo é:

1. **No projeto `EChamado.Server`** (API com OpenIddict), habilitar o fluxo Authorization Code com PKCE, mas delegando a tela de login a um Blazor Server externo.
2. **No projeto `EChamado.Client`** (Blazor WebAssembly), configurar a autenticação OIDC (Authorization Code + PKCE) para se comunicar com a API/OpenIddict e processar login, logout, callbacks, proteção de rotas, serviço de exemplo etc.

Use o layout, nomes e namespaces exatamente como estão no repositório. Siga o passo a passo abaixo para editar ou criar cada arquivo.

---

## 1. Estrutura de pastas relevante

```
e-chamado/
├── EChamado.Server/
│   ├── Controllers/
│   │   └── AuthorizationController.cs
│   ├── Infrastructure/
│   │   ├── Configuration/
│   │   │   └── IdentityConfig.cs
│   │   ├── OpenIddict/
│   │   │   └── OpenIddictWorker.cs
│   └── Persistence/
│       └── ApplicationDbContext.cs
│   └── Program.cs
├── EChamado.Client/
│   ├── wwwroot/
│   │   └── appsettings.json
│   ├── Pages/
│   │   ├── Authentication/
│   │   │   ├── Login.razor
│   │   │   ├── LoginCallback.razor
│   │   │   ├── Logout.razor
│   │   │   └── LogoutCallback.razor
│   ├── Shared/
│   │   └── LoginDisplay.razor
│   ├── Services/
│   │   └── ChamadoService.cs
│   └── Program.cs
└── EChamado.Shared/
    └── Shared/Settings/
        ├── AppSettings.cs
        └── ClientSettings.cs
└── EChamado.sln
```

---

## 2. Modificações no `EChamado.Server` (API + OpenIddict)

### 2.1. IdentityConfig.cs (`EChamado.Server/Infrastructure/Configuration/IdentityConfig.cs`)

Substitua o conteúdo de **IdentityConfig.cs** pelo código abaixo:

```csharp
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
```

### 2.2. OpenIddictWorker.cs (`EChamado.Server/Infrastructure/OpenIddict/OpenIddictWorker.cs`)

Substitua o conteúdo de **OpenIddictWorker.cs** por:

```csharp
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
```

### 2.3. AuthorizationController.cs (`EChamado.Server/Controllers/AuthorizationController.cs`)

Substitua o conteúdo de **AuthorizationController.cs** por:

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;
using EChamado.Server.Domain.Services.Interface;

namespace EChamado.Server.Controllers
{
    public class AuthorizationController(
        IOpenIddictApplicationManager applicationManager,
        IOpenIddictService openIddictService
    ) : Controller
    {
        [HttpGet("~/connect/authorize")]
        [HttpPost("~/connect/authorize")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Authorize()
        {
            var request = HttpContext.GetOpenIddictServerRequest()
                          ?? throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

            // Tenta obter o usuário autenticado via cookie
            var result = await HttpContext.AuthenticateAsync();
            if (!result.Succeeded)
            {
                // Se não estiver autenticado, redireciona para a aplicação externa de login (esquema "External")
                return Challenge(
                    authenticationSchemes: new[] { "External" },
                    properties: new AuthenticationProperties
                    {
                        RedirectUri = Request.PathBase + Request.Path +
                                      QueryString.Create(Request.HasFormContentType
                                          ? Request.Form.ToList()
                                          : Request.Query.ToList())
                    });
            }

            // Se autenticado, cria claims principal para gerar authorization code
            var claims = new List<Claim>
            {
                new Claim(Claims.Subject, result.Principal.Identity.Name),
                new Claim(Claims.Email, result.Principal.FindFirst(Claims.Email)?.Value ?? string.Empty),
                new Claim(Claims.Name, result.Principal.Identity.Name ?? string.Empty)
            };

            var claimsIdentity = new ClaimsIdentity(claims, TokenValidationParameters.DefaultAuthenticationType);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            // Seta os escopos solicitados
            claimsPrincipal.SetScopes(request.GetScopes());

            return SignIn(claimsPrincipal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        [HttpPost("~/connect/token"), Produces("application/json")]
        public async Task<IActionResult> Exchange()
        {
            var request = HttpContext.GetOpenIddictServerRequest();

            if (request.IsClientCredentialsGrantType())
            {
                var application = await applicationManager.FindByClientIdAsync(request.ClientId)
                                  ?? throw new InvalidOperationException("The application cannot be found.");

                var identity = new ClaimsIdentity(TokenValidationParameters.DefaultAuthenticationType, Claims.Name, Claims.Role);
                identity.SetClaim(Claims.Subject, await applicationManager.GetClientIdAsync(application));
                identity.SetClaim(Claims.Name, await applicationManager.GetDisplayNameAsync(application));

                identity.SetDestinations(claim => claim.Type switch
                {
                    Claims.Name when claim.Subject.HasScope(Scopes.Profile) =>
                        new[] { Destinations.AccessToken, Destinations.IdentityToken },
                    _ => new[] { Destinations.AccessToken }
                });

                return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }

            if (request.IsPasswordGrantType())
            {
                var identity = await openIddictService.LoginOpenIddictAsync(request.Username, request.Password);
                if (identity == null)
                {
                    return Forbid(
                        authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                        properties: new AuthenticationProperties(new Dictionary<string, string>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The username/password couple is invalid."
                        }));
                }

                identity.SetDestinations(claim => claim.Type switch
                {
                    Claims.Name or Claims.Email when claim.Subject.HasScope(Scopes.Profile) =>
                        new[] { Destinations.AccessToken, Destinations.IdentityToken },
                    Claims.Role => new[] { Destinations.AccessToken },
                    _ => new[] { Destinations.AccessToken }
                });

                return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }

            if (request.IsAuthorizationCodeGrantType())
            {
                var principal = (await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)).Principal;
                principal.SetDestinations(claim => claim.Type switch
                {
                    Claims.Name or Claims.Email when principal.HasScope(Scopes.Profile) =>
                        new[] { Destinations.AccessToken, Destinations.IdentityToken },
                    Claims.Role => new[] { Destinations.AccessToken },
                    _ => new[] { Destinations.AccessToken }
                });

                return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }

            throw new NotImplementedException("The specified grant is not implemented.");
        }
    }
}
```

### 2.4. Program.cs do `EChamado.Server` (`EChamado.Server/Program.cs`)

Ajuste seu **Program.cs** para ter a seguinte ordem:

```csharp
var builder = WebApplication.CreateBuilder(args);

// ... outras configurações de serviços ...
builder.Services.AddIdentityConfig(builder.Configuration);
// ... adicionar controllers, etc. ...

var app = builder.Build();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
```

Não adicione `MapRazorPages()` na API, pois as telas de login ficam na aplicação Blazor Server separada.

---

## 3. Configurações de `EChamado.Shared/Shared/Settings`

Os arquivos `AppSettings.cs` e `ClientSettings.cs` já existem e não requerem alteração. Certifique-se de que o seu **appsettings.json** (da API) contenha:

```json
{
  "AppSettings": {
    "Secret": "sua-chave-secreta-aqui",
    "ValidOn": "https://localhost:7296",
    "Issuer": "EChamado"
  },
  "ClientSettings": {
    "Clients": []
  },
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=EChamadoDb;Username=postgres;Password=senha"
  }
}
```

---

## 4. Modificações no `EChamado.Client` (Blazor WebAssembly)

### 4.1. `wwwroot/appsettings.json`

Substitua ou crie este arquivo com o conteúdo:

```json
{
  "oidc": {
    "Authority": "https://localhost:7296",
    "ClientId": "bwa-client",
    "DefaultScopes": ["openid", "profile", "email", "api", "chamados"],
    "ResponseType": "code",
    "PostLogoutRedirectUri": "https://localhost:7274/authentication/logout-callback",
    "RedirectUri": "https://localhost:7274/authentication/login-callback"
  },
  "BackendUrl": "https://localhost:7296"
}
```

### 4.2. Program.cs (`EChamado.Client/Program.cs`)

Substitua ou ajuste `Program.cs` para:

```csharp
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace EChamado.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            // Configura HttpClient normal
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            // Configuração OIDC (Authorization Code + PKCE)
            builder.Services.AddOidcAuthentication(options =>
            {
                // Carrega as configurações de appsettings.json
                builder.Configuration.Bind("oidc", options.ProviderOptions);

                // Garante os escopos necessários
                options.ProviderOptions.DefaultScopes.Clear();
                foreach (var scope in builder.Configuration.GetSection("oidc:DefaultScopes").Get<string[]>())
                {
                    options.ProviderOptions.DefaultScopes.Add(scope);
                }
            });

            // HttpClient que usa tokens para chamadas autenticadas
            builder.Services.AddHttpClient<ChamadoService>(client =>
            {
                client.BaseAddress = new Uri(builder.Configuration["BackendUrl"]);
            })
            .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

            builder.Services.AddScoped<AuthenticationStateProvider, 
                RemoteAuthenticationService<RemoteAuthenticationState, RemoteUserAccount, OidcProviderOptions>>();

            await builder.Build().RunAsync();
        }
    }
}
```

### 4.3. Páginas de Autenticação (`EChamado.Client/Pages/Authentication/`)

Crie ou ajuste as quatro páginas:

#### 4.3.1. Login.razor

```razor
@page "/authentication/login"
@inject NavigationManager Navigation

<PageTitle>Login</PageTitle>

<RemoteAuthenticatorView Action="login" />
```

#### 4.3.2. LoginCallback.razor

```razor
@page "/authentication/login-callback"
@inject NavigationManager Navigation

<PageTitle>Login Callback</PageTitle>

<RemoteAuthenticatorView Action="login-callback" />
```

#### 4.3.3. Logout.razor

```razor
@page "/authentication/logout"
@inject NavigationManager Navigation

<PageTitle>Logout</PageTitle>

<RemoteAuthenticatorView Action="logout" />
```

#### 4.3.4. LogoutCallback.razor

```razor
@page "/authentication/logout-callback"
@inject NavigationManager Navigation

<PageTitle>Logout Callback</PageTitle>

<RemoteAuthenticatorView Action="logout-callback" />
```

### 4.4. `LoginDisplay.razor` (`EChamado.Client/Shared/LoginDisplay.razor`)

Crie ou ajuste para exibir status de autenticação:

```razor
@using Microsoft.AspNetCore.Components.WebAssembly.Authentication
@inject NavigationManager Navigation

<AuthorizeView>
    <Authorized>
        <button class="btn btn-link" @onclick="BeginLogout">Logout</button>
    </Authorized>
    <NotAuthorized>
        <button class="btn btn-link" @onclick="BeginLogin">Login</button>
    </NotAuthorized>
</AuthorizeView>

@code {
    private void BeginLogin()
    {
        Navigation.NavigateTo("authentication/login");
    }

    private void BeginLogout()
    {
        Navigation.NavigateTo("authentication/logout");
    }
}
```

### 4.5. Exemplo de Proteção de Rotas (`App.razor`)

Ajuste para usar `AuthorizeRouteView`:

```razor
<CascadingAuthenticationState>
    <Router AppAssembly="@typeof(App).Assembly">
        <Found Context="routeData">
            <AuthorizeRouteView RouteData="@routeData" DefaultLayout="@typeof(MainLayout)" />
        </Found>
        <NotFound>
            <LayoutView Layout="@typeof(MainLayout)">
                <p>Sorry, there's nothing at this address.</p>
            </LayoutView>
        </NotFound>
    </Router>
</CascadingAuthenticationState>
```

Não esqueça de adicionar:

```csharp
@using Microsoft.AspNetCore.Components.Authorization
@using Microsoft.AspNetCore.Components.WebAssembly.Authentication
```

### 4.6. `ChamadoService.cs` (`EChamado.Client/Services/ChamadoService.cs`)

Exemplo de serviço que usa tokens para chamadas autenticadas:

```csharp
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace EChamado.Client.Services
{
    public class ChamadoService
    {
        private readonly HttpClient _httpClient;

        public ChamadoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<DepartamentoDto>> GetDepartamentosAsync()
        {
            // A mensagem de HTTP incluirá automaticamente o access token
            var result = await _httpClient.GetFromJsonAsync<IEnumerable<DepartamentoDto>>("v1/departments");
            return result;
        }
    }

    public class DepartamentoDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public bool Ativo { get; set; }
    }
}
```

### 4.7. Protegendo uma Página Exemplo

Em `Pages/Protected.razor` (novo ou existente):

```razor
@page "/protected"
@attribute [Authorize]

<h3>Página Protegida</h3>
<p>Esta página só é acessível para usuários autenticados.</p>
```

E.g., adicione ao menu nav, etc.

---

## 5. Resumo dos Ajustes

1. **`EChamado.Server`**

   * Atualizar `IdentityConfig.cs` (usar cookie “External” que aponta para `https://localhost:7132/Account/Login`)
   * Atualizar `OpenIddictWorker.cs` (criar/atualizar clientes “bwa-client” e “mobile-client” com as permissões, URIs, PKCE).
   * Atualizar `AuthorizationController.cs` (Challenge("External") no método `Authorize()`, manter lógica de código, token).
   * Ajustar `Program.cs` para chamar `UseAuthentication()`, `UseAuthorization()`, `MapControllers()`.

2. **`EChamado.Client`**

   * Criar/ajustar `wwwroot/appsettings.json` com seções `oidc` e `BackendUrl`.
   * Ajustar `Program.cs` para usar `AddOidcAuthentication` com Authority = `https://localhost:7296`, ClientId = `bwa-client`, ResponseType = `code`, DefaultScopes = `[ "openid","profile","email","api","chamados" ]`.
   * Adicionar páginas em `Pages/Authentication/`:

     * `Login.razor`, `LoginCallback.razor`, `Logout.razor`, `LogoutCallback.razor`, todas usando `<RemoteAuthenticatorView>`.
   * Ajustar `App.razor` para usar `CascadingAuthenticationState` + `AuthorizeRouteView`.
   * Criar `LoginDisplay.razor` em `Shared/` para exibir botões de login/logout.
   * Criar `ChamadoService.cs` em `Services/` que injeta `HttpClient` e faz requisições autenticadas.
   * Proteger páginas com `@attribute [Authorize]`.

3. **URLs e Portas**

   * **Blazor WASM (Client)**: `https://localhost:7274`
   * **API Server (OpenIddict)**: `https://localhost:7296`
   * **Blazor Server (Identity)**: `https://localhost:7132`

4. **AppSettings JSON** (`EChamado.Server/appsettings.json`):

   ```json
   {
     "AppSettings": {
       "Secret": "sua-chave-secreta-aqui",
       "ValidOn": "https://localhost:7296",
       "Issuer": "EChamado"
     },
     "ClientSettings": {
       "Clients": []
     },
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Database=EChamadoDb;Username=postgres;Password=senha"
     }
   }
   ```

5. **Fluxo Completo**

   * O cliente chama `/connect/authorize` → API redireciona (Challenge) para `https://localhost:7132/Account/Login?returnUrl=...` → Blazor Server exibe login → usuário faz login → cookie criado → volta para `/connect/authorize` → OpenIddict gera código → redireciona para `https://localhost:7274/authentication/login-callback?code=...` → Blazor WASM troca código por tokens em `/connect/token` → tokens retornados, usuário autenticado no client.

Com este prompt, o Codex terá todas as instruções detalhadas de alteração/criação de arquivos para implementar o fluxo OIDC completo, tanto no servidor (`EChamado.Server`) quanto no cliente (`EChamado.Client`).
