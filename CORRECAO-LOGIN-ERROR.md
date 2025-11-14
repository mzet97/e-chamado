# Correção do Erro de Login - "Transient Failure"

## 🔴 Problema Identificado

Ao fazer login no sistema, aparecia o erro:
```
An error occurred: An exception has been raised that is likely due to a transient failure.
```

## 🔍 Causa Raiz

**Strings de conexão diferentes** entre os projetos:

### Antes (❌ ERRADO):

**Echamado.Auth** (`appsettings.json`):
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Pooling=true;Database=e-chamado;User Id=postgres;Password=dsv@123;"
  }
}
```

**EChamado.Server** (`appsettings.json`):
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=192.168.31.52;Port=5432;Pooling=true;Database=e-chamado;User Id=app;Password=Admin@123;"
  }
}
```

### ❓ Por que isso causava erro?

1. **Hosts diferentes**: `localhost` vs `192.168.31.52`
2. **Usuários diferentes**: `postgres` vs `app`
3. **Senhas diferentes**: `dsv@123` vs `Admin@123`
4. O `Echamado.Auth` não conseguia acessar o banco de dados corretamente
5. Ao tentar autenticar o usuário via `SignInManager`, ocorria falha de conexão

## ✅ Solução Implementada

### 1. Unificação das Strings de Conexão

**Echamado.Auth** - `appsettings.json` (CORRIGIDO):
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=192.168.31.52;Port=5432;Pooling=true;Database=e-chamado;User Id=app;Password=Admin@123;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Information",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  }
}
```

### 2. Melhor Tratamento de Erros

**Login.razor** - Adicionados logs detalhados:
```csharp
catch (Exception ex)
{
    errorMessage = $"An error occurred: {ex.Message}";
    // Log detalhado para debug
    Console.WriteLine($"Login Error: {ex}");
    if (ex.InnerException != null)
    {
        Console.WriteLine($"Inner Exception: {ex.InnerException}");
        errorMessage += $" | Details: {ex.InnerException.Message}";
    }
}
```

### 3. Logs de EF Core Habilitados

Agora você pode ver queries SQL nos logs para diagnosticar problemas:
```json
{
  "Logging": {
    "Microsoft.EntityFrameworkCore": "Information"
  }
}
```

## 🚀 Como Testar

### 1. Verificar Banco de Dados

```bash
# Certifique-se de que PostgreSQL está rodando em 192.168.31.52:5432
# e que o usuário 'app' tem acesso ao database 'e-chamado'
```

### 2. Executar os Servidores

```bash
# Terminal 1 - Echamado.Auth
cd src/EChamado/Echamado.Auth
dotnet run

# Aguarde a mensagem:
# Now listening on: https://localhost:7132

# Terminal 2 - EChamado.Server
cd src/EChamado/Server/EChamado.Server
dotnet run

# Aguarde a mensagem:
# Now listening on: https://localhost:7296

# Terminal 3 - EChamado.Client
cd src/EChamado/Client/EChamado.Client
dotnet run

# Aguarde a mensagem:
# Now listening on: https://localhost:7274
```

### 3. Testar Login

1. Abra o navegador em `https://localhost:7274`
2. Clique em "Log in"
3. Use as credenciais:
   - **Email**: `admin@echamado.com`
   - **Password**: `Admin@123`

### 4. Verificar Logs

**Se ainda houver erro**, verifique os logs no terminal do `Echamado.Auth`:

```
Login Error: [Exception completa]
Inner Exception: [Detalhes do erro]
```

Possíveis erros e soluções:

#### ❌ "Connection refused"
```
Solução: Verificar se PostgreSQL está rodando:
- Host: 192.168.31.52
- Port: 5432
```

#### ❌ "Password authentication failed for user 'app'"
```
Solução: Verificar credenciais no PostgreSQL:
User: app
Password: Admin@123
Database: e-chamado
```

#### ❌ "Database 'e-chamado' does not exist"
```
Solução: Criar o banco de dados ou rodar migrations:
cd src/EChamado/Server/EChamado.Server
dotnet ef database update
```

## 🔧 Verificações Adicionais

### Testar Conexão Diretamente

```bash
# No terminal do Echamado.Auth, verifique os logs quando iniciar:
# Você deve ver algo como:
info: Microsoft.EntityFrameworkCore.Database.Command[20101]
      Executed DbCommand (123ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
      SELECT 1
```

### Verificar Usuários no Banco

```sql
-- Conecte ao PostgreSQL e execute:
SELECT "Id", "Email", "UserName", "EmailConfirmed"
FROM "AspNetUsers"
WHERE "Email" = 'admin@echamado.com';

-- Deve retornar 1 registro
```

## 📝 Alterações Realizadas

### Arquivos Modificados:

1. ✅ `src/EChamado/Echamado.Auth/appsettings.json`
   - String de conexão corrigida
   - Logs de EF Core habilitados

2. ✅ `src/EChamado/Echamado.Auth/Components/Pages/Accounts/Login.razor`
   - Melhor tratamento de exceções
   - Logs detalhados no console

### Nenhuma Lógica de Negócio Foi Alterada

- ✅ Fluxo de autenticação permanece o mesmo
- ✅ OpenIddict configurado como antes
- ✅ Cookies e sessões funcionando normalmente

## ✅ Resultado Esperado

Após essas correções:

1. ✅ O login deve funcionar sem erros
2. ✅ Usuário é autenticado com sucesso
3. ✅ Cookie "EChamado.External" é criado
4. ✅ Redirecionamento para a aplicação funciona corretamente

## 🆘 Se o Erro Persistir

Execute este comando para ver o erro completo:

```bash
cd src/EChamado/Echamado.Auth
dotnet run

# Quando tentar fazer login, veja o console completo
# Copie e cole toda a stack trace do erro para análise
```

Ou verifique se o problema é de rede:

```bash
# Tente conectar ao PostgreSQL manualmente:
psql -h 192.168.31.52 -p 5432 -U app -d e-chamado

# Se funcionar, o problema não é de conexão
# Se não funcionar, verifique firewall/rede
```

---

**Data**: 2025-11-12
**Status**: ✅ Corrigido
**Versão**: 1.0
