# Guia: Resolver 401 ao Enviar Token Bearer para API

## 📌 Situação Atual

Você está recebendo **HTTP 401 Unauthorized** ao enviar um Bearer token válido para a API:

```bash
POST https://localhost:7296/v1/category
Authorization: Bearer eyJhbGci...
Content-Type: application/json

{"name": "Teste 1", "description": "Teste 1"}

# Resposta: HTTP 401 Unauthorized
```

## ✅ Progresso Até Agora

**Problema ANTERIOR (RESOLVIDO):**
- ❌ API retornava HTML da página de login
- ✅ **CORRIGIDO** - Agora retorna HTTP 401 (JSON) corretamente

**Problema ATUAL:**
- Token é gerado com sucesso pelo Echamado.Auth (porta 7132)
- Mas é **rejeitado** pelo EChamado.Server (porta 7296)
- Retorna 401 em vez de aceitar o token e processar a requisição

## 🔍 Causa Provável

O arquivo `IdentityConfig.cs` foi corrigido para validar tokens Bearer corretamente, mas o **EChamado.Server não foi reconstruído** com as mudanças.

**Mudança aplicada** (já feita no código):
```csharp
// src/EChamado/Server/EChamado.Server.Infrastructure/Configuration/IdentityConfig.cs
// Linha 104

// ANTES:
options.DefaultChallengeScheme = "External"; // ❌ Redirecionava para login

// DEPOIS:
options.DefaultChallengeScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme; // ✅ Valida token
```

## 🚀 Solução: Reconstruir EChamado.Server no Windows

### Passo 1: Parar o EChamado.Server

Se o servidor estiver rodando, **pressione Ctrl+C** no terminal onde ele está executando.

Se houver processos dotnet travados:

```powershell
# No PowerShell como Administrador
Get-Process -Name "dotnet" | Stop-Process -Force
```

### Passo 2: Executar o Script de Rebuild

**Opção A - Usando o Script PowerShell (RECOMENDADO):**

```powershell
# 1. Abra o PowerShell como Administrador
# 2. Navegue até o diretório do projeto:
cd E:\TI\git\e-chamado\src\EChamado\Server\EChamado.Server

# 3. Execute o script de rebuild:
.\rebuild-windows.ps1

# O script irá:
# - Parar processos dotnet
# - Limpar projeto (dotnet clean)
# - Remover bin/obj recursivamente
# - Limpar cache NuGet
# - Restaurar pacotes
# - Reconstruir projeto
# - Perguntar se deseja executar o servidor
```

**Opção B - Manual (se o script não funcionar):**

```powershell
# 1. Navegue até o diretório
cd E:\TI\git\e-chamado\src\EChamado\Server\EChamado.Server

# 2. Limpe o projeto
dotnet clean

# 3. Remova pastas bin/obj (opcional mas recomendado)
Get-ChildItem -Path . -Include bin,obj -Recurse -Directory -Force | Remove-Item -Recurse -Force

# 4. Restaure pacotes
dotnet restore --force

# 5. Reconstrua
dotnet build

# 6. Execute
dotnet run --launch-profile https
```

### Passo 3: Aguardar Servidor Iniciar

Aguarde até ver estas mensagens no console:

```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7296
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5125
```

### Passo 4: Testar Autenticação

**Opção A - Usando Script PowerShell (RECOMENDADO):**

```powershell
# Em um NOVO terminal PowerShell
cd E:\TI\git\e-chamado
.\test-api-with-token.ps1
```

Este script irá:
1. ✅ Obter token do Echamado.Auth (porta 7132)
2. ✅ Testar API sem token (deve retornar 401)
3. ✅ Testar API com token (deve retornar 200/201 com JSON)
4. ✅ Mostrar diagnóstico detalhado em caso de erro

**Opção B - Manual com curl:**

```bash
# 1. Obter token
curl -k -X POST https://localhost:7132/connect/token \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=password" \
  -d "username=admin@admin.com" \
  -d "password=Admin@123" \
  -d "client_id=mobile-client" \
  -d "scope=openid profile email roles api chamados"

# Copie o access_token da resposta

# 2. Testar API COM token (substitua <TOKEN> pelo access_token copiado)
curl -k -X POST https://localhost:7296/v1/category \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer <TOKEN>" \
  -d '{"name": "Teste Com Token", "description": "Teste de autenticação"}'
```

## ✅ Resultado Esperado

### Teste SEM Token (deve falhar):
```
HTTP/1.1 401 Unauthorized
WWW-Authenticate: Bearer
```

### Teste COM Token (deve funcionar):
```json
HTTP/1.1 201 Created
Content-Type: application/json

{
  "data": {
    "id": "...",
    "name": "Teste Com Token",
    "description": "Teste de autenticação",
    "createdAt": "2025-11-23T..."
  },
  "success": true,
  "message": "Category created successfully"
}
```

## 🔧 Se o Problema Persistir

Se após o rebuild você ainda receber 401, verifique:

### 1. Verificar se ambos os servidores estão rodando:

```powershell
# Verificar portas em uso
netstat -ano | findstr "7132"  # Echamado.Auth
netstat -ano | findstr "7296"  # EChamado.Server
```

### 2. Verificar logs do EChamado.Server:

Procure por erros de validação de token no console do servidor:
- `Bearer token validation failed`
- `IDX10503: Signature validation failed`
- `IDX10205: Issuer validation failed`

### 3. Verificar configuração do Issuer:

**Arquivo:** `src/EChamado/Server/EChamado.Server.Infrastructure/Configuration/IdentityConfig.cs`

Linha 184 deve estar:
```csharp
options.SetIssuer(new Uri("https://localhost:7132"));
```

### 4. Verificar se o token não expirou:

Tokens expiram em **1 hora (3600 segundos)**. Se você obteve o token há mais de 1 hora, gere um novo.

### 5. Verificar se os escopos estão corretos:

O token deve incluir os escopos: `openid profile email roles api chamados`

Para verificar, copie o `id_token` e cole em https://jwt.io

## 📊 Checklist de Validação

Antes de testar:
- [ ] EChamado.Server foi reconstruído com `dotnet clean && dotnet build`
- [ ] EChamado.Server está rodando em https://localhost:7296
- [ ] Echamado.Auth está rodando em https://localhost:7132
- [ ] Token foi obtido há menos de 1 hora

Durante o teste:
- [ ] Teste SEM token retorna 401 ✅
- [ ] Teste COM token retorna 200/201 ✅
- [ ] Resposta é JSON (não HTML) ✅
- [ ] Header `WWW-Authenticate: Bearer` está presente no 401 ✅

## 🎯 Resumo Executivo

**Para resolver o 401:**

1. **Pare** o EChamado.Server (Ctrl+C)
2. **Execute** o script de rebuild:
   ```powershell
   cd E:\TI\git\e-chamado\src\EChamado\Server\EChamado.Server
   .\rebuild-windows.ps1
   ```
3. **Aguarde** o servidor iniciar completamente
4. **Teste** com o script:
   ```powershell
   cd E:\TI\git\e-chamado
   .\test-api-with-token.ps1
   ```

## 📝 Arquivos Criados

1. **rebuild-windows.ps1** - Script de rebuild para EChamado.Server
   - Localização: `src/EChamado/Server/EChamado.Server/rebuild-windows.ps1`

2. **test-api-with-token.ps1** - Script de teste completo
   - Localização: `test-api-with-token.ps1` (raiz do projeto)

3. **CORRECAO-API-REDIRECT-LOGIN.md** - Documentação da correção do redirect
   - Localização: `CORRECAO-API-REDIRECT-LOGIN.md` (raiz do projeto)

## 🔗 Documentação Relacionada

- **CORRECAO-FINAL-AUTH.md** - Correção completa do Echamado.Auth
- **CORRECAO-API-REDIRECT-LOGIN.md** - Correção do redirect HTML para 401
- **docs/ARQUITETURA-AUTENTICACAO.md** - Arquitetura completa
- **CLAUDE.md** - Guia geral do projeto
