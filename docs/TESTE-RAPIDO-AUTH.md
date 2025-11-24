# Teste Rápido de Autenticação - EChamado

## ✅ Correções Aplicadas

### 1. Erro CS0117 - Constante OpenId
**Problema:** `Permissions.Scopes.OpenId` não existe
**Solução:** Trocado para `Scopes.OpenId` (sem o `Permissions`)

### 2. Erro invalid_scope - Scope "chamados" faltando
**Problema:** Scope "chamados" não estava registrado
**Solução:**
- Adicionado em `Program.cs`: `options.RegisterScopes(..., "chamados")`
- Implementado `RegisterCustomScopesAsync()` no `OpenIddictWorker`

### 3. OpenIddictWorker incompleto
**Problema:** Worker não registrava clientes nem scopes
**Solução:** Implementado completo com:
- Registro de scopes personalizados (`api`, `chamados`)
- Criação/atualização de clientes (`bwa-client`, `mobile-client`)
- Logs detalhados

## 🚀 Como Testar (Passo a Passo)

### Passo 1: Inicie o servidor de autenticação

```bash
cd src/EChamado/Echamado.Auth
dotnet run
```

**Aguarde os logs de inicialização:**
```
Ensuring database is created for OpenIddict...
✅ Database ready for OpenIddict
Registering custom scopes...
✅ Scope 'api' registered
✅ Scope 'chamados' registered
✅ Custom scopes registration completed
Configuring bwa-client (Blazor WebAssembly)...
✅ Client 'bwa-client' created
Configuring mobile-client...
✅ Client 'mobile-client' created
✅ OpenIddict clients and scopes configured successfully

info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7132
```

⚠️ **IMPORTANTE:** Se você não ver as mensagens de "✅ Scope registered", significa que:
- Os scopes já existiam no banco (OK)
- OU houve um erro (verifique os logs completos)

### Passo 2: Teste a autenticação

#### Opção A: Usando curl (manual)

```bash
curl -k -X POST https://localhost:7132/connect/token \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=password" \
  -d "username=admin@admin.com" \
  -d "password=Admin@123" \
  -d "client_id=mobile-client" \
  -d "scope=openid profile email roles api chamados"
```

#### Opção B: Usando o script automatizado

```bash
./test-auth-fixed.sh
```

### Passo 3: Verifique a resposta

✅ **Sucesso - Você deve ver:**
```json
{
  "access_token": "eyJhbGciOiJSUzI1NiIsImtpZCI6...",
  "token_type": "Bearer",
  "expires_in": 3600,
  "refresh_token": "CfDJ8...",
  "id_token": "eyJhbGciOiJSUzI1NiIsImtpZCI6...",
  "scope": "openid profile email roles api chamados"
}
```

❌ **Se der erro:**

**Erro: "Connection refused"**
- O servidor não está rodando na porta 7132
- Verifique se iniciou corretamente (Passo 1)

**Erro: "invalid_scope"**
- O servidor não foi reiniciado após as correções
- Pare (Ctrl+C) e inicie novamente (Passo 1)

**Erro: "invalid_grant"**
- Credenciais incorretas
- Tente: `admin@admin.com` / `Admin@123`
- Ou: `user@echamado.com` / `User@123`

**Erro: "invalid_client"**
- Cliente não foi registrado
- Verifique os logs de inicialização (deve ver "✅ Client 'mobile-client' created")

## 🔍 Validando o Token

Se recebeu o token com sucesso, você pode decodificá-lo em https://jwt.io

**O token deve conter:**
- Claims: `sub`, `name`, `email`, `role`
- Scopes: `openid`, `profile`, `email`, `roles`, `api`, `chamados`
- Issuer: `https://localhost:7132`

## 📝 Testando com a API (Opcional)

Se o servidor API também estiver rodando (porta 7296):

```bash
# 1. Obter token
TOKEN=$(curl -k -s -X POST https://localhost:7132/connect/token \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=password" \
  -d "username=admin@admin.com" \
  -d "password=Admin@123" \
  -d "client_id=mobile-client" \
  -d "scope=openid profile email roles api chamados" | jq -r '.access_token')

# 2. Usar token para acessar API
curl -k -X GET https://localhost:7296/v1/categories \
  -H "Authorization: Bearer $TOKEN"
```

## 📚 Documentação Completa

Para mais detalhes, consulte:
- `docs/ARQUITETURA-AUTENTICACAO.md` - Arquitetura completa
- `CLAUDE.md` - Guia do projeto
- `test-auth-fixed.sh` - Script de teste automatizado

## 🎯 Checklist de Sucesso

- [ ] Servidor Echamado.Auth rodando na porta 7132
- [ ] Logs mostram "✅ OpenIddict clients and scopes configured successfully"
- [ ] curl retorna access_token, refresh_token e id_token
- [ ] Scope na resposta inclui "chamados"
- [ ] Token pode ser decodificado em jwt.io
- [ ] (Opcional) Token funciona para acessar API na porta 7296

Se todos os itens estão marcados: **🎉 Autenticação está 100% funcional!**

## ⚠️ Troubleshooting

### Banco de dados com scopes antigos

Se você já tinha rodado o servidor antes e os scopes não estão sendo atualizados:

**Opção 1: Limpar tabelas OpenIddict (recomendado)**
```sql
-- Conecte no PostgreSQL e execute:
DELETE FROM "OpenIddictScopes" WHERE "Name" IN ('api', 'chamados');
DELETE FROM "OpenIddictApplications";
```

Depois reinicie o servidor `Echamado.Auth` para recriar tudo.

**Opção 2: Recriar banco completo (use com cuidado!)**
```bash
cd src/EChamado/Server/EChamado.Server
dotnet ef database drop --force
dotnet ef database update
```

### Verificar scopes no banco de dados

```sql
-- Ver todos os scopes registrados
SELECT "Id", "Name", "DisplayName", "Description"
FROM "OpenIddictScopes";

-- Ver todos os clientes registrados
SELECT "Id", "ClientId", "DisplayName", "Type"
FROM "OpenIddictApplications";
```

## 🔗 Links Úteis

- OpenIddict Documentation: https://documentation.openiddict.com/
- JWT Decoder: https://jwt.io
- OpenID Connect Playground: https://openidconnect.net/
