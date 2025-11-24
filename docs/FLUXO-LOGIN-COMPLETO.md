# Fluxo de Login - Sistema EChamado

## 📋 Índice

1. [Visão Geral da Arquitetura](#visão-geral-da-arquitetura)
2. [Servidores e Portas](#servidores-e-portas)
3. [Grant Types Suportados](#grant-types-suportados)
4. [Fluxo 1: Authorization Code + PKCE (Blazor WASM)](#fluxo-1-authorization-code--pkce-blazor-wasm)
5. [Fluxo 2: Password Grant (Mobile/CLI)](#fluxo-2-password-grant-mobilecli)
6. [Fluxo 3: Client Credentials (M2M)](#fluxo-3-client-credentials-m2m)
7. [Fluxo 4: Refresh Token](#fluxo-4-refresh-token)
8. [Validação de Token na API](#validação-de-token-na-api)
9. [Configurações de Segurança](#configurações-de-segurança)
10. [Troubleshooting](#troubleshooting)

---

## Visão Geral da Arquitetura

O sistema EChamado usa **arquitetura separada** para autenticação:

```
┌──────────────────────┐      ┌──────────────────────┐      ┌──────────────────────┐
│                      │      │                      │      │                      │
│  EChamado.Client     │◄────►│  Echamado.Auth       │      │  EChamado.Server     │
│  (Blazor WASM)       │      │  (Auth Server)       │      │  (Resource Server)   │
│                      │      │                      │      │                      │
│  Porta: 7274         │      │  Porta: 7132         │      │  Porta: 7296         │
│                      │      │                      │      │                      │
│  - UI do usuário     │      │  - Emite tokens      │      │  - Valida tokens     │
│  - Login redirect    │      │  - Gerencia usuários │      │  - API REST          │
│  - Armazena tokens   │      │  - OAuth 2.0 / OIDC  │      │  - Endpoints /v1/... │
│                      │      │                      │      │                      │
└──────────────────────┘      └──────────────────────┘      └──────────────────────┘
         │                              │                              │
         └──────────────────────────────┴──────────────────────────────┘
                                        │
                              ┌─────────▼─────────┐
                              │                   │
                              │  PostgreSQL DB    │
                              │  Porta: 5432      │
                              │                   │
                              │  - Usuários       │
                              │  - Roles          │
                              │  - Tokens         │
                              │  - Aplicações     │
                              │                   │
                              └───────────────────┘
```

### Princípios da Arquitetura

1. **Separação de Responsabilidades:**
   - **Echamado.Auth** = Authorization Server (emite tokens)
   - **EChamado.Server** = Resource Server (valida tokens e serve dados)
   - **EChamado.Client** = Client Application (consome API)

2. **Protocolo OAuth 2.0 / OpenID Connect:**
   - Implementado via **OpenIddict 7.x**
   - Suporta múltiplos grant types
   - Tokens JWT encriptados (JWE)

3. **Segurança:**
   - HTTPS obrigatório
   - PKCE para SPAs
   - Token encryption (RSA-OAEP + A256CBC-HS512)
   - Refresh tokens com rotação

---

## Servidores e Portas

### 1. Echamado.Auth (Authorization Server)

**Porta:** 7132 (HTTPS) / 5136 (HTTP)
**Função:** Emitir e gerenciar tokens de autenticação
**Endpoints principais:**

| Endpoint | Método | Descrição |
|----------|--------|-----------|
| `/connect/authorize` | GET | Inicia Authorization Code Flow |
| `/connect/token` | POST | Emite tokens (todos os grant types) |
| `/connect/userinfo` | GET | Retorna informações do usuário |
| `/.well-known/openid-configuration` | GET | Configuração OIDC (discovery) |
| `/Account/Login` | GET/POST | Página de login Blazor Server |
| `/Account/Register` | GET/POST | Página de registro |

**Tecnologia:** Blazor Server + ASP.NET Core Identity + OpenIddict Server

### 2. EChamado.Server (Resource Server / API)

**Porta:** 7296 (HTTPS) / 5125 (HTTP)
**Função:** Validar tokens e servir dados via API REST
**Endpoints principais:**

| Endpoint | Método | Descrição |
|----------|--------|-----------|
| `/v1/category` | GET/POST | CRUD de categorias |
| `/v1/order` | GET/POST/PUT | CRUD de chamados |
| `/v1/department` | GET/POST/PUT | CRUD de departamentos |
| `/health` | GET | Health check |

**Tecnologia:** ASP.NET Core Minimal API + OpenIddict Validation

### 3. EChamado.Client (Client Application)

**Porta:** 7274 (HTTPS) / 5182 (HTTP)
**Função:** Interface de usuário (SPA)
**Tecnologia:** Blazor WebAssembly + MudBlazor

---

## Grant Types Suportados

O sistema suporta **4 grant types** diferentes:

| Grant Type | Cliente | Uso Principal | Segurança |
|------------|---------|---------------|-----------|
| **Authorization Code + PKCE** | `bwa-client` | SPAs (Blazor WASM) | ⭐⭐⭐⭐⭐ Máxima |
| **Password Grant** | `mobile-client` | Mobile apps, CLI tools | ⭐⭐⭐ Média |
| **Client Credentials** | (custom) | APIs M2M (Machine-to-Machine) | ⭐⭐⭐⭐ Alta |
| **Refresh Token** | Todos | Renovar tokens expirados | ⭐⭐⭐⭐ Alta |

---

## Fluxo 1: Authorization Code + PKCE (Blazor WASM)

**Usado por:** EChamado.Client (Blazor WebAssembly)
**Cliente:** `bwa-client`
**Segurança:** Máxima (recomendado para SPAs públicas)

### Diagrama do Fluxo

```
┌─────────────┐                 ┌──────────────┐                 ┌─────────────┐
│             │                 │              │                 │             │
│   Browser   │                 │ Echamado.Auth│                 │ EChamado    │
│  (Client)   │                 │ (Auth Server)│                 │   .Server   │
│             │                 │              │                 │  (API)      │
└──────┬──────┘                 └──────┬───────┘                 └──────┬──────┘
       │                               │                                │
       │ 1. Acessa /orders             │                                │
       │    (não autenticado)          │                                │
       ├──────────────────────────────►│                                │
       │                               │                                │
       │ 2. Redirect para login        │                                │
       │    com PKCE challenge         │                                │
       │◄──────────────────────────────┤                                │
       │                               │                                │
       │ 3. GET /connect/authorize     │                                │
       │    + code_challenge           │                                │
       ├──────────────────────────────►│                                │
       │                               │                                │
       │ 4. Mostra /Account/Login      │                                │
       │◄──────────────────────────────┤                                │
       │                               │                                │
       │ 5. POST /Account/Login        │                                │
       │    (username + password)      │                                │
       ├──────────────────────────────►│                                │
       │                               │                                │
       │                        6. Valida credenciais                   │
       │                           (ASP.NET Identity)                   │
       │                               │                                │
       │ 7. Redirect com code          │                                │
       │    /authentication/           │                                │
       │    login-callback?code=ABC    │                                │
       │◄──────────────────────────────┤                                │
       │                               │                                │
       │ 8. POST /connect/token        │                                │
       │    grant_type=authorization_  │                                │
       │    code + code_verifier       │                                │
       ├──────────────────────────────►│                                │
       │                               │                                │
       │                        9. Valida code_verifier                 │
       │                           contra code_challenge                │
       │                               │                                │
       │ 10. Retorna tokens            │                                │
       │     {access_token,            │                                │
       │      refresh_token,           │                                │
       │      id_token}                │                                │
       │◄──────────────────────────────┤                                │
       │                               │                                │
       │ 11. Armazena tokens           │                                │
       │     no sessionStorage         │                                │
       │                               │                                │
       │ 12. GET /v1/orders            │                                │
       │     Authorization: Bearer     │                                │
       ├───────────────────────────────┼───────────────────────────────►│
       │                               │                                │
       │                               │    13. Valida token            │
       │                               │        (OpenIddict Validation) │
       │                               │◄───────────────────────────────┤
       │                               │                                │
       │                               │    14. Token válido            │
       │                               │────────────────────────────────►│
       │                               │                                │
       │ 15. Retorna dados             │                                │
       │     200 OK + JSON             │                                │
       │◄──────────────────────────────┼────────────────────────────────┤
       │                               │                                │
```

### Passo a Passo Detalhado

#### 1. Usuário Acessa Página Protegida

```
GET https://localhost:7274/orders
```

O Blazor WASM detecta que o usuário não está autenticado.

#### 2. Redirect Automático para Login

O `RemoteAuthenticationService` redireciona automaticamente para:

```
GET https://localhost:7132/connect/authorize?
  response_type=code&
  client_id=bwa-client&
  redirect_uri=https://localhost:7274/authentication/login-callback&
  scope=openid profile email roles api chamados&
  code_challenge=SHA256(code_verifier)&
  code_challenge_method=S256&
  state=ABC123&
  nonce=XYZ789
```

**Parâmetros importantes:**
- `code_challenge`: Hash do `code_verifier` (PKCE)
- `code_challenge_method`: `S256` (SHA-256)
- `state`: Token anti-CSRF
- `nonce`: Token anti-replay

#### 3. Exibição da Página de Login

O Echamado.Auth mostra a página `/Account/Login` (Blazor Server).

#### 4. Usuário Preenche Credenciais

```html
<form>
  <input name="username" value="admin@admin.com" />
  <input name="password" type="password" value="Admin@123" />
  <button type="submit">Login</button>
</form>
```

#### 5. Validação de Credenciais

O servidor valida via **ASP.NET Core Identity**:

```csharp
var result = await _signInManager.PasswordSignInAsync(
    model.Username,
    model.Password,
    isPersistent: false,
    lockoutOnFailure: false
);

if (result.Succeeded)
{
    // Usuário autenticado
}
```

#### 6. Redirect com Authorization Code

Se credenciais válidas, redireciona de volta para o client com um **authorization code**:

```
HTTP/1.1 302 Found
Location: https://localhost:7274/authentication/login-callback?code=ABC123DEF456&state=ABC123
```

#### 7. Troca do Code por Tokens

O Blazor WASM faz um POST para trocar o code por tokens:

```http
POST https://localhost:7132/connect/token
Content-Type: application/x-www-form-urlencoded

grant_type=authorization_code&
code=ABC123DEF456&
redirect_uri=https://localhost:7274/authentication/login-callback&
client_id=bwa-client&
code_verifier=ORIGINAL_CODE_VERIFIER
```

**Validação PKCE:**
```
SHA256(code_verifier) == code_challenge armazenado
```

#### 8. Resposta com Tokens

```json
{
  "access_token": "eyJhbGciOiJSU0EtT0FFUCIsImVuYyI6IkEyNTZDQkMtSFM1MTIi...",
  "token_type": "Bearer",
  "expires_in": 3600,
  "refresh_token": "eyJhbGciOiJkaXIiLCJlbmMiOiJBMjU2Q0JDLUhTNTEyIiwia...",
  "id_token": "eyJhbGciOiJSUzI1NiIsImtpZCI6IkM5MTQ2OEIxQTRCMTM2NEU..."
}
```

**Tipos de tokens:**
- `access_token`: Para acessar API (JWE encriptado)
- `id_token`: Informações do usuário (JWT assinado)
- `refresh_token`: Para renovar tokens (JWE encriptado)

#### 9. Armazenamento de Tokens

O Blazor WASM armazena tokens no `sessionStorage`:

```javascript
sessionStorage.setItem('oidc.user', JSON.stringify({
  access_token: '...',
  refresh_token: '...',
  id_token: '...',
  expires_at: 1700000000
}));
```

#### 10. Requisição à API com Token

```http
GET https://localhost:7296/v1/orders
Authorization: Bearer eyJhbGciOiJSU0EtT0FFUCIsImVuYyI6IkEyNTZDQkMtSFM1MTIi...
```

O `BaseAddressAuthorizationMessageHandler` adiciona o token automaticamente.

#### 11. Validação do Token pela API

O EChamado.Server valida o token via **OpenIddict Validation**:

```csharp
// Configuração em IdentityConfig.cs
.AddValidation(options =>
{
    options.SetIssuer(new Uri("https://localhost:7132"));
    options.UseSystemNetHttp();
    options.UseAspNetCore();
});
```

**Validações realizadas:**
1. Token não expirou (`exp` claim)
2. Issuer correto (`iss` == `https://localhost:7132`)
3. Assinatura válida (chave pública)
4. Audience correto (`aud`)
5. Scopes necessários presentes

#### 12. Resposta da API

```json
HTTP/1.1 200 OK
Content-Type: application/json

{
  "data": [
    {
      "id": "123",
      "title": "Problema no sistema",
      "status": "Open"
    }
  ],
  "success": true
}
```

---

## Fluxo 2: Password Grant (Mobile/CLI)

**Usado por:** Mobile apps, aplicações desktop, scripts, ferramentas CLI
**Cliente:** `mobile-client`
**Segurança:** Média (apenas para clientes confiáveis)

### Diagrama do Fluxo

```
┌──────────────┐              ┌──────────────┐              ┌──────────────┐
│              │              │              │              │              │
│  Mobile App  │              │ Echamado.Auth│              │  EChamado    │
│   / CLI      │              │ (Auth Server)│              │   .Server    │
│              │              │              │              │              │
└──────┬───────┘              └──────┬───────┘              └──────┬───────┘
       │                             │                             │
       │ 1. POST /connect/token      │                             │
       │    grant_type=password      │                             │
       │    username=admin@admin.com │                             │
       │    password=Admin@123       │                             │
       │    client_id=mobile-client  │                             │
       ├────────────────────────────►│                             │
       │                             │                             │
       │                      2. Valida credenciais                │
       │                         (ASP.NET Identity)                │
       │                             │                             │
       │ 3. Retorna tokens           │                             │
       │    {access_token,           │                             │
       │     refresh_token}          │                             │
       │◄────────────────────────────┤                             │
       │                             │                             │
       │ 4. GET/POST /v1/...         │                             │
       │    Authorization: Bearer    │                             │
       ├─────────────────────────────┼────────────────────────────►│
       │                             │                             │
       │                             │   5. Valida token           │
       │                             │◄────────────────────────────┤
       │                             │                             │
       │                             │   6. Token válido           │
       │                             │─────────────────────────────►│
       │                             │                             │
       │ 7. Retorna dados            │                             │
       │    200 OK + JSON            │                             │
       │◄────────────────────────────┼─────────────────────────────┤
       │                             │                             │
```

### Exemplo de Requisição

```bash
curl -k -X POST https://localhost:7132/connect/token \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=password" \
  -d "username=admin@admin.com" \
  -d "password=Admin@123" \
  -d "client_id=mobile-client" \
  -d "scope=openid profile email roles api chamados"
```

### Resposta

```json
{
  "access_token": "eyJhbGciOiJSU0EtT0FFUCIsImVuYyI6IkEyNTZDQkMtSFM1MTIi...",
  "token_type": "Bearer",
  "expires_in": 3600,
  "refresh_token": "eyJhbGciOiJkaXIiLCJlbmMiOiJBMjU2Q0JDLUhTNTEyIiwia...",
  "id_token": "eyJhbGciOiJSUzI1NiIsImtpZCI6IkM5MTQ2OEIxQTRCMTM2NEU..."
}
```

### Uso do Token

```bash
curl -k -X GET https://localhost:7296/v1/orders \
  -H "Authorization: Bearer eyJhbGciOiJSU0EtT0FFUCIsImVuYyI6IkEyNTZDQkMtSFM1MTIi..."
```

### ⚠️ Considerações de Segurança

**Quando usar:**
- ✅ Aplicações móveis nativas
- ✅ Aplicações desktop
- ✅ Scripts de automação internos
- ✅ Ferramentas CLI

**Quando NÃO usar:**
- ❌ Single Page Applications (use Authorization Code + PKCE)
- ❌ Aplicações de terceiros
- ❌ Qualquer aplicação onde as credenciais possam ser expostas

**Mitigações:**
- Exigir HTTPS sempre
- Usar refresh tokens com rotação
- Implementar rate limiting
- Registrar todas as tentativas de login
- Usar certificado pinning em mobile apps

---

## Fluxo 3: Client Credentials (M2M)

**Usado por:** APIs, serviços backend, jobs agendados
**Segurança:** Alta (não envolve usuário)

### Diagrama do Fluxo

```
┌──────────────┐              ┌──────────────┐              ┌──────────────┐
│              │              │              │              │              │
│  Backend     │              │ Echamado.Auth│              │  EChamado    │
│  Service     │              │ (Auth Server)│              │   .Server    │
│              │              │              │              │              │
└──────┬───────┘              └──────┬───────┘              └──────┬───────┘
       │                             │                             │
       │ 1. POST /connect/token      │                             │
       │    grant_type=client_       │                             │
       │    credentials              │                             │
       │    client_id=backend-api    │                             │
       │    client_secret=SECRET123  │                             │
       ├────────────────────────────►│                             │
       │                             │                             │
       │                      2. Valida client_id +                │
       │                         client_secret                     │
       │                             │                             │
       │ 3. Retorna access_token     │                             │
       │    (sem refresh_token)      │                             │
       │◄────────────────────────────┤                             │
       │                             │                             │
       │ 4. API request              │                             │
       │    Authorization: Bearer    │                             │
       ├─────────────────────────────┼────────────────────────────►│
       │                             │                             │
       │ 5. Retorna dados            │                             │
       │◄────────────────────────────┼─────────────────────────────┤
       │                             │                             │
```

### Exemplo (quando implementado)

```bash
curl -k -X POST https://localhost:7132/connect/token \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=client_credentials" \
  -d "client_id=backend-api" \
  -d "client_secret=YOUR_SECRET_HERE" \
  -d "scope=api chamados"
```

### Configuração de Cliente

```csharp
// Em OpenIddictWorker.cs
await manager.CreateAsync(new OpenIddictApplicationDescriptor
{
    ClientId = "backend-api",
    ClientSecret = "SECRET_FROM_APPSETTINGS",
    DisplayName = "Backend API Service",
    Permissions =
    {
        Permissions.Endpoints.Token,
        Permissions.GrantTypes.ClientCredentials,
        Permissions.Scopes.Api,
        Permissions.Scopes.Roles
    }
});
```

---

## Fluxo 4: Refresh Token

**Usado por:** Renovar tokens expirados sem re-autenticação
**Disponível para:** Todos os clientes

### Diagrama do Fluxo

```
┌──────────────┐              ┌──────────────┐              ┌──────────────┐
│              │              │              │              │              │
│    Client    │              │ Echamado.Auth│              │  EChamado    │
│              │              │ (Auth Server)│              │   .Server    │
│              │              │              │              │              │
└──────┬───────┘              └──────┬───────┘              └──────┬───────┘
       │                             │                             │
       │ 1. GET /v1/orders           │                             │
       │    Authorization: Bearer    │                             │
       ├─────────────────────────────┼────────────────────────────►│
       │                             │                             │
       │                             │   2. Token expirado         │
       │                             │◄────────────────────────────┤
       │                             │                             │
       │ 3. 401 Unauthorized         │                             │
       │    (token expirou)          │                             │
       │◄────────────────────────────┼─────────────────────────────┤
       │                             │                             │
       │ 4. POST /connect/token      │                             │
       │    grant_type=refresh_token │                             │
       │    refresh_token=xyz...     │                             │
       │    client_id=bwa-client     │                             │
       ├────────────────────────────►│                             │
       │                             │                             │
       │                      5. Valida refresh_token               │
       │                         (não expirado, válido)            │
       │                             │                             │
       │ 6. Retorna novos tokens     │                             │
       │    {access_token,           │                             │
       │     refresh_token}          │                             │
       │◄────────────────────────────┤                             │
       │                             │                             │
       │ 7. GET /v1/orders           │                             │
       │    Authorization: Bearer    │                             │
       │    (novo access_token)      │                             │
       ├─────────────────────────────┼────────────────────────────►│
       │                             │                             │
       │ 8. 200 OK + dados           │                             │
       │◄────────────────────────────┼─────────────────────────────┤
       │                             │                             │
```

### Exemplo de Requisição

```bash
curl -k -X POST https://localhost:7132/connect/token \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=refresh_token" \
  -d "refresh_token=eyJhbGciOiJkaXIiLCJlbmMiOiJBMjU2Q0JDLUhTNTEyIiwia..." \
  -d "client_id=bwa-client"
```

### Resposta

```json
{
  "access_token": "eyJhbGciOiJSU0EtT0FFUCIsImVuYyI6IkEyNTZDQkMtSFM1MTIi... (NOVO)",
  "token_type": "Bearer",
  "expires_in": 3600,
  "refresh_token": "eyJhbGciOiJkaXIiLCJlbmMiOiJBMjU2Q0JDLUhTNTEyIiwia... (NOVO)"
}
```

### Refresh Token Rotation

O OpenIddict implementa **automatic refresh token rotation**:

1. Quando você usa um refresh token, ele é invalidado
2. Um novo refresh token é emitido junto com o novo access token
3. Isso previne replay attacks

---

## Validação de Token na API

### Configuração da Validação

**Arquivo:** `src/EChamado/Server/EChamado.Server.Infrastructure/Configuration/IdentityConfig.cs`

```csharp
services.AddAuthentication(options =>
{
    // Define que tokens Bearer são o esquema padrão
    options.DefaultAuthenticateScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
})

// Configuração da validação OpenIddict
services.AddOpenIddict()
    .AddValidation(options =>
    {
        // Issuer que emitiu o token (Auth Server)
        options.SetIssuer(new Uri("https://localhost:7132"));

        // Usar HTTP client para buscar chaves públicas
        options.UseSystemNetHttp();

        // Integração com ASP.NET Core
        options.UseAspNetCore();
    });
```

### Processo de Validação

Quando uma requisição chega na API com um Bearer token:

```http
GET /v1/orders
Authorization: Bearer eyJhbGciOiJSU0EtT0FFUCIsImVuYyI6IkEyNTZDQkMtSFM1MTIi...
```

**Etapas da validação:**

1. **Extração do Token**
   - OpenIddict extrai o token do header `Authorization`

2. **Descriptografia (JWE)**
   ```
   Token JWE → Chave privada do servidor → Token JWT
   ```

3. **Validação da Assinatura**
   ```
   JWT assinado → Chave pública do Auth Server → Válido/Inválido
   ```

4. **Validação de Claims**
   - `exp` (expiration): Token não expirou?
   - `iss` (issuer): `https://localhost:7132`?
   - `aud` (audience): Inclui este servidor?
   - `nbf` (not before): Já é válido?

5. **Validação de Scopes**
   - Token contém os scopes necessários para o endpoint?

6. **Criação do ClaimsPrincipal**
   - Se tudo válido, cria um `ClaimsPrincipal` com as claims do token
   - Disponível via `HttpContext.User`

### Retorno em Caso de Erro

**Token ausente ou inválido:**
```http
HTTP/1.1 401 Unauthorized
WWW-Authenticate: Bearer
Content-Type: application/json

{
  "error": "invalid_token",
  "error_description": "The access token is invalid"
}
```

**Token válido mas sem permissão:**
```http
HTTP/1.1 403 Forbidden
Content-Type: application/json

{
  "error": "insufficient_scope",
  "error_description": "The access token does not have sufficient scope"
}
```

---

## Configurações de Segurança

### 1. Configuração de Clientes

**Arquivo:** `src/EChamado/Echamado.Auth/OpenIddictWorker.cs`

#### Cliente Blazor WASM (`bwa-client`)

```csharp
await manager.CreateAsync(new OpenIddictApplicationDescriptor
{
    ClientId = "bwa-client",
    DisplayName = "Blazor WebAssembly Client",
    ConsentType = ConsentTypes.Implicit, // Sem tela de consentimento
    Type = ClientTypes.Public, // Sem client secret (SPA pública)

    // Authorization Code Flow + PKCE
    Permissions =
    {
        Permissions.Endpoints.Authorization,
        Permissions.Endpoints.Token,
        Permissions.Endpoints.Logout,
        Permissions.GrantTypes.AuthorizationCode,
        Permissions.GrantTypes.RefreshToken,
        Permissions.ResponseTypes.Code,

        // Scopes permitidos
        Permissions.Scopes.OpenId,
        Permissions.Scopes.Profile,
        Permissions.Scopes.Email,
        Permissions.Scopes.Roles,
        Permissions.Prefixes.Scope + "api",
        Permissions.Prefixes.Scope + "chamados"
    },

    // URLs de redirect permitidas
    RedirectUris =
    {
        new Uri("https://localhost:7274/authentication/login-callback")
    },

    PostLogoutRedirectUris =
    {
        new Uri("https://localhost:7274/authentication/logout-callback")
    },

    // PKCE obrigatório
    Requirements =
    {
        Requirements.Features.ProofKeyForCodeExchange
    }
});
```

#### Cliente Mobile (`mobile-client`)

```csharp
await manager.CreateAsync(new OpenIddictApplicationDescriptor
{
    ClientId = "mobile-client",
    DisplayName = "Mobile Client",
    Type = ClientTypes.Public, // Sem client secret (apps públicos)

    // Password Grant + Refresh Token
    Permissions =
    {
        Permissions.Endpoints.Token,
        Permissions.GrantTypes.Password,
        Permissions.GrantTypes.RefreshToken,

        // Scopes permitidos
        Permissions.Scopes.OpenId,
        Permissions.Scopes.Profile,
        Permissions.Scopes.Email,
        Permissions.Scopes.Roles,
        Permissions.Prefixes.Scope + "api",
        Permissions.Prefixes.Scope + "chamados"
    }
});
```

### 2. Configuração de Scopes

**Arquivo:** `src/EChamado/Echamado.Auth/Program.cs`

```csharp
builder.Services.AddOpenIddict()
    .AddServer(options =>
    {
        // Registrar scopes
        options.RegisterScopes(
            "openid",    // Identificação do usuário
            "profile",   // Nome, foto, etc
            "email",     // Email do usuário
            "roles",     // Roles/perfis
            "api",       // Acesso à API geral
            "chamados"   // Acesso específico a chamados
        );
    });
```

**Registro no banco:**

```csharp
// Em OpenIddictWorker.cs
await scopeManager.CreateAsync(new OpenIddictScopeDescriptor
{
    Name = "chamados",
    DisplayName = "Chamados Access",
    Description = "Allows full access to chamados (tickets)",
    Resources = { "echamado_api" }
});
```

### 3. Tempo de Expiração de Tokens

**Configuração padrão:**

| Token | Duração | Renovável |
|-------|---------|-----------|
| **Access Token** | 1 hora (3600s) | ✅ Via refresh token |
| **Refresh Token** | 14 dias | ✅ Com rotação |
| **ID Token** | 1 hora (3600s) | ❌ Não renovável |
| **Authorization Code** | 5 minutos | ❌ Uso único |

### 4. Criptografia de Tokens

**Chaves de desenvolvimento:**

```csharp
// Configuração no Program.cs do Echamado.Auth
.AddServer(options =>
{
    // Para desenvolvimento: chaves efêmeras
    options.AddDevelopmentEncryptionCertificate()
           .AddDevelopmentSigningCertificate();

    // Para produção: usar certificados reais
    // options.AddEncryptionCertificate(cert)
    //        .AddSigningCertificate(cert);
})
```

**Algoritmos usados:**
- **Assinatura:** RS256 (RSA SHA-256)
- **Encriptação:** RSA-OAEP + A256CBC-HS512

### 5. CORS

**Arquivo:** `src/EChamado/Server/EChamado.Server/Configuration/CorsConfig.cs`

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins(
            "https://localhost:7274",  // Blazor WASM
            "https://localhost:7132"   // Auth Server
        )
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });
});
```

---

## Troubleshooting

### ❌ Erro: invalid_scope

**Sintomas:**
```json
{
  "error": "invalid_scope",
  "error_description": "The specified 'scope' is invalid."
}
```

**Causas:**
1. Scope não registrado no `Program.cs` (RegisterScopes)
2. Scope não criado no banco (OpenIddictWorker)
3. Cliente não tem permissão para o scope

**Solução:**
- Verificar `RegisterScopes()` no Program.cs
- Verificar `RegisterCustomScopesAsync()` no OpenIddictWorker.cs
- Rebuildar o Echamado.Auth

---

### ❌ Erro: 401 Unauthorized com token válido

**Sintomas:**
```http
POST /v1/category
Authorization: Bearer eyJhbGci...
→ HTTP/1.1 401 Unauthorized
```

**Causas:**
1. EChamado.Server não foi reconstruído após mudanças no IdentityConfig.cs
2. Issuer não corresponde (`https://localhost:7132`)
3. Token expirou
4. Chaves de assinatura não correspondem

**Solução:**
```powershell
# Rebuild do servidor API
cd E:\TI\git\e-chamado\src\EChamado\Server\EChamado.Server
.\rebuild-windows.ps1
```

---

### ❌ Erro: API retorna HTML em vez de JSON

**Sintomas:**
```html
<!DOCTYPE html>
<html>... (página de login)
```

**Causa:**
`DefaultChallengeScheme` configurado como `"External"` (cookie).

**Solução:**
Verificar `IdentityConfig.cs` linha 104:

```csharp
// CORRETO:
options.DefaultChallengeScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;

// ERRADO:
options.DefaultChallengeScheme = "External";
```

---

### ❌ Erro: IDX10503 Signature validation failed

**Sintomas:**
```
IDX10503: Signature validation failed. Keys tried: 'RSA, KeyId: ...'
```

**Causas:**
1. Auth Server e API Server usando chaves diferentes
2. Chaves efêmeras foram regeneradas (restart do Auth Server)
3. Cache de chaves público desatualizado

**Solução:**
1. Reiniciar **ambos** os servidores (Auth e API)
2. Gerar novo token após restart
3. Para produção: usar certificados fixos (não development)

---

### ❌ Erro: Unable to resolve service IOpenIddictService

**Sintomas:**
```
System.InvalidOperationException: Unable to resolve service for type
'EChamado.Server.Domain.Services.Interface.IOpenIddictService'
```

**Causa:**
Falta registrar serviços de Application no Echamado.Auth.

**Solução:**
Verificar `Program.cs` do Echamado.Auth:

```csharp
builder.Services.AddApplicationServices();
builder.Services.ResolveDependenciesApplication();
```

---

### ✅ Checklist de Validação

Antes de reportar problemas, verifique:

- [ ] Ambos os servidores estão rodando (7132 e 7296)
- [ ] PostgreSQL está acessível (porta 5432)
- [ ] Token foi obtido há menos de 1 hora
- [ ] Token inclui todos os scopes necessários
- [ ] Header `Authorization: Bearer <token>` está correto
- [ ] HTTPS está sendo usado (não HTTP)
- [ ] IdentityConfig.cs usa OpenIddictValidation como DefaultChallengeScheme
- [ ] Ambos os servidores foram reconstruídos após mudanças

---

## 📚 Referências

- **CORRECAO-FINAL-AUTH.md** - Correção completa do Echamado.Auth
- **CORRECAO-API-REDIRECT-LOGIN.md** - Correção do redirect para 401
- **GUIA-RESOLVER-401-TOKEN.md** - Troubleshooting do 401
- **docs/ARQUITETURA-AUTENTICACAO.md** - Arquitetura detalhada
- **CLAUDE.md** - Guia geral do projeto
- [OpenIddict Documentation](https://documentation.openiddict.com/)
- [OAuth 2.0 RFC 6749](https://datatracker.ietf.org/doc/html/rfc6749)
- [OpenID Connect Core 1.0](https://openid.net/specs/openid-connect-core-1_0.html)

---

## 📝 Notas de Implementação

### Usuários Padrão (Seeded)

```
Admin:
  Username: admin@admin.com
  Password: Admin@123
  Roles: Admin, User

User:
  Username: user@echamado.com
  Password: User@123
  Roles: User
```

### Estrutura do Access Token (Claims)

```json
{
  "sub": "user-id-guid",
  "email": "admin@admin.com",
  "name": "Admin User",
  "role": ["Admin", "User"],
  "scope": ["openid", "profile", "email", "roles", "api", "chamados"],
  "iss": "https://localhost:7132/",
  "aud": "echamado_api",
  "exp": 1700000000,
  "iat": 1699996400,
  "nbf": 1699996400
}
```

### Endpoints OIDC Discovery

```
GET https://localhost:7132/.well-known/openid-configuration
```

Retorna configuração completa do servidor OIDC.

---

**Versão:** 1.0
**Data:** 23/11/2025
**Status:** ✅ Documentação Completa
