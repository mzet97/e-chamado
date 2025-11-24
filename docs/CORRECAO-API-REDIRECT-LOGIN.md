# Correção: API Retornando HTML de Login em Vez de JSON

## ❌ Problema

Ao fazer um POST para a API com Bearer token:

```bash
POST https://localhost:7296/v1/category
Authorization: Bearer eyJhbGci...
Content-Type: application/json

{"name": "Teste 1", "description": "Teste 1"}
```

**Recebe HTML de login** em vez de resposta JSON:
```html
<!DOCTYPE html>
<html lang="en">
<head><title>Login - EChamado</title>...
```

## 🔍 Causa Raiz

No arquivo `IdentityConfig.cs` (linha 104), o `DefaultChallengeScheme` estava configurado como `"External"` (cookie authentication):

```csharp
services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = "External"; // ❌ PROBLEMA AQUI
})
```

Quando a autenticação falha:
1. ❌ O esquema "External" (cookie) é chamado
2. ❌ Redireciona para `/Account/Login` (linha 113)
3. ❌ Retorna HTML da página de login
4. ❌ Em vez de retornar HTTP 401 Unauthorized

## ✅ Solução Aplicada

**Arquivo:** `src/EChamado/Server/EChamado.Server.Infrastructure/Configuration/IdentityConfig.cs`

**Mudança na linha 104:**

```csharp
services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme; // ✅ CORRETO
})
```

Agora quando a autenticação falha:
1. ✅ O OpenIddict Validation é chamado
2. ✅ Retorna HTTP 401 Unauthorized
3. ✅ Com header `WWW-Authenticate: Bearer`
4. ✅ Resposta JSON apropriada

## 🚀 Como Aplicar a Correção

### No Windows (Recomendado)

```powershell
# 1. PARE o EChamado.Server se estiver rodando
# Pressione Ctrl+C no terminal onde ele está rodando

# 2. Navegue até o diretório do projeto
cd E:\TI\git\e-chamado\src\EChamado\Server\EChamado.Server

# 3. Limpe e reconstrua
dotnet clean
dotnet build

# 4. Execute novamente
dotnet run
```

### No WSL

```bash
# 1. Navegue até o diretório
cd /mnt/e/TI/git/e-chamado/src/EChamado/Server/EChamado.Server

# 2. Limpe e reconstrua
dotnet clean
dotnet build

# 3. Execute
dotnet run
```

## 🧪 Como Testar

### 1. Obter um Token Válido

```bash
curl -k -X POST https://localhost:7132/connect/token \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=password" \
  -d "username=admin@admin.com" \
  -d "password=Admin@123" \
  -d "client_id=mobile-client" \
  -d "scope=openid profile email roles api chamados"
```

**Resposta esperada:**
```json
{
  "access_token": "eyJhbGci...",
  "token_type": "Bearer",
  "expires_in": 3600
}
```

### 2. Testar a API com Token Válido

```bash
curl -k -X POST https://localhost:7296/v1/category \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer <SEU_ACCESS_TOKEN_AQUI>" \
  -d '{"name": "Teste 1", "description": "Teste 1"}'
```

**✅ Resposta esperada (sucesso):**
```json
{
  "data": {
    "id": "...",
    "name": "Teste 1",
    "description": "Teste 1",
    "createdAt": "2025-11-23..."
  },
  "success": true,
  "message": "Category created successfully"
}
```

### 3. Testar sem Token (Deve Retornar 401)

```bash
curl -k -X POST https://localhost:7296/v1/category \
  -H "Content-Type: application/json" \
  -d '{"name": "Teste 1", "description": "Teste 1"}'
```

**✅ Resposta esperada (401 Unauthorized):**
```
HTTP/1.1 401 Unauthorized
WWW-Authenticate: Bearer
```

**❌ Antes da correção (ERRADO):**
```html
<!DOCTYPE html>
<html>... (página de login)
```

## 📊 Comparação Antes/Depois

| Cenário | Antes (ERRADO) | Depois (CORRETO) |
|---------|----------------|------------------|
| **Token válido** | HTML de login | JSON com dados |
| **Token inválido** | Redirect 302 → HTML | HTTP 401 + JSON |
| **Sem token** | Redirect 302 → HTML | HTTP 401 + JSON |
| **Header WWW-Authenticate** | ❌ Ausente | ✅ Presente |
| **Content-Type resposta** | text/html | application/json |

## 🎯 Casos de Uso Afetados

Esta correção resolve problemas para:

✅ **APIs REST** - Retorna status HTTP correto
✅ **Aplicações móveis** - Pode detectar 401 e pedir re-login
✅ **SPAs (Blazor WASM)** - Intercepta 401 e redireciona para login
✅ **Postman/Insomnia** - Recebe JSON em vez de HTML
✅ **Swagger/Scalar** - Testa autenticação corretamente

## 📝 Nota Importante: Cookie "External" Ainda Existe

O esquema de cookie "External" **ainda está registrado** e pode ser usado explicitamente em páginas Blazor Server ou MVC que precisam de redirecionamento para login.

Para usar cookie auth explicitamente em um endpoint:

```csharp
[Authorize(AuthenticationSchemes = "External")]
public IActionResult SomePageRequiringCookieAuth()
{
    // Este endpoint vai redirecionar para /Account/Login se não autenticado
}
```

Mas os endpoints de API usarão OpenIddict Validation por padrão (Bearer tokens).

## ⚠️ Se Você Ainda Vê HTML

Se após aplicar a correção você ainda vê HTML de login:

1. **Certifique-se que reconstruiu o projeto:**
   ```bash
   dotnet clean
   dotnet build
   ```

2. **Pare COMPLETAMENTE o servidor e reinicie:**
   - Ctrl+C no terminal
   - Mate qualquer processo dotnet pendente
   - Inicie novamente

3. **Verifique se está testando o servidor correto:**
   - Auth Server: `https://localhost:7132` (emite tokens)
   - API Server: `https://localhost:7296` (valida tokens)

4. **Verifique se o token não expirou:**
   - Tokens expiram em 1 hora (3600 segundos)
   - Gere um novo token se necessário

5. **Verifique o header Authorization:**
   - Deve ser exatamente: `Authorization: Bearer <token>`
   - Sem quebras de linha no token
   - Token completo sem cortes

## 🔗 Documentação Relacionada

- **CORRECAO-FINAL-AUTH.md** - Correção completa da autenticação OpenIddict
- **docs/ARQUITETURA-AUTENTICACAO.md** - Arquitetura de autenticação
- **CLAUDE.md** - Guia do projeto

## ✅ Status

**🟢 Correção Aplicada**

- [x] Identificado problema no `DefaultChallengeScheme`
- [x] Alterado para `OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme`
- [x] Documentação criada
- [ ] Rebuild e teste pendente (aguardando usuário parar servidor)
