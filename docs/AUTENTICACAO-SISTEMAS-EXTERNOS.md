# Autenticação para Sistemas Externos - EChamado

## 🚨 Problema Atual

Você está usando um **token JWT customizado** que **NÃO é compatível** com o servidor de API:

```bash
# ❌ Este token NÃO funciona com a API
curl -X GET 'https://localhost:7296/v1/categories' \
  -H 'Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...'
```

**Motivo:** A API está configurada para aceitar apenas tokens **OpenIddict**, não JWT customizado.

---

## ✅ Solução: Usar OpenIddict

O sistema **JÁ ESTÁ CONFIGURADO** para autenticação com OpenIddict. Não precisa implementar nada!

### Como Obter Token Válido

#### Opção 1: cURL (Linha de comando)
```bash
# Obter token
curl -X POST https://localhost:7132/connect/token \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=password" \
  -d "username=admin@admin.com" \
  -d "password=Admin@123" \
  -d "client_id=mobile-client" \
  -d "scope=openid profile email roles api chamados"

# Usar token na API
curl -X GET 'https://localhost:7296/v1/categories' \
  -H 'Authorization: Bearer {TOKEN_AQUI}'
```

#### Opção 2: Script Automatizado

Criamos 3 scripts para facilitar o teste:

**Bash/Linux/WSL:**
```bash
./test-openiddict-login.sh
```

**PowerShell/Windows:**
```powershell
.\test-openiddict-login.ps1
```

**Python:**
```bash
python test-openiddict-login.py
```

Todos os scripts:
- ✅ Obtêm o token automaticamente
- ✅ Testam a chamada à API
- ✅ Decodificam e mostram o payload
- ✅ Testam refresh token
- ✅ Salvam os tokens em `.tokens.json`

---

## 🔐 Tipos de Autenticação Suportados

O OpenIddict já suporta **4 fluxos de autenticação** configurados em `AuthorizationController.cs`:

### 1. Password Grant (Apps Mobile/Desktop/CLI)
**Cliente:** `mobile-client` (já configurado)

```bash
POST /connect/token
grant_type=password
username=admin@admin.com
password=Admin@123
client_id=mobile-client
scope=openid profile email roles api
```

**Quando usar:**
- ✅ Apps mobile (Android/iOS)
- ✅ Apps desktop (.NET, Electron, etc.)
- ✅ Scripts CLI/automação
- ✅ Testes rápidos

### 2. Authorization Code + PKCE (SPAs)
**Cliente:** `bwa-client` (já configurado para Blazor)

```javascript
// React/Vue/Angular
const userManager = new UserManager({
    authority: 'https://localhost:7132',
    client_id: 'bwa-client',
    redirect_uri: 'https://localhost:3000/callback',
    scope: 'openid profile email roles api'
});
```

**Quando usar:**
- ✅ Blazor WebAssembly (já está usando)
- ✅ React, Angular, Vue
- ✅ Qualquer SPA

### 3. Client Credentials (M2M - Machine to Machine)
**Cliente:** Precisa criar novo cliente (veja abaixo)

```bash
POST /connect/token
grant_type=client_credentials
client_id=meu-servico
client_secret=super-secret
scope=api
```

**Quando usar:**
- ✅ APIs backend que consomem sua API
- ✅ Jobs/Workers
- ✅ Integrações com sistemas externos (PowerBI, etc.)

**Como criar cliente M2M:**

Adicione em `OpenIddictWorker.cs`:

```csharp
private async Task CreateM2MClientAsync(IOpenIddictApplicationManager manager, CancellationToken cancellationToken)
{
    var client = await manager.FindByClientIdAsync("powerbi-service", cancellationToken);
    if (client is null)
    {
        var descriptor = new OpenIddictApplicationDescriptor
        {
            ClientId = "powerbi-service",
            ClientSecret = "seu-secret-aqui", // Guardar em Azure KeyVault / AWS Secrets Manager
            DisplayName = "PowerBI Integration",
            ClientType = OpenIddictConstants.ClientTypes.Confidential,
            Permissions =
            {
                OpenIddictConstants.Permissions.Endpoints.Token,
                OpenIddictConstants.Permissions.GrantTypes.ClientCredentials,
                OpenIddictConstants.Permissions.Prefixes.Scope + "api"
            }
        };

        await manager.CreateAsync(descriptor, cancellationToken);
    }
}

// No método StartAsync, adicione:
await CreateM2MClientAsync(manager, cancellationToken);
```

### 4. Refresh Token (Renovar tokens expirados)

```bash
POST /connect/token
grant_type=refresh_token
refresh_token={REFRESH_TOKEN}
client_id=mobile-client
```

**Quando usar:**
- ✅ Renovar access token quando expirar
- ✅ Evitar fazer login novamente
- ✅ Melhorar UX (usuário permanece logado)

---

## 📊 Comparação: JWT Customizado vs OpenIddict

| Aspecto | JWT Customizado ❌ | OpenIddict ✅ |
|---------|-------------------|--------------|
| **Funciona na API atual** | ❌ Não | ✅ Sim |
| **Endpoint** | `/v1/auth/login` | `/connect/token` |
| **Padrão da Indústria** | ❌ Implementação manual | ✅ OAuth 2.0 / OIDC |
| **Suporte a Mobile** | ⚠️ Apenas username/password | ✅ Password Grant |
| **Suporte a SPAs** | ❌ Não | ✅ Authorization Code + PKCE |
| **Suporte a M2M** | ❌ Não | ✅ Client Credentials |
| **Refresh Token** | ❌ Não implementado | ✅ Nativo |
| **Revogação de Token** | ❌ Não | ✅ `/connect/revoke` |
| **Introspection** | ❌ Não | ✅ `/connect/introspect` |
| **Segurança** | ⚠️ HMAC-SHA256 (chave compartilhada) | ✅ RSA-SHA256 (certificados) |
| **Múltiplos Clientes** | ❌ Não suporta | ✅ Ilimitados clientes |
| **Compatibilidade** | ⚠️ Apenas este projeto | ✅ Qualquer cliente OAuth/OIDC |
| **Manutenção** | ⚠️ Código customizado | ✅ Biblioteca mantida |

---

## 🎯 Recomendação

### Para Resolver Agora
Use um dos scripts de teste para obter um token válido do OpenIddict:
```bash
./test-openiddict-login.sh
```

### Para Produção
**Migrar 100% para OpenIddict** (Opção 2):

**Vantagens:**
1. ✅ Já está configurado e funcionando
2. ✅ Suporta todos os cenários (mobile, web, M2M)
3. ✅ Padrão da indústria (OAuth 2.0 / OIDC)
4. ✅ Mais seguro (RSA ao invés de HMAC)
5. ✅ Menos código para manter
6. ✅ Melhor interoperabilidade

**O que remover:**
```csharp
// DELETAR estes arquivos (não são mais necessários):
- GetTokenCommand.cs
- GetTokenCommandHandler.cs
- LoginUserEndpoint.cs (endpoint /v1/auth/login)
```

**O que manter:**
- ✅ `AuthorizationController.cs` (endpoints OpenIddict)
- ✅ `OpenIddictWorker.cs` (configuração de clientes)
- ✅ `IdentityConfig.cs` (validação OpenIddict)

---

## 📝 Exemplos de Integração

### C# / .NET
Ver arquivo: [`exemplos-autenticacao-openiddict.md`](./exemplos-autenticacao-openiddict.md) (seção C#)

### Python
Ver arquivo: [`test-openiddict-login.py`](./test-openiddict-login.py)

### JavaScript/TypeScript
Ver arquivo: [`exemplos-autenticacao-openiddict.md`](./exemplos-autenticacao-openiddict.md) (seção JavaScript)

### PowerShell
Ver arquivo: [`test-openiddict-login.ps1`](./test-openiddict-login.ps1)

---

## 🔧 Testando Agora

1. **Certifique-se que os 2 servidores estão rodando:**
   ```bash
   # Terminal 1 - Auth Server (OpenIddict)
   cd src/EChamado/Echamado.Auth
   dotnet run

   # Terminal 2 - API Server
   cd src/EChamado/Server/EChamado.Server
   dotnet run
   ```

2. **Execute um dos scripts de teste:**
   ```bash
   # Bash
   ./test-openiddict-login.sh

   # PowerShell
   .\test-openiddict-login.ps1

   # Python
   python test-openiddict-login.py
   ```

3. **Veja o token válido sendo usado na API!** ✅

---

## 🆘 Troubleshooting

### Erro: "Failed to fetch" ou "CORS"
**Causa:** Auth Server (porta 7132) não está rodando

**Solução:**
```bash
cd src/EChamado/Echamado.Auth
dotnet run
```

### Erro: "Invalid username/password"
**Causa:** Credenciais incorretas ou usuário não existe

**Solução:** Verificar usuários seeded no banco:
- `admin@admin.com` / `Admin@123`
- `user@echamado.com` / `User@123`

### Erro: "invalid_client"
**Causa:** Cliente não está configurado

**Solução:** Verificar se `OpenIddictWorker.cs` está criando os clientes corretamente

### Token funciona mas API retorna 401
**Causa:** API não está validando tokens OpenIddict

**Solução:** Verificar `IdentityConfig.cs:184`:
```csharp
options.SetIssuer(new Uri("https://localhost:7132"));
```

---

## 📚 Documentação Adicional

- **Guia Completo:** [`exemplos-autenticacao-openiddict.md`](./exemplos-autenticacao-openiddict.md)
- **OpenIddict:** https://documentation.openiddict.com/
- **OAuth 2.0:** https://oauth.net/2/
- **OpenID Connect:** https://openid.net/connect/
