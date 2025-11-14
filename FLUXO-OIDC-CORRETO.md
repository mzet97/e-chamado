# Fluxo OIDC Authorization Code + PKCE - Análise Completa

## Data: 2025-11-12
## Problema: Redirecionamento para 7296 ao invés de 7274

---

## 🎯 Fluxo Esperado (CORRETO)

### 1️⃣ **Cliente inicia o login** (7274)
```
Usuário em: https://localhost:7274
Clica em "Login" → RemoteAuthenticatorView inicia fluxo OIDC

Request:
GET https://localhost:7296/connect/authorize?
  client_id=bwa-client
  &redirect_uri=https%3A%2F%2Flocalhost%3A7274%2Fauthentication%2Flogin-callback
  &response_type=code
  &scope=openid%20profile%20email%20api%20chamados
  &state=abc123...
  &code_challenge=xyz789...
  &code_challenge_method=S256
  &response_mode=query
```

**Parâmetros importantes**:
- `redirect_uri`: Para onde o OpenIddict DEVE redirecionar após autenticação (`https://localhost:7274/authentication/login-callback`)
- `state`: Estado para validação CSRF
- `code_challenge`: Para PKCE (Proof Key for Code Exchange)

---

### 2️⃣ **Server OpenIddict recebe request** (7296)
```
AuthorizationController.Authorize()

Log esperado:
[INFO] Authorization request received.
  Client: bwa-client
  RedirectUri: https://localhost:7274/authentication/login-callback
  Scope: openid profile email api chamados
  ResponseType: code
  State: abc123...
  CodeChallenge: xyz789...
```

**Ação**: `AuthenticateAsync("External")` → FALHA (usuário não autenticado)

**Challenge**:
```csharp
return Challenge(
    authenticationSchemes: new[] { "External" },
    properties: new AuthenticationProperties
    {
        RedirectUri = "/connect/authorize?client_id=bwa-client&redirect_uri=...&state=...&code_challenge=..."
    });
```

---

### 3️⃣ **IdentityConfig redireciona para Auth Server** (7296 → 7132)
```
IdentityConfig.OnRedirectToLogin disparado

RedirectUri = "/connect/authorize?client_id=bwa-client&redirect_uri=https%3A%2F%2Flocalhost%3A7274%2F...&state=...&code_challenge=..."

Constrói URL de login:
loginUrl = "https://localhost:7132/Account/Login"
returnUrl = "https://localhost:7296/connect/authorize?client_id=bwa-client&redirect_uri=https%3A%2F%2Flocalhost%3A7274%2F...&state=...&code_challenge=..."
encodedReturnUrl = Uri.EscapeDataString(returnUrl)
finalUrl = loginUrl + "?returnUrl=" + encodedReturnUrl

Log esperado:
[INFO] OnRedirectToLogin:
  Original RedirectUri=/connect/authorize?client_id=bwa-client&redirect_uri=https%3A%2F%2Flocalhost%3A7274%2F...
  Final URL=https://localhost:7132/Account/Login?returnUrl=https%3A%2F%2Flocalhost%3A7296%2Fconnect%2Fauthorize%3Fclient_id%3D...

Response.Redirect(finalUrl)
```

---

### 4️⃣ **Auth Server exibe form de login** (7132)
```
GET https://localhost:7132/Account/Login?returnUrl=https%3A%2F%2Flocalhost%3A7296%2Fconnect%2Fauthorize%3Fclient_id%3D...

Login.razor carrega com:
- ReturnUrl = decodifica query param → "https://localhost:7296/connect/authorize?client_id=..."
- Form exibe campos Email e Password
```

---

### 5️⃣ **Usuário submete credenciais** (7132)
```
POST https://localhost:7132/Account/DoLogin
  email=admin@echamado.com
  password=Admin@123
  returnUrl=https://localhost:7296/connect/authorize?client_id=bwa-client&redirect_uri=https%3A%2F%2Flocalhost%3A7274%2F...

AccountController.DoLogin():

Logs esperados:
[INFO] Login attempt for admin@echamado.com with returnUrl: https://localhost:7296/connect/authorize?client_id=bwa-client&redirect_uri=https%3A%2F%2Flocalhost%3A7274%2F...

[INFO] User admin@echamado.com authenticated successfully

[INFO] Decoded returnUrl: https://localhost:7296/connect/authorize?client_id=bwa-client&redirect_uri=https://localhost:7274/authentication/login-callback&state=...&code_challenge=...

[INFO] ✅ Redirecting to VALID returnUrl: https://localhost:7296/connect/authorize?client_id=bwa-client&redirect_uri=https://localhost:7274/authentication/login-callback&state=...

[INFO] ✅ URL contains OIDC parameters (client_id, redirect_uri)

SignInAsync("External", principal) → Cria cookie External
Redirect(target) → https://localhost:7296/connect/authorize?client_id=...&redirect_uri=https://localhost:7274/authentication/login-callback&...
```

**CRÍTICO**: A URL de redirecionamento DEVE conter TODOS os query parameters originais:
- ✅ `client_id=bwa-client`
- ✅ `redirect_uri=https://localhost:7274/authentication/login-callback` (URL COMPLETA do cliente!)
- ✅ `state=abc123...`
- ✅ `code_challenge=xyz789...`
- ✅ `code_challenge_method=S256`
- ✅ `response_type=code`
- ✅ `scope=openid profile email api chamados`

---

### 6️⃣ **Server OpenIddict recebe usuário autenticado** (7296)
```
GET https://localhost:7296/connect/authorize?client_id=bwa-client&redirect_uri=https://localhost:7274/authentication/login-callback&state=...&code_challenge=...

AuthorizationController.Authorize():

Logs esperados:
[INFO] Authorization request received.
  Client: bwa-client
  RedirectUri: https://localhost:7274/authentication/login-callback  <--- URL DO CLIENTE!
  Scope: openid profile email api chamados
  ResponseType: code
  State: abc123...
  CodeChallenge: xyz789...

AuthenticateAsync("External") → SUCESSO (cookie External presente!)

[INFO] User authenticated via External cookie. UserId: {guid}

Cria ClaimsPrincipal com claims do usuário
SetScopes(request.GetScopes())

[INFO] Generating authorization code. Will redirect to: https://localhost:7274/authentication/login-callback

SignIn(claimsPrincipal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)
```

**O que o SignIn() faz internamente**:
1. Gera authorization code (código de autorização)
2. Armazena no banco/cache com lifetime curto (10 min)
3. **Redireciona automaticamente** para `request.RedirectUri` + `?code={authorization_code}&state={state}`

**Resposta HTTP**:
```
HTTP/1.1 302 Found
Location: https://localhost:7274/authentication/login-callback?code=abc123def456...&state=abc123...
```

---

### 7️⃣ **Cliente recebe authorization code** (7274)
```
GET https://localhost:7274/authentication/login-callback?code=abc123def456...&state=abc123...

RemoteAuthenticatorView.razor (action="login-callback"):
1. Valida state (CSRF protection)
2. Troca authorization code por access_token:

POST https://localhost:7296/connect/token
  grant_type=authorization_code
  code=abc123def456...
  redirect_uri=https://localhost:7274/authentication/login-callback
  client_id=bwa-client
  code_verifier=original_verifier (PKCE)

Resposta:
{
  "access_token": "eyJhbGc...",
  "id_token": "eyJhbGc...",
  "refresh_token": "def456...",
  "token_type": "Bearer",
  "expires_in": 3600
}

3. Armazena tokens no localStorage/sessionStorage
4. Redireciona para página original ou "/"
```

**✅ USUÁRIO AUTENTICADO COM SUCESSO!**

---

## 🐛 Problema Atual

**Sintoma**: Após login, o navegador fica em `https://localhost:7296/` ao invés de voltar para `https://localhost:7274`

**Possíveis Causas**:

### Hipótese 1: ReturnUrl não contém todos os query params
```
❌ returnUrl = "https://localhost:7296/connect/authorize"  (SEM query params!)
✅ returnUrl = "https://localhost:7296/connect/authorize?client_id=bwa-client&redirect_uri=https://localhost:7274/..."
```

**Como verificar**:
- Olhar log: `[INFO] Decoded returnUrl: {DecodedReturnUrl}`
- Deve conter `client_id` e `redirect_uri`

### Hipótese 2: OpenIddict não está redirecionando
```
AuthorizationController.SignIn() executa, mas não redireciona para request.RedirectUri
```

**Como verificar**:
- Olhar log: `[INFO] Generating authorization code. Will redirect to: {RedirectUri}`
- Deve ser `https://localhost:7274/authentication/login-callback`

### Hipótese 3: Cookie External não está sendo compartilhado
```
Após login em 7132, cookie External não é visível em 7296
→ AuthenticateAsync("External") falha novamente
→ Loop infinito de redirects
```

**Como verificar**:
- DevTools → Application → Cookies → localhost:7296
- Deve existir cookie `EChamado.External`

---

## 🧪 Teste Passo a Passo

### 1. Abrir DevTools
- F12 → Network tab
- Habilitar "Preserve log"
- Filtrar: All

### 2. Iniciar fluxo de login
```
Acessar: https://localhost:7274
Clicar em "Login"
```

### 3. Verificar request inicial
```
Procurar em Network:
GET /connect/authorize?client_id=bwa-client&redirect_uri=https%3A%2F%2Flocalhost%3A7274%2Fauthentication%2Flogin-callback&...

Verificar query params:
✅ client_id: bwa-client
✅ redirect_uri: https://localhost:7274/authentication/login-callback
✅ response_type: code
✅ state: ...
✅ code_challenge: ...
```

### 4. Verificar redirect para Auth Server
```
Procurar em Network:
GET /Account/Login?returnUrl=https%3A%2F%2Flocalhost%3A7296%2Fconnect%2Fauthorize%3Fclient_id%3D...

Decodificar returnUrl (DevTools Console):
> decodeURIComponent("returnUrl_value_aqui")

DEVE retornar:
"https://localhost:7296/connect/authorize?client_id=bwa-client&redirect_uri=https://localhost:7274/authentication/login-callback&..."
```

### 5. Fazer login
```
Email: admin@echamado.com
Password: Admin@123
Submit
```

### 6. Verificar logs do Auth Server (Terminal 1)
```
[INFO] Login attempt for admin@echamado.com with returnUrl: ...
[INFO] Decoded returnUrl: https://localhost:7296/connect/authorize?client_id=...&redirect_uri=https://localhost:7274/...
[INFO] ✅ Redirecting to VALID returnUrl: https://localhost:7296/connect/authorize?...
[INFO] ✅ URL contains OIDC parameters (client_id, redirect_uri)
```

### 7. Verificar logs do API Server (Terminal 2)
```
[INFO] Authorization request received.
  Client: bwa-client
  RedirectUri: https://localhost:7274/authentication/login-callback
  ...
[INFO] User authenticated via External cookie. UserId: {guid}
[INFO] Generating authorization code. Will redirect to: https://localhost:7274/authentication/login-callback
```

### 8. Verificar redirect final no Network
```
Procurar em Network:
GET /authentication/login-callback?code=...&state=...

Status: 200
URL: https://localhost:7274/authentication/login-callback?code=abc123...&state=...
```

---

## 🔧 Correção (se necessário)

### Se Hipótese 1 for verdadeira:

O problema está na construção do `RedirectUri` no `IdentityConfig.OnRedirectToLogin`.

**Verificar**:
```csharp
var redirectUri = Request.PathBase + Request.Path +
                  QueryString.Create(Request.HasFormContentType
                      ? Request.Form.ToList()
                      : Request.Query.ToList());
```

**Deve incluir todos os query params!**

### Se Hipótese 2 for verdadeira:

O problema está no `AuthorizationController.SignIn()`.

**Verificar** se `request.RedirectUri` está correto antes do SignIn().

### Se Hipótese 3 for verdadeira:

O problema está na configuração do cookie External.

**Verificar** `SameSite`, `Domain`, `Path` no `IdentityConfig.cs`.

---

## ✅ Status

- [ ] Teste realizado
- [ ] Logs capturados
- [ ] Hipótese identificada
- [ ] Correção aplicada
- [ ] Fluxo funcionando end-to-end

---

**Autor**: Claude Code (Anthropic)
**Data**: 2025-11-12
