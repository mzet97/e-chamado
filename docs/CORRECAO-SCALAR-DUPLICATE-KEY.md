# Correção: Erro de Chave Duplicada "v1" no Scalar

## ❌ Erro Original

```
System.ArgumentException
  HResult=0x80070057
  Message=An item with the same key has already been added. Key: v1
  at Microsoft.Extensions.DependencyInjection.SwaggerGenOptionsExtensions.SwaggerDoc(SwaggerGenOptions swaggerGenOptions, String name, OpenApiInfo info)
  at EChamado.Server.Configuration.ScalarConfig.<>c.<AddApiDocumentation>b__0_1(SwaggerGenOptions c) in E:\TI\git\e-chamado\src\EChamado\Server\EChamado.Server\Configuration\ScalarConfig.cs:line 235
```

## 🔍 Causa Raiz

O método `c.SwaggerDoc("v1", ...)` estava sendo chamado **duas vezes** no arquivo `ScalarConfig.cs`:

1. **Primeira ocorrência** (linha 23-91): Definição original com todas as configurações
2. **Segunda ocorrência** (linha 235-239): Adicionada por engano durante a implementação do agrupamento de endpoints

```csharp
// Primeira definição (CORRETO - linha 23)
c.SwaggerDoc("v1", new OpenApiInfo
{
    Title = "EChamado API",
    Version = "v1.0.0",
    Description = @"..."
});

// ... código ...

// Segunda definição DUPLICADA (ERRADO - linha 235)
c.SwaggerDoc("v1", new OpenApiInfo  // ❌ ERRO!
{
    Version = "v1.0.0",
    Title = "EChamado API",
});
```

## ✅ Solução Aplicada

**Arquivo:** `src/EChamado/Server/EChamado.Server/Configuration/ScalarConfig.cs`

Removidas as linhas duplicadas 235-239 e linhas desnecessárias:

### Antes (ERRADO):
```csharp
// Ordenar tags alfabeticamente
c.OrderActionsBy(api => $"{api.GroupName}_{api.HttpMethod}_{api.RelativePath}");

// Descrições detalhadas para cada tag
c.SwaggerDoc("v1", new OpenApiInfo  // ❌ DUPLICADO!
{
    Version = "v1.0.0",
    Title = "EChamado API",
});

// Tags com descrições
c.DescribeAllParametersInCamelCase();

c.DocInclusionPredicate((name, api) => true);
```

### Depois (CORRETO):
```csharp
// Ordenar tags alfabeticamente
c.OrderActionsBy(api => $"{api.GroupName}_{api.HttpMethod}_{api.RelativePath}");

c.DocInclusionPredicate((name, api) => true);
```

## ✅ Resultado do Build

Após a correção:

```
Build succeeded.
    47 Warning(s)
    0 Error(s)

Time Elapsed 00:00:49.20
```

## 🚀 Como Testar

### 1. Rebuild do Servidor

**PowerShell (Windows):**
```powershell
cd E:\TI\git\e-chamado\src\EChamado\Server\EChamado.Server
dotnet clean
dotnet build
dotnet run --launch-profile https
```

**Bash (WSL):**
```bash
cd /mnt/e/TI/git/e-chamado/src/EChamado/Server/EChamado.Server
dotnet clean
dotnet build
dotnet run --launch-profile https
```

### 2. Verificar Servidor Iniciou

Aguarde ver no console:

```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7296
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5125
```

### 3. Acessar Scalar

Abra o navegador em:

```
https://localhost:7296/api-docs/v1
```

### 4. Verificar Agrupamento de Endpoints

Você deve ver na sidebar do Scalar os endpoints agrupados por entidade:

```
📁 Category
   ├─ POST /v1/category
   ├─ GET /v1/category/{id}
   ├─ PUT /v1/category/{id}
   ├─ DELETE /v1/category/{id}
   └─ GET /v1/categories

📁 Order
   ├─ POST /v1/order
   ├─ GET /v1/order/{id}
   └─ GET /v1/orders

... (outras entidades)
```

## 📝 Lições Aprendidas

### 1. Evitar Duplicação de Configuração

Sempre verificar se uma configuração já existe antes de adicionar nova:

```csharp
// ❌ ERRADO - Adicionar sem verificar
c.SwaggerDoc("v1", new OpenApiInfo { ... });

// ✅ CORRETO - Verificar primeiro se não existe
// Se c.SwaggerDoc() já foi chamado, não chame novamente
```

### 2. SwaggerDoc é um Dictionary

O método `SwaggerDoc()` adiciona entradas a um dicionário interno. Chamar duas vezes com a mesma chave (`"v1"`) causa `ArgumentException`.

### 3. Testar Build Após Mudanças

Sempre fazer `dotnet build` após alterações em arquivos de configuração para detectar erros cedo:

```bash
dotnet build  # Antes de fazer commit
```

## 🔗 Documentação Relacionada

- **AGRUPAMENTO-ENDPOINTS-SCALAR.md** - Documentação completa do agrupamento de endpoints
- **ScalarConfig.cs** - Arquivo de configuração do Scalar/Swagger
- **CLAUDE.md** - Guia geral do projeto

## ✅ Status

**🟢 CORRIGIDO**

- [x] Erro identificado (chave duplicada "v1")
- [x] Linhas duplicadas removidas
- [x] Build bem-sucedido (0 erros)
- [x] Documentação criada
- [ ] Teste em runtime pendente (aguardando usuário executar servidor)

---

**Data:** 23/11/2025
**Arquivo Modificado:** `ScalarConfig.cs` (linhas 231-234)
**Tipo de Erro:** Configuração duplicada
**Impacto:** Bloqueava inicialização do servidor
**Severidade:** Alta (🔴)
**Resolução:** Simples (remoção de código duplicado)
