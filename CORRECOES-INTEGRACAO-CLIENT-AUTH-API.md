# 🔧 CORREÇÕES DE INTEGRAÇÃO - EChamado

**Data:** 2025-11-24
**Objetivo:** Corrigir integração entre Client, Auth Server e API Server

---

## 🔴 CRÍTICO: Fluxo de Autenticação Incompatível

### Problema:
O Client Blazor usa fluxo customizado (redirect com token na URL), mas OpenIddict está configurado para Authorization Code Flow padrão.

### Opção 1: Adaptar Client para Authorization Code Flow (RECOMENDADO)

**Vantagens:**
- ✅ Padrão OAuth2 seguro
- ✅ Compatível com OpenIddict sem modificações
- ✅ Melhor para produção

**Bibliotecas necessárias:**
```xml
<PackageReference Include="Microsoft.AspNetCore.Components.WebAssembly.Authentication" Version="9.0.0" />
```

**Implementação:**

#### 1. Program.cs (Client)
```csharp
builder.Services.AddOidcAuthentication(options =>
{
    options.ProviderOptions.Authority = "https://localhost:7133";
    options.ProviderOptions.ClientId = "bwa-client";
    options.ProviderOptions.ResponseType = "code";

    // Scopes
    options.ProviderOptions.DefaultScopes.Add("openid");
    options.ProviderOptions.DefaultScopes.Add("profile");
    options.ProviderOptions.DefaultScopes.Add("email");
    options.ProviderOptions.DefaultScopes.Add("roles");
    options.ProviderOptions.DefaultScopes.Add("api");
    options.ProviderOptions.DefaultScopes.Add("chamados");

    // Redirect URIs
    options.ProviderOptions.RedirectUri = "https://localhost:7274/authentication/login-callback";
    options.ProviderOptions.PostLogoutRedirectUri = "https://localhost:7274/authentication/logout-callback";
});

// HttpClient com token automático
builder.Services.AddHttpClient("EChamado.ServerAPI",
    client => client.BaseAddress = new Uri("https://localhost:7296"))
    .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
    .CreateClient("EChamado.ServerAPI"));
```

#### 2. App.razor
```razor
<CascadingAuthenticationState>
    <Router AppAssembly="@typeof(App).Assembly">
        <Found Context="routeData">
            <AuthorizeRouteView RouteData="@routeData" DefaultLayout="@typeof(MainLayout)">
                <NotAuthorized>
                    <RedirectToLogin />
                </NotAuthorized>
            </AuthorizeRouteView>
        </Found>
    </Router>
</CascadingAuthenticationState>
```

#### 3. Authentication.razor (novo arquivo)
```razor
@page "/authentication/{action}"
@using Microsoft.AspNetCore.Components.WebAssembly.Authentication

<RemoteAuthenticatorView Action="@Action" />

@code {
    [Parameter] public string? Action { get; set; }
}
```

#### 4. Remover arquivos antigos:
- ❌ `CookieAuthenticationStateProvider.cs`
- ❌ `CookieHandler.cs`
- ❌ `Login.razor`, `LoginCallback.razor`, `Logout.razor`, `LogoutCallback.razor`

---

### Opção 2: Manter Fluxo Customizado (Password Flow)

**Desvantagens:**
- ⚠️ Menos seguro (credenciais no client)
- ⚠️ Não funciona para SSO/OAuth2 externo

**Implementação:**

#### 1. Criar endpoint /connect/token no Auth Server

Já existe, mas precisa configurar client para Password Flow.

#### 2. Atualizar OpenIddictWorker.cs
```csharp
// Modificar bwa-client para também aceitar Password Flow
if (await manager.FindByClientIdAsync("bwa-client") is null)
{
    await manager.CreateAsync(new OpenIddictApplicationDescriptor
    {
        ClientId = "bwa-client",
        ClientSecret = "bwa_secret_2024", // Adicionar secret
        ConsentType = ConsentTypes.Implicit,
        DisplayName = "Blazor WebAssembly Client",
        Type = ClientTypes.Confidential, // Mudar para Confidential
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
            Permissions.GrantTypes.AuthorizationCode,
            Permissions.GrantTypes.RefreshToken,
            Permissions.GrantTypes.Password, // ADICIONAR
            Permissions.ResponseTypes.Code,
            // Scopes
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
```

#### 3. Criar serviço de autenticação no Client
```csharp
public class AuthService
{
    private readonly HttpClient _httpClient;

    public AuthService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://localhost:7133");
    }

    public async Task<TokenResponse> LoginAsync(string username, string password)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/connect/token");
        request.Content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "password"),
            new KeyValuePair<string, string>("client_id", "bwa-client"),
            new KeyValuePair<string, string>("client_secret", "bwa_secret_2024"),
            new KeyValuePair<string, string>("username", username),
            new KeyValuePair<string, string>("password", password),
            new KeyValuePair<string, string>("scope", "openid profile email roles api chamados")
        });

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<TokenResponse>();
    }
}

public class TokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }

    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; set; }

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    [JsonPropertyName("token_type")]
    public string TokenType { get; set; }
}
```

---

## 🟡 MÉDIO: Anexar Token JWT nas Requisições

### Problema:
Serviços não anexam `Authorization: Bearer <token>` nas requisições HTTP.

### Solução: Criar AuthorizationMessageHandler

#### 1. Criar BearerTokenHandler.cs
```csharp
public class BearerTokenHandler : DelegatingHandler
{
    private readonly IJSRuntime _jsRuntime;

    public BearerTokenHandler(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Recuperar token do localStorage
        var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");

        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
```

#### 2. Registrar no Program.cs
```csharp
builder.Services.AddScoped<BearerTokenHandler>();

builder.Services.AddHttpClient<OrderService>(client =>
{
    client.BaseAddress = new Uri(backendUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
})
.AddHttpMessageHandler<BearerTokenHandler>();

// Repetir para todos os serviços
builder.Services.AddHttpClient<CategoryService>(...)
    .AddHttpMessageHandler<BearerTokenHandler>();

builder.Services.AddHttpClient<DepartmentService>(...)
    .AddHttpMessageHandler<BearerTokenHandler>();

builder.Services.AddHttpClient<LookupService>(...)
    .AddHttpMessageHandler<BearerTokenHandler>();

builder.Services.AddHttpClient<CommentService>(...)
    .AddHttpMessageHandler<BearerTokenHandler>();
```

---

## 🟠 BAIXO: Limpar CORS Desnecessário

### Auth Server - Program.cs

**Antes:**
```csharp
policy.WithOrigins("https://localhost:5199", "https://localhost:7274",
                  "http://localhost:5199", "http://localhost:7274",
                  "https://localhost:7133", "http://localhost:5137")
```

**Depois:**
```csharp
policy.WithOrigins("https://localhost:7274",   // Client
                  "https://localhost:7296")    // API Server
```

### Server - Program.cs

**Manter como está:**
```csharp
policy.WithOrigins("https://localhost:7274", "https://localhost:7133")
      .AllowAnyHeader().AllowAnyMethod().AllowCredentials();
```
✅ Já está correto.

---

## 🔒 SEGURANÇA: Migrar de localStorage para HttpOnly Cookies

### Problema:
localStorage é vulnerável a XSS. Tokens devem estar em HttpOnly cookies.

### Implementação (Requer mudança no Auth Server e Client):

#### 1. Auth Server - AuthorizationController.cs
Modificar endpoint de token para retornar cookie ao invés de JSON:

```csharp
[HttpPost("~/connect/token")]
public async Task<IActionResult> Exchange()
{
    var request = HttpContext.GetOpenIddictServerRequest();

    // ... validação existente ...

    var result = SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

    // Adicionar token em cookie HttpOnly
    Response.Cookies.Append("access_token", accessToken, new CookieOptions
    {
        HttpOnly = true,
        Secure = true,
        SameSite = SameSiteMode.Strict,
        Expires = DateTimeOffset.UtcNow.AddHours(1)
    });

    return result;
}
```

#### 2. Client - Remover localStorage
Remover todo código que usa `localStorage.setItem/getItem` e confiar nos cookies automáticos.

---

## 📋 CHECKLIST DE IMPLEMENTAÇÃO

### Fase 1 - Correção Crítica (Escolher Opção 1 OU Opção 2):
- [ ] **Opção 1 (Recomendado):** Implementar Authorization Code Flow padrão
  - [ ] Adicionar pacote `Microsoft.AspNetCore.Components.WebAssembly.Authentication`
  - [ ] Configurar `AddOidcAuthentication` no Program.cs
  - [ ] Criar `Authentication.razor`
  - [ ] Remover arquivos customizados antigos
  - [ ] Testar fluxo de login/logout

- [ ] **Opção 2:** Implementar Password Flow
  - [ ] Modificar `bwa-client` no OpenIddictWorker
  - [ ] Criar `AuthService` no Client
  - [ ] Atualizar Login.razor para usar AuthService
  - [ ] Testar autenticação

### Fase 2 - Anexar Tokens:
- [ ] Criar `BearerTokenHandler.cs`
- [ ] Registrar handler em todos os HttpClients
- [ ] Testar requisições autenticadas para API

### Fase 3 - Limpeza:
- [ ] Limpar CORS no Auth Server
- [ ] Remover origens HTTP desnecessárias
- [ ] Validar que tudo funciona com apenas HTTPS

### Fase 4 - Segurança (Opcional para Produção):
- [ ] Migrar de localStorage para HttpOnly cookies
- [ ] Atualizar Auth Server para retornar cookies
- [ ] Remover código de localStorage do Client

---

## 🧪 TESTES RECOMENDADOS

### Após Fase 1:
1. Acessar `https://localhost:7274`
2. Clicar em "Login"
3. Redirecionar para Auth Server
4. Fazer login com credenciais válidas
5. Verificar redirecionamento para Client
6. Verificar que usuário está autenticado

### Após Fase 2:
1. Fazer login
2. Tentar acessar endpoint protegido: `GET /v1/orders`
3. Verificar que requisição tem header: `Authorization: Bearer <token>`
4. Verificar resposta 200 OK (não 401 Unauthorized)

### Após Fase 3:
1. Verificar que apenas HTTPS funciona
2. Testar em diferentes browsers
3. Verificar console do browser para erros de CORS

---

**Prioridade de Implementação:**
1. 🔴 **Fase 1** (Crítico) - Sem isso, autenticação não funciona
2. 🟡 **Fase 2** (Alto) - Sem isso, API retorna 401
3. 🟠 **Fase 3** (Médio) - Limpeza e boas práticas
4. 🔒 **Fase 4** (Baixo/Produção) - Segurança adicional

---

**Revisão por:** Claude Code (Senior SWE)
**Data:** 2025-11-24
**Status:** Pronto para implementação
