# Changelog - Migração para OpenIddict

## [2.0.0] - 2025-11-19

### 🔥 BREAKING CHANGES

#### Endpoints Removidos
- ❌ **REMOVIDO** `POST /v1/auth/login`
  - **Substituído por:** `POST /connect/token` (porta 7132)
  - **Impacto:** Clientes que faziam login via `/v1/auth/login` devem migrar para `/connect/token`

- ❌ **REMOVIDO** `POST /v1/auth/register`
  - **Substituído por:** Registro via Auth Server (porta 7132)
  - **Impacto:** Registro de novos usuários deve ser feito no Auth Server

### ➕ Adicionado

#### Documentação
- ✅ `AUTENTICACAO-SISTEMAS-EXTERNOS.md` - Guia completo de autenticação
- ✅ `exemplos-autenticacao-openiddict.md` - Exemplos práticos em múltiplas linguagens
- ✅ `MIGRATION-GUIDE-JWT-TO-OPENIDDICT.md` - Guia de migração detalhado
- ✅ `test-openiddict-login.sh` - Script de teste para Bash/Linux/WSL
- ✅ `test-openiddict-login.ps1` - Script de teste para PowerShell/Windows
- ✅ `test-openiddict-login.py` - Script de teste para Python

#### Recursos de Autenticação
- ✅ Suporte a **Refresh Token** (renovar tokens expirados)
- ✅ Suporte a **ID Token** (informações do usuário em formato padronizado)
- ✅ Suporte a **múltiplos Grant Types**:
  - Authorization Code + PKCE (SPAs)
  - Password Grant (Mobile/Desktop/CLI)
  - Client Credentials (M2M)
  - Refresh Token
- ✅ Múltiplos clientes configurados automaticamente:
  - `bwa-client` - Blazor WASM
  - `mobile-client` - Apps Mobile/Desktop/Scripts

#### Segurança
- ✅ Tokens assinados com **RSA-SHA256** (ao invés de HMAC-SHA256)
- ✅ Suporte a certificados de produção
- ✅ Suporte a revogação de tokens
- ✅ Suporte a introspection de tokens

### ❌ Removido

#### Código Legado JWT Customizado

**Commands & Handlers:**
```
src/EChamado/Server/EChamado.Server.Application/UseCases/Auth/
├── Commands/
│   ├── GetTokenCommand.cs
│   ├── LoginUserCommand.cs
│   ├── RegisterUserCommand.cs
│   └── Handlers/
│       ├── GetTokenCommandHandler.cs
│       ├── LoginUserCommandHandler.cs
│       └── RegisterUserCommandHandler.cs
```

**Endpoints:**
```
src/EChamado/Server/EChamado.Server/Endpoints/Auth/
├── LoginUserEndpoint.cs
├── RegisterUserEndpoint.cs
└── DTOs/
    ├── LoginRequestDto.cs
    ├── RegisterRequestDto.cs
    └── AuthDTOSExtensions.cs
```

**Notifications:**
```
src/EChamado/Server/EChamado.Server.Application/UseCases/Auth/Notifications/
├── LoginUserNotification.cs
├── RegisterUserNotification.cs
└── Handlers/
    └── AuthNotificationHandler.cs
```

**Total de arquivos removidos:** 13 arquivos + 4 diretórios

### 🔄 Modificado

#### Arquivos Atualizados
- ✏️ `src/EChamado/Server/EChamado.Server/Endpoints/Endpoint.cs`
  - Removido mapeamento dos endpoints `/v1/auth/login` e `/v1/auth/register`
  - Adicionado comentário indicando migração para OpenIddict

- ✏️ `CLAUDE.md`
  - Atualizada seção "Authentication Flow" com informações detalhadas do OpenIddict
  - Adicionadas instruções sobre Grant Types suportados
  - Adicionados exemplos de como obter tokens
  - Adicionados links para documentação adicional

### 📊 Estatísticas da Mudança

| Métrica | Quantidade |
|---------|-----------|
| **Arquivos Removidos** | 13 |
| **Diretórios Removidos** | 4 |
| **Arquivos Adicionados** | 6 (documentação + scripts) |
| **Arquivos Modificados** | 2 |
| **Linhas de Código Removidas** | ~500 |
| **Linhas de Documentação Adicionadas** | ~1,200 |

### 🎯 Benefícios

1. **Arquitetura Simplificada**
   - Um único sistema de autenticação ao invés de dois
   - Menos código para manter
   - Configuração centralizada

2. **Segurança Aprimorada**
   - RSA-SHA256 ao invés de HMAC-SHA256
   - Certificados ao invés de chave simétrica
   - Suporte a revogação e introspection

3. **Compatibilidade Aumentada**
   - Padrão OAuth 2.0 / OpenID Connect
   - Compatível com qualquer cliente OIDC
   - Integração facilitada com sistemas externos

4. **Mais Funcionalidades**
   - Refresh tokens (renovação automática)
   - ID tokens (claims padronizados)
   - Múltiplos grant types
   - Múltiplos clientes

5. **Melhor Developer Experience**
   - Scripts de teste prontos
   - Documentação completa
   - Exemplos em múltiplas linguagens
   - Guia de migração detalhado

### 🔧 Ações Necessárias para Desenvolvedores

#### Imediatas (Obrigatórias)
1. ✅ Atualizar código que usa `POST /v1/auth/login`
2. ✅ Trocar endpoint para `POST https://localhost:7132/connect/token`
3. ✅ Atualizar formato do request (JSON → form-urlencoded)
4. ✅ Adicionar `client_id` ao request (`mobile-client`)

#### Recomendadas
1. ✅ Implementar suporte a Refresh Token
2. ✅ Salvar e usar ID Token para claims do usuário
3. ✅ Testar usando os scripts fornecidos
4. ✅ Ler documentação em `AUTENTICACAO-SISTEMAS-EXTERNOS.md`

### 📝 Exemplo de Migração

**ANTES:**
```csharp
var response = await httpClient.PostAsJsonAsync("/v1/auth/login", new {
    Email = "admin@admin.com",
    Password = "Admin@123"
});
```

**DEPOIS:**
```csharp
var authClient = new HttpClient { BaseAddress = new Uri("https://localhost:7132") };
var content = new FormUrlEncodedContent(new[] {
    new KeyValuePair<string, string>("grant_type", "password"),
    new KeyValuePair<string, string>("username", "admin@admin.com"),
    new KeyValuePair<string, string>("password", "Admin@123"),
    new KeyValuePair<string, string>("client_id", "mobile-client"),
    new KeyValuePair<string, string>("scope", "openid profile email roles api")
});
var response = await authClient.PostAsync("/connect/token", content);
```

### 🧪 Como Testar

Execute um dos scripts de teste:

```bash
# Bash
./test-openiddict-login.sh

# PowerShell
.\test-openiddict-login.ps1

# Python
python test-openiddict-login.py
```

Todos os scripts:
- Fazem login automaticamente
- Testam chamada à API
- Testam refresh token
- Salvam tokens em `.tokens.json`

### 📚 Documentação

- **Guia Principal:** `AUTENTICACAO-SISTEMAS-EXTERNOS.md`
- **Exemplos:** `exemplos-autenticacao-openiddict.md`
- **Migração:** `MIGRATION-GUIDE-JWT-TO-OPENIDDICT.md`
- **Projeto:** `CLAUDE.md` (atualizado)

### ⚠️ Avisos Importantes

1. **Porta Mudou:** Auth Server agora é **porta 7132** (antes era 7296)
2. **Formato Mudou:** JSON → form-urlencoded
3. **Campo Mudou:** `email` → `username`
4. **Novo Campo:** Adicionar `client_id` e `scope`
5. **Token Diferente:** Novo formato RSA-SHA256

### 🚀 Próximos Passos

1. Compile o projeto: `dotnet build`
2. Execute os servidores:
   - Terminal 1: `cd src/EChamado/Echamado.Auth && dotnet run`
   - Terminal 2: `cd src/EChamado/Server/EChamado.Server && dotnet run`
3. Execute um script de teste para validar
4. Atualize código de clientes conforme guia de migração

### 📞 Suporte

Para dúvidas ou problemas:
1. Consulte `MIGRATION-GUIDE-JWT-TO-OPENIDDICT.md`
2. Execute scripts de teste para diagnóstico
3. Verifique logs em Elasticsearch/Kibana
4. Consulte documentação OpenIddict: https://documentation.openiddict.com/

---

**Versão:** 2.0.0
**Data:** 19 de Novembro de 2025
**Autor:** Claude Code
**Status:** ✅ Completo
