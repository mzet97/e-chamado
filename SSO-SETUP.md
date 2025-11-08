# 🔐 SSO Setup - EChamado com OpenIddict

Este documento contém todas as informações necessárias para configurar e testar o SSO (Single Sign-On) implementado com OpenIddict usando **Authorization Code Flow + PKCE**.

---

## 📋 Arquitetura da Solução

A aplicação é composta por 3 projetos principais:

### 1. **EChamado.Server** (porta 7296)
- **Tipo**: ASP.NET Core Web API
- **Função**:
  - API Backend para o sistema de chamados
  - **Provedor OIDC** (OpenIddict Server)
  - Endpoints de autorização e token

### 2. **Echamado.Auth** (porta 7132)
- **Tipo**: Blazor Server
- **Função**:
  - Interface de usuário para login e registro
  - Autentica usuários usando ASP.NET Core Identity
  - Compartilha cookie de autenticação com o Server

### 3. **EChamado.Client** (porta 7274)
- **Tipo**: Blazor WebAssembly
- **Função**:
  - Frontend SPA (Single Page Application)
  - Cliente OIDC que consome a API
  - Usa Authorization Code + PKCE para autenticação

---

## 🔄 Fluxo de Autenticação Completo

```
┌─────────────────────────────────────────────────────────────────────┐
│                         FLUXO DE AUTENTICAÇÃO                        │
└─────────────────────────────────────────────────────────────────────┘

1. 👤 Usuário acessa EChamado.Client (https://localhost:7274)
   │
   ↓
2. 🔒 Cliente tenta acessar recurso protegido
   │
   ↓
3. 🔀 Redireciona para EChamado.Server/connect/authorize
   │   (Authority: https://localhost:7296)
   │
   ↓
4. 🍪 Server verifica cookie "EChamado.External"
   │   Cookie não encontrado? → Redireciona
   │
   ↓
5. 🔐 Redireciona para Echamado.Auth/Account/Login
   │   (https://localhost:7132/Account/Login?returnUrl=...)
   │
   ↓
6. 📝 Usuário preenche credenciais e faz login
   │   Email: admin@echamado.com
   │   Senha: Admin@123
   │
   ↓
7. ✅ SignInManager cria cookie "EChamado.External"
   │   Cookie compartilhado entre Auth (7132) e Server (7296)
   │
   ↓
8. ↩️  Redireciona de volta para Server com cookie
   │   URL: https://localhost:7296/connect/authorize?...
   │
   ↓
9. 🔍 Server valida cookie e busca usuário no Identity
   │   Extrai claims: Subject, Email, Name, Roles
   │
   ↓
10. 🎫 Server emite Authorization Code
    │   Code gerado com PKCE code_challenge
    │
    ↓
11. ↩️  Redireciona para Client com code
    │   URL: https://localhost:7274/authentication/login-callback?code=...
    │
    ↓
12. 🔄 Client troca code por tokens
    │   POST /connect/token
    │   Body: code + code_verifier (PKCE)
    │
    ↓
13. 🎁 Server retorna tokens
    │   - access_token
    │   - id_token
    │   - refresh_token
    │
    ↓
14. 🚀 Client armazena tokens e está autenticado!
    │   Todas as chamadas API incluem access_token automaticamente
    │
    ✅ AUTENTICAÇÃO CONCLUÍDA
```

---

## 🛠️ Pré-requisitos

- **.NET 9.0 SDK**
- **PostgreSQL** (rodando na porta 5432)
- **Docker** (opcional, para rodar PostgreSQL via docker-compose)

---

## 🚀 Como Rodar a Aplicação

### **Opção 1: Usando Docker Compose (Recomendado)**

#### Passo 1: Iniciar PostgreSQL

```bash
cd /home/user/e-chamado
docker-compose up -d postgres
```

Aguarde alguns segundos para o PostgreSQL inicializar.

#### Passo 2: Rodar as 3 aplicações em terminais separados

**Terminal 1 - EChamado.Server (API + OIDC Provider):**

```bash
cd src/EChamado/Server/EChamado.Server
dotnet run
```

Aguarde a mensagem:
```
Now listening on: https://localhost:7296
```

**Terminal 2 - Echamado.Auth (UI de Login):**

```bash
cd src/EChamado/Echamado.Auth
dotnet run
```

Aguarde a mensagem:
```
Now listening on: https://localhost:7132
```

**Terminal 3 - EChamado.Client (Blazor WASM):**

```bash
cd src/EChamado/Client/EChamado.Client
dotnet run
```

Aguarde a mensagem:
```
Now listening on: https://localhost:7274
```

---

### **Opção 2: Usando PostgreSQL Local**

Se você já tem PostgreSQL instalado localmente, atualize a connection string em:

- `src/EChamado/Server/EChamado.Server/appsettings.json`
- `src/EChamado/Echamado.Auth/appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=e-chamado;User Id=seu_usuario;Password=sua_senha;"
  }
}
```

Depois siga os passos 2 acima para rodar as aplicações.

---

## 🧪 Testando o SSO

### **1. Primeiro Acesso (Registro)**

1. Acesse `https://localhost:7274` (EChamado.Client)
2. Você será automaticamente redirecionado para a tela de login
3. Clique em **"Register"**
4. Preencha:
   - **Username**: `testuser`
   - **Email**: `test@echamado.com`
   - **Password**: `Test@123`
   - **Confirm Password**: `Test@123`
5. Clique em **"Register"**
6. Aguarde redirecionamento para tela de login

### **2. Login**

1. Na tela de login, preencha:
   - **Email**: `test@echamado.com`
   - **Password**: `Test@123`
2. Clique em **"Log In"**
3. Você será redirecionado de volta ao Client e estará autenticado!

### **3. Usuários Pré-configurados**

O sistema cria automaticamente 2 usuários na primeira inicialização:

#### **Admin:**
- Email: `admin@echamado.com`
- Senha: `Admin@123`
- Role: `Admin`

#### **Usuário Teste:**
- Email: `user@echamado.com`
- Senha: `User@123`
- Role: `User`

---

## 🔍 Verificando a Autenticação

### **1. Verificar Tokens no Browser**

Abra o **Developer Tools** (F12) → **Application** → **Local Storage** → `https://localhost:7274`

Você deve ver:
- `oidc.user:<authority>:<clientId>` contendo os tokens

### **2. Verificar Cookie Compartilhado**

Abra **Developer Tools** → **Application** → **Cookies**

Você deve ver o cookie `EChamado.External` tanto em:
- `https://localhost:7296` (Server)
- `https://localhost:7132` (Auth)

### **3. Testar Refresh Token**

O Blazor WASM automaticamente renova o access_token usando o refresh_token quando ele expira.

Para forçar uma renovação:
1. Aguarde 24 horas (tempo de expiração configurado)
2. Ou: Delete o access_token no Local Storage e recarregue a página
3. O sistema deve renovar automaticamente

---

## 🔐 Segurança Implementada

✅ **Authorization Code Flow** (não Implicit Flow)
✅ **PKCE obrigatório** (Proof Key for Code Exchange)
✅ **Cookies seguros** (Secure=Always, HttpOnly=true)
✅ **SameSite=None** (permite compartilhamento entre portas diferentes)
✅ **Data Protection compartilhado** (chaves em `/tmp/EChamado-DataProtection-Keys`)
✅ **CORS configurado** (apenas origens permitidas)
✅ **Lockout habilitado** (proteção contra brute force)
✅ **Refresh Tokens** (renovação automática de tokens)
✅ **Claims completos** (Subject, Email, Name, Roles)

---

## 📝 Endpoints Disponíveis

### **EChamado.Server (7296)**

| Endpoint | Método | Descrição |
|----------|--------|-----------|
| `/connect/authorize` | GET/POST | Endpoint de autorização OIDC |
| `/connect/token` | POST | Endpoint de troca de código/refresh token |

### **Echamado.Auth (7132)**

| Endpoint | Método | Descrição |
|----------|--------|-----------|
| `/Account/Login` | GET/POST | Página de login |
| `/Account/Register` | GET/POST | Página de registro |

### **EChamado.Client (7274)**

| Endpoint | Descrição |
|----------|-----------|
| `/authentication/login` | Inicia fluxo de login |
| `/authentication/login-callback` | Callback após autorização |
| `/authentication/logout` | Faz logout |
| `/authentication/logout-callback` | Callback após logout |

---

## 🐛 Troubleshooting

### **Problema: Erro de conexão com PostgreSQL**

**Solução:**
```bash
# Verificar se PostgreSQL está rodando
docker ps | grep postgres

# Se não estiver, iniciar:
docker-compose up -d postgres

# Verificar logs:
docker logs e-chamado-postgres
```

### **Problema: Cookie não está sendo compartilhado**

**Possíveis causas:**
1. **HTTPS não está funcionando** → Use certificados de desenvolvimento válidos
2. **SameSite incorreto** → Deve ser `SameSite=None` para compartilhar entre portas
3. **Data Protection não compartilhado** → Verifique se ambas as apps usam o mesmo diretório

**Verificar Data Protection:**
```bash
ls -la /tmp/EChamado-DataProtection-Keys/
```

Deve existir e conter arquivos XML com chaves.

### **Problema: "The specified grant is not implemented"**

**Causa:** Tentando usar um grant type não suportado.

**Grants suportados:**
- `authorization_code` (com PKCE)
- `refresh_token`
- `password` (para mobile)
- `client_credentials`

### **Problema: Tokens expiram muito rápido**

**Solução:** Ajuste em `appsettings.json`:

```json
{
  "AppSettings": {
    "ExpirationHours": 24  // Tempo de expiração do token
  }
}
```

### **Problema: Erro "User no longer exists" no refresh token**

**Causa:** Usuário foi deletado do banco de dados.

**Solução:** O sistema valida se o usuário ainda existe antes de renovar o token. Crie um novo usuário ou faça login novamente.

---

## 📊 Configurações Importantes

### **Data Protection**

**Localização das chaves:**
```
/tmp/EChamado-DataProtection-Keys/
```

**Compartilhado entre:**
- EChamado.Server (7296)
- Echamado.Auth (7132)

### **Cookie "EChamado.External"**

**Configurações:**
- Nome: `EChamado.External`
- SameSite: `None`
- Secure: `true`
- HttpOnly: `true`
- Expira: 30 minutos
- SlidingExpiration: `true`

### **Cliente OIDC (bwa-client)**

**Configurações:**
- ClientId: `bwa-client`
- ClientType: `Public`
- RequirePKCE: `true`
- GrantTypes: `authorization_code`, `refresh_token`
- RedirectUri: `https://localhost:7274/authentication/login-callback`
- PostLogoutUri: `https://localhost:7274/`

---

## 🎯 Próximos Passos

### **Para Produção:**

1. **Certificados SSL**
   - Substitua certificados de desenvolvimento por certificados válidos
   - Configure certificado assimétrico para assinatura de tokens

2. **Data Protection**
   - Mova para Redis ou Azure KeyVault
   - Não use filesystem em produção distribuída

3. **Configurações de Ambiente**
   - Use variáveis de ambiente para secrets
   - Configure diferentes appsettings por ambiente

4. **Logging e Monitoramento**
   - Configure Application Insights ou similar
   - Monitore falhas de autenticação

5. **Rate Limiting**
   - Implemente rate limiting nos endpoints de token
   - Proteja contra ataques de força bruta

### **Funcionalidades Adicionais:**

- [ ] Logout completo (revogar tokens)
- [ ] Endpoint `/connect/userinfo`
- [ ] Tela de consentimento
- [ ] Two-Factor Authentication (2FA)
- [ ] External providers (Google, Microsoft, etc.)
- [ ] Recuperação de senha

---

## 📚 Referências

- [OpenIddict Documentation](https://documentation.openiddict.com/)
- [OAuth 2.0 Authorization Code Flow](https://oauth.net/2/grant-types/authorization-code/)
- [PKCE (RFC 7636)](https://tools.ietf.org/html/rfc7636)
- [OpenID Connect Core](https://openid.net/specs/openid-connect-core-1_0.html)

---

## ✅ Checklist de Validação

Após configurar, verifique:

- [ ] PostgreSQL está rodando e acessível
- [ ] Migrations foram aplicadas automaticamente
- [ ] Usuários padrão foram criados (admin e user)
- [ ] Cliente OIDC (bwa-client) foi registrado
- [ ] As 3 aplicações estão rodando nas portas corretas
- [ ] Login funciona e redireciona corretamente
- [ ] Cookie é compartilhado entre Server e Auth
- [ ] Tokens são recebidos após login
- [ ] API aceita chamadas autenticadas
- [ ] Refresh token funciona

---

## 🤝 Suporte

Se encontrar problemas:

1. Verifique os logs das 3 aplicações
2. Verifique se o PostgreSQL está acessível
3. Verifique se as portas 7132, 7274 e 7296 estão livres
4. Limpe o cache do navegador e cookies
5. Reinicie todas as aplicações

---

**Desenvolvido com ❤️ usando .NET 9 e OpenIddict**
