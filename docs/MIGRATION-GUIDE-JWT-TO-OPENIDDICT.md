# Guia de Migração: JWT Customizado → OpenIddict

**Data da Migração:** 19 de Novembro de 2025
**Motivo:** Consolidação em um único sistema de autenticação padrão (OAuth 2.0 / OpenID Connect)

---

## 📋 Sumário Executivo

O sistema de autenticação JWT customizado foi **completamente removido** e substituído pelo **OpenIddict**. Esta mudança:

✅ **Simplifica** a arquitetura (um único sistema de auth ao invés de dois)
✅ **Melhora a segurança** (RSA-SHA256 ao invés de HMAC-SHA256)
✅ **Aumenta a compatibilidade** (padrão OAuth 2.0 / OIDC)
✅ **Suporta mais cenários** (SPAs, Mobile, M2M, Refresh Tokens)
✅ **Reduz manutenção** (menos código customizado)

---

## ❌ O Que Foi Removido

### Endpoints Removidos

| Endpoint Antigo | Status | Substituído Por |
|----------------|--------|-----------------|
| `POST /v1/auth/login` | ❌ Removido | `POST /connect/token` (porta 7132) |
| `POST /v1/auth/register` | ❌ Removido | Registro via Auth Server (porta 7132) |

### Arquivos Removidos

**Commands & Handlers:**
```
❌ Server/EChamado.Server.Application/UseCases/Auth/Commands/GetTokenCommand.cs
❌ Server/EChamado.Server.Application/UseCases/Auth/Commands/Handlers/GetTokenCommandHandler.cs
❌ Server/EChamado.Server.Application/UseCases/Auth/Commands/LoginUserCommand.cs
❌ Server/EChamado.Server.Application/UseCases/Auth/Commands/Handlers/LoginUserCommandHandler.cs
❌ Server/EChamado.Server.Application/UseCases/Auth/Commands/RegisterUserCommand.cs
❌ Server/EChamado.Server.Application/UseCases/Auth/Commands/Handlers/RegisterUserCommandHandler.cs
```

**Endpoints:**
```
❌ Server/EChamado.Server/Endpoints/Auth/LoginUserEndpoint.cs
❌ Server/EChamado.Server/Endpoints/Auth/RegisterUserEndpoint.cs
```

**DTOs:**
```
❌ Server/EChamado.Server/Endpoints/Auth/DTOs/LoginRequestDto.cs
❌ Server/EChamado.Server/Endpoints/Auth/DTOs/RegisterRequestDto.cs
❌ Server/EChamado.Server/Endpoints/Auth/DTOs/AuthDTOSExtensions.cs
```

**Notifications:**
```
❌ Server/EChamado.Server.Application/UseCases/Auth/Notifications/LoginUserNotification.cs
❌ Server/EChamado.Server.Application/UseCases/Auth/Notifications/RegisterUserNotification.cs
❌ Server/EChamado.Server.Application/UseCases/Auth/Notifications/Handlers/AuthNotificationHandler.cs
```

**Diretórios:**
```
❌ Server/EChamado.Server/Endpoints/Auth/
❌ Server/EChamado.Server.Application/UseCases/Auth/
```

---

## ✅ Como Migrar Seu Código

### Cenário 1: Você Estava Fazendo Login via API

**❌ ANTES (JWT Customizado):**
```csharp
// NÃO FUNCIONA MAIS!
var response = await httpClient.PostAsJsonAsync("/v1/auth/login", new
{
    Email = "admin@admin.com",
    Password = "Admin@123"
});

var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
var token = result.Data.AccessToken;
```

**✅ AGORA (OpenIddict):**
```csharp
var content = new FormUrlEncodedContent(new[]
{
    new KeyValuePair<string, string>("grant_type", "password"),
    new KeyValuePair<string, string>("username", "admin@admin.com"),
    new KeyValuePair<string, string>("password", "Admin@123"),
    new KeyValuePair<string, string>("client_id", "mobile-client"),
    new KeyValuePair<string, string>("scope", "openid profile email roles api chamados")
});

// IMPORTANTE: Auth Server está na porta 7132, não 7296
var authClient = new HttpClient { BaseAddress = new Uri("https://localhost:7132") };
var response = await authClient.PostAsync("/connect/token", content);

var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
var accessToken = tokenResponse.access_token;
var refreshToken = tokenResponse.refresh_token; // Novo! Permite renovar o token
```

### Cenário 2: Você Estava Testando via cURL/Postman

**❌ ANTES:**
```bash
curl -X POST https://localhost:7296/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@admin.com","password":"Admin@123"}'
```

**✅ AGORA:**
```bash
# ATENÇÃO: Porta mudou de 7296 para 7132
curl -X POST https://localhost:7132/connect/token \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=password" \
  -d "username=admin@admin.com" \
  -d "password=Admin@123" \
  -d "client_id=mobile-client" \
  -d "scope=openid profile email roles api chamados"
```

### Cenário 3: Você Estava Usando o Token na API

**✅ ISSO NÃO MUDOU!**

Usar o token nas chamadas à API permanece **exatamente igual**:

```bash
curl -X GET https://localhost:7296/v1/categories \
  -H "Authorization: Bearer {ACCESS_TOKEN}"
```

```csharp
httpClient.DefaultRequestHeaders.Authorization =
    new AuthenticationHeaderValue("Bearer", accessToken);

var categories = await httpClient.GetFromJsonAsync<List<Category>>("/v1/categories");
```

---

## 🔐 Novos Recursos Disponíveis

### 1. Refresh Token (Renovar Token Expirado)

**❌ ANTES:** Não era possível. Tinha que fazer login novamente.

**✅ AGORA:**
```csharp
// Salve o refresh_token quando fizer login
var refreshToken = tokenResponse.refresh_token;

// Quando o access_token expirar:
var refreshContent = new FormUrlEncodedContent(new[]
{
    new KeyValuePair<string, string>("grant_type", "refresh_token"),
    new KeyValuePair<string, string>("refresh_token", refreshToken),
    new KeyValuePair<string, string>("client_id", "mobile-client")
});

var response = await authClient.PostAsync("/connect/token", refreshContent);
var newTokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
var newAccessToken = newTokenResponse.access_token; // Token renovado!
```

### 2. ID Token (Informações do Usuário)

**✅ NOVO:**
```csharp
var idToken = tokenResponse.id_token; // JWT com informações do usuário

// Decodificar para ver claims:
var handler = new JwtSecurityTokenHandler();
var jwtToken = handler.ReadJwtToken(idToken);

var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
var email = jwtToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
var roles = jwtToken.Claims.Where(c => c.Type == "role").Select(c => c.Value);
```

### 3. Múltiplos Clientes Configurados

**✅ NOVO:**

Agora você pode ter diferentes configurações por tipo de aplicação:

| Cliente | Grant Type | Uso |
|---------|-----------|-----|
| `bwa-client` | Authorization Code + PKCE | Blazor WASM, SPAs |
| `mobile-client` | Password Grant | Apps Mobile, Desktop, CLIs |
| `{custom}` | Client Credentials | APIs M2M, Jobs, Serviços |

---

## 🧪 Scripts de Teste

Criamos 3 scripts prontos para testar a autenticação:

```bash
# Bash/Linux/WSL
./test-openiddict-login.sh

# PowerShell/Windows
.\test-openiddict-login.ps1

# Python
python test-openiddict-login.py
```

Todos os scripts:
- ✅ Fazem login automaticamente
- ✅ Testam chamada à API
- ✅ Testam refresh token
- ✅ Salvam tokens em `.tokens.json`

---

## 🔧 Configuração de Novos Clientes

Se você precisa criar um novo cliente (ex: integração com PowerBI, app desktop corporativo):

**1. Adicione em `OpenIddictWorker.cs`:**

```csharp
private async Task CreateMyCustomClientAsync(IOpenIddictApplicationManager manager, CancellationToken cancellationToken)
{
    var client = await manager.FindByClientIdAsync("meu-cliente-id", cancellationToken);
    if (client is null)
    {
        var descriptor = new OpenIddictApplicationDescriptor
        {
            ClientId = "meu-cliente-id",
            DisplayName = "Meu Cliente Customizado",
            ClientType = OpenIddictConstants.ClientTypes.Public, // ou Confidential se tiver secret
            Permissions =
            {
                OpenIddictConstants.Permissions.Endpoints.Token,
                OpenIddictConstants.Permissions.GrantTypes.Password,
                OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
                OpenIddictConstants.Permissions.Prefixes.Scope + "api"
            }
        };

        await manager.CreateAsync(descriptor, cancellationToken);
    }
}

// No método StartAsync:
public async Task StartAsync(CancellationToken cancellationToken)
{
    // ... código existente ...
    await CreateMyCustomClientAsync(manager, cancellationToken);
}
```

**2. Use o novo cliente:**

```bash
curl -X POST https://localhost:7132/connect/token \
  -d "grant_type=password" \
  -d "username=admin@admin.com" \
  -d "password=Admin@123" \
  -d "client_id=meu-cliente-id" \
  -d "scope=api"
```

---

## 📊 Comparação de Tokens

### Token JWT Customizado (Removido)

```json
{
  "alg": "HS256",
  "typ": "JWT"
}
{
  "sub": "0423b631-98e0-4e8d-a93b-37c63b528572",
  "email": "admin@admin.com",
  "jti": "7a47ddf8-36c7-40ce-be72-5057c4d46ab0",
  "nbf": 1763573246,
  "iat": 1763573246,
  "exp": 1763659646,
  "iss": "Echamado",
  "aud": "https://localhost:7296"
}
```

**Problemas:**
- ❌ HMAC-SHA256 (chave simétrica compartilhada - menos seguro)
- ❌ Sem refresh token
- ❌ Sem ID token
- ❌ Não suporta revogação
- ❌ Não suporta introspection

### Token OpenIddict (Atual)

**Access Token:**
```json
{
  "alg": "RS256",  // RSA ao invés de HMAC!
  "typ": "at+jwt",
  "kid": "ABC123"
}
{
  "sub": "0423b631-98e0-4e8d-a93b-37c63b528572",
  "email": "admin@admin.com",
  "name": "Administrator",
  "role": ["Admin", "User"],
  "iat": 1763573246,
  "exp": 1763576846,
  "iss": "https://localhost:7132",
  "aud": "https://localhost:7296",
  "client_id": "mobile-client",
  "oi_tkn_id": "unique-token-id"
}
```

**Vantagens:**
- ✅ RSA-SHA256 (certificados - mais seguro)
- ✅ Refresh token incluído
- ✅ ID token separado com claims do usuário
- ✅ Suporta revogação (`/connect/revoke`)
- ✅ Suporta introspection (`/connect/introspect`)
- ✅ Token ID único para rastreamento

---

## 🚨 Troubleshooting

### Erro: "Connection refused" ao chamar /connect/token

**Causa:** Auth Server não está rodando

**Solução:**
```bash
cd src/EChamado/Echamado.Auth
dotnet run
```

### Erro: "invalid_grant" ou "invalid_client"

**Causa:** Credenciais incorretas ou cliente não existe

**Solução:**
1. Verificar usuários seeded: `admin@admin.com` / `Admin@123`
2. Verificar se `OpenIddictWorker` criou os clientes
3. Verificar logs do Auth Server

### Erro: 401 Unauthorized mesmo com token válido

**Causa:** API não está validando tokens OpenIddict corretamente

**Solução:**
1. Verificar que API Server está rodando (porta 7296)
2. Verificar `IdentityConfig.cs:184`: `options.SetIssuer(new Uri("https://localhost:7132"));`
3. Verificar que o token foi obtido do Auth Server (porta 7132)

### Token expira muito rápido

**Solução:** Ajustar em `Echamado.Auth/Program.cs`:

```csharp
options.SetAccessTokenLifetime(TimeSpan.FromHours(1)); // Padrão: 1 hora
options.SetRefreshTokenLifetime(TimeSpan.FromDays(14)); // Padrão: 14 dias
```

---

## 📚 Documentação Adicional

| Documento | Descrição |
|-----------|-----------|
| **AUTENTICACAO-SISTEMAS-EXTERNOS.md** | Guia completo de autenticação para sistemas externos |
| **exemplos-autenticacao-openiddict.md** | Exemplos práticos em C#, Python, JavaScript, PowerShell |
| **CLAUDE.md** | Documentação principal do projeto (atualizada) |
| **test-openiddict-login.sh** | Script de teste Bash |
| **test-openiddict-login.ps1** | Script de teste PowerShell |
| **test-openiddict-login.py** | Script de teste Python |

---

## 📞 Suporte

Se encontrar problemas durante a migração:

1. Verifique se os 2 servidores estão rodando (Auth: 7132, API: 7296)
2. Execute um dos scripts de teste para validar a configuração
3. Consulte os logs em Elasticsearch (Kibana: http://localhost:5601)
4. Consulte a documentação do OpenIddict: https://documentation.openiddict.com/

---

## ✅ Checklist de Migração

Use este checklist para garantir que sua migração está completa:

- [ ] Auth Server rodando na porta 7132
- [ ] API Server rodando na porta 7296
- [ ] Testei obter token via `/connect/token`
- [ ] Testei usar token na API `/v1/categories`
- [ ] Testei refresh token
- [ ] Atualizei código do cliente para usar novo endpoint
- [ ] Removi referências ao endpoint `/v1/auth/login`
- [ ] Executei pelo menos um dos scripts de teste
- [ ] Li a documentação em `AUTENTICACAO-SISTEMAS-EXTERNOS.md`
- [ ] Configurei novos clientes se necessário

---

**Data da Última Atualização:** 19 de Novembro de 2025
**Versão:** 1.0.0
