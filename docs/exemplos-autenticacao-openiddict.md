# Exemplos de Autenticação OpenIddict - EChamado

## 1. App Mobile/Desktop/CLI (Password Grant)

### Endpoint
```
POST https://localhost:7132/connect/token
Content-Type: application/x-www-form-urlencoded
```

### Request Body
```
grant_type=password
&username=admin@admin.com
&password=Admin@123
&client_id=mobile-client
&scope=openid profile email roles api chamados
```

### Exemplo cURL
```bash
curl -X POST https://localhost:7132/connect/token \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=password" \
  -d "username=admin@admin.com" \
  -d "password=Admin@123" \
  -d "client_id=mobile-client" \
  -d "scope=openid profile email roles api chamados"
```

### Exemplo C# (.NET)
```csharp
using System.Net.Http;
using System.Text.Json;

public class EchamadoAuthClient
{
    private readonly HttpClient _httpClient;

    public EchamadoAuthClient()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7132")
        };
    }

    public async Task<TokenResponse> LoginAsync(string username, string password)
    {
        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "password"),
            new KeyValuePair<string, string>("username", username),
            new KeyValuePair<string, string>("password", password),
            new KeyValuePair<string, string>("client_id", "mobile-client"),
            new KeyValuePair<string, string>("scope", "openid profile email roles api chamados")
        });

        var response = await _httpClient.PostAsync("/connect/token", content);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<TokenResponse>(json);
    }

    public async Task<TokenResponse> RefreshTokenAsync(string refreshToken)
    {
        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "refresh_token"),
            new KeyValuePair<string, string>("refresh_token", refreshToken),
            new KeyValuePair<string, string>("client_id", "mobile-client")
        });

        var response = await _httpClient.PostAsync("/connect/token", content);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<TokenResponse>(json);
    }
}

public class TokenResponse
{
    public string access_token { get; set; }
    public string token_type { get; set; }
    public int expires_in { get; set; }
    public string refresh_token { get; set; }
    public string id_token { get; set; }
}
```

### Exemplo Python
```python
import requests

def login_echamado(username, password):
    url = "https://localhost:7132/connect/token"

    data = {
        "grant_type": "password",
        "username": username,
        "password": password,
        "client_id": "mobile-client",
        "scope": "openid profile email roles api chamados"
    }

    response = requests.post(url, data=data, verify=False)  # verify=False apenas para dev
    response.raise_for_status()

    return response.json()

# Uso
token_response = login_echamado("admin@admin.com", "Admin@123")
access_token = token_response["access_token"]

# Usar token para chamar API
api_response = requests.get(
    "https://localhost:7296/v1/categories",
    headers={"Authorization": f"Bearer {access_token}"},
    verify=False
)
```

### Exemplo JavaScript/Node.js
```javascript
async function loginEchamado(username, password) {
    const url = 'https://localhost:7132/connect/token';

    const params = new URLSearchParams({
        grant_type: 'password',
        username: username,
        password: password,
        client_id: 'mobile-client',
        scope: 'openid profile email roles api chamados'
    });

    const response = await fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded'
        },
        body: params
    });

    if (!response.ok) {
        throw new Error(`Login failed: ${response.statusText}`);
    }

    return await response.json();
}

// Uso
const tokenResponse = await loginEchamado('admin@admin.com', 'Admin@123');
const accessToken = tokenResponse.access_token;

// Chamar API
const categories = await fetch('https://localhost:7296/v1/categories', {
    headers: {
        'Authorization': `Bearer ${accessToken}`
    }
});
```

### Response Esperada
```json
{
  "access_token": "eyJhbGciOiJSUzI1NiIsImtpZCI6IjEyMzQ1...",
  "token_type": "Bearer",
  "expires_in": 3600,
  "refresh_token": "CfDJ8Pz6...",
  "id_token": "eyJhbGciOiJSUzI1NiIsImtpZCI6IjEyMzQ1...",
  "scope": "openid profile email roles api chamados"
}
```

---

## 2. APIs/Serviços Backend (Client Credentials)

### Quando usar
- Comunicação entre serviços (M2M - Machine to Machine)
- Integração com sistemas externos
- Jobs/Workers que precisam acessar a API

### Criar Cliente para API Externa

Adicione em `OpenIddictWorker.cs`:

```csharp
private async Task CreateOrUpdateApiClientAsync(IOpenIddictApplicationManager manager, CancellationToken cancellationToken)
{
    var apiClient = await manager.FindByClientIdAsync("external-api-client", cancellationToken);
    if (apiClient is null)
    {
        var descriptor = new OpenIddictApplicationDescriptor
        {
            ClientId = "external-api-client",
            ClientSecret = "seu-secret-super-seguro-aqui", // Guardar em vault/secrets
            DisplayName = "API Externa",
            ClientType = OpenIddictConstants.ClientTypes.Confidential, // Confidencial tem secret
            Permissions =
            {
                OpenIddictConstants.Permissions.Endpoints.Token,
                OpenIddictConstants.Permissions.GrantTypes.ClientCredentials,

                OpenIddictConstants.Permissions.Prefixes.Scope + "api",
                OpenIddictConstants.Permissions.Prefixes.Scope + "chamados"
            }
        };

        await manager.CreateAsync(descriptor, cancellationToken);
    }
}

// Chamar no StartAsync:
await CreateOrUpdateApiClientAsync(manager, cancellationToken);
```

### Request
```bash
curl -X POST https://localhost:7132/connect/token \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=client_credentials" \
  -d "client_id=external-api-client" \
  -d "client_secret=seu-secret-super-seguro-aqui" \
  -d "scope=api chamados"
```

### Exemplo C#
```csharp
public async Task<string> GetApiTokenAsync()
{
    var client = new HttpClient();

    var content = new FormUrlEncodedContent(new[]
    {
        new KeyValuePair<string, string>("grant_type", "client_credentials"),
        new KeyValuePair<string, string>("client_id", "external-api-client"),
        new KeyValuePair<string, string>("client_secret", "seu-secret-super-seguro-aqui"),
        new KeyValuePair<string, string>("scope", "api chamados")
    });

    var response = await client.PostAsync("https://localhost:7132/connect/token", content);
    var json = await response.Content.ReadAsStringAsync();
    var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(json);

    return tokenResponse.access_token;
}
```

---

## 3. Single Page Applications (Authorization Code + PKCE)

**Já está configurado para o Blazor WASM** via `bwa-client`.

Outros SPAs (React, Angular, Vue) podem usar o mesmo fluxo:

### Exemplo com oidc-client-ts (TypeScript/JavaScript)
```typescript
import { UserManager } from 'oidc-client-ts';

const userManager = new UserManager({
    authority: 'https://localhost:7132',
    client_id: 'bwa-client',
    redirect_uri: 'https://localhost:3000/callback',
    post_logout_redirect_uri: 'https://localhost:3000',
    response_type: 'code',
    scope: 'openid profile email roles api chamados',

    // PKCE automático
    automaticSilentRenew: true,
    filterProtocolClaims: true,
    loadUserInfo: true
});

// Login
await userManager.signinRedirect();

// Callback
const user = await userManager.signinRedirectCallback();
const accessToken = user.access_token;
```

---

## 4. Refresh Token (Renovar tokens expirados)

### Request
```bash
curl -X POST https://localhost:7132/connect/token \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=refresh_token" \
  -d "refresh_token=CfDJ8Pz6..." \
  -d "client_id=mobile-client"
```

### Exemplo C#
```csharp
public async Task<TokenResponse> RefreshAsync(string refreshToken)
{
    var content = new FormUrlEncodedContent(new[]
    {
        new KeyValuePair<string, string>("grant_type", "refresh_token"),
        new KeyValuePair<string, string>("refresh_token", refreshToken),
        new KeyValuePair<string, string>("client_id", "mobile-client")
    });

    var response = await _httpClient.PostAsync("/connect/token", content);
    response.EnsureSuccessStatusCode();

    var json = await response.Content.ReadAsStringAsync();
    return JsonSerializer.Deserialize<TokenResponse>(json);
}
```

---

## Comparação: JWT Customizado vs OpenIddict

| Aspecto | JWT Customizado | OpenIddict |
|---------|----------------|------------|
| **Endpoint** | `/v1/auth/login` (porta 7296) | `/connect/token` (porta 7132) |
| **Padrão** | Implementação manual | OAuth 2.0 / OIDC (RFC 6749, RFC 7519) |
| **Grant Types** | Apenas username/password | Password, Client Credentials, Authorization Code, Refresh Token |
| **Refresh Token** | ❌ Não implementado | ✅ Suportado nativamente |
| **Introspection** | ❌ Não disponível | ✅ `/connect/introspect` |
| **Revogação** | ❌ Não disponível | ✅ Suportado |
| **PKCE** | ❌ Não suportado | ✅ Obrigatório para SPAs |
| **Scopes** | ❌ Não implementado | ✅ openid, profile, email, roles, api |
| **ID Token** | ❌ Não retorna | ✅ JWT com claims do usuário |
| **Múltiplos Clientes** | ❌ Não suportado | ✅ bwa-client, mobile-client, etc. |
| **Segurança** | ⚠️ HMAC-SHA256 (chave compartilhada) | ✅ RSA-SHA256 (certificados) |
| **Manutenção** | ⚠️ Código customizado | ✅ Biblioteca mantida pela comunidade |
| **Compatibilidade** | ⚠️ Apenas este projeto | ✅ Qualquer cliente OIDC (Auth0, IdentityServer, etc.) |

---

## Recomendação Final: Migrar para OpenIddict

### Vantagens
1. **Já está configurado** - Não precisa implementar nada novo
2. **Mais seguro** - Certificados RSA ao invés de chave simétrica
3. **Padrão da indústria** - OAuth 2.0 / OpenID Connect
4. **Suporta todos os cenários**:
   - ✅ Apps Mobile (Password Grant) → `mobile-client`
   - ✅ SPAs (Authorization Code + PKCE) → `bwa-client`
   - ✅ APIs/Serviços (Client Credentials) → Criar novo client
   - ✅ Refresh Tokens
5. **Menos código para manter** - Remove `GetTokenCommandHandler`, `LoginUserEndpoint`, etc.
6. **Melhor interoperabilidade** - Qualquer cliente OAuth/OIDC funciona

### Passos para Migração

1. **Atualizar documentação** - Indicar uso de `/connect/token`
2. **Remover código legado**:
   - `GetTokenCommand.cs`
   - `GetTokenCommandHandler.cs`
   - `LoginUserEndpoint.cs` (opcional, pode manter se quiser)
3. **Adicionar clientes conforme necessário** (ver exemplo de Client Credentials acima)

---

## Configuração de Novos Clientes

Para adicionar um novo sistema que precisa se autenticar:

```csharp
// OpenIddictWorker.cs
private async Task CreateClientAsync(IOpenIddictApplicationManager manager, string clientId, string displayName, string[] grantTypes)
{
    var client = await manager.FindByClientIdAsync(clientId);
    if (client is null)
    {
        var descriptor = new OpenIddictApplicationDescriptor
        {
            ClientId = clientId,
            DisplayName = displayName,
            ClientType = OpenIddictConstants.ClientTypes.Public,
            Permissions = new List<string>
            {
                OpenIddictConstants.Permissions.Endpoints.Token,
                OpenIddictConstants.Scopes.OpenId,
                OpenIddictConstants.Permissions.Scopes.Profile,
                OpenIddictConstants.Permissions.Scopes.Email,
                OpenIddictConstants.Permissions.Prefixes.Scope + "api"
            }
        };

        foreach (var grantType in grantTypes)
        {
            descriptor.Permissions.Add(grantType);
        }

        await manager.CreateAsync(descriptor);
    }
}

// Uso
await CreateClientAsync(manager, "power-bi-client", "PowerBI Integration",
    new[] { OpenIddictConstants.Permissions.GrantTypes.ClientCredentials });
```
