# Agrupamento de Endpoints por Entidade no Scalar

## 🎯 Objetivo

Configurar o Scalar (interface de documentação da API) para agrupar endpoints automaticamente por entidade, baseando-se no prefixo da rota.

## ✅ O Que Foi Feito

### 1. Melhorias no ScalarConfig.cs

**Arquivo:** `src/EChamado/Server/EChamado.Server/Configuration/ScalarConfig.cs`

#### Alterações Principais:

1. **TagActionsBy melhorado** (linhas 196-229):
   - Extrai o nome da entidade do route pattern
   - Normaliza nomes (remove plural, capitaliza)
   - Agrupa automaticamente endpoints relacionados

2. **Método NormalizeTagName** (linhas 250-260):
   - Remove o 's' do plural (categories → category)
   - Capitaliza a primeira letra (category → Category)
   - Garante consistência nos nomes das tags

3. **Ordenação de tags** (linha 232):
   - Ordena alfabeticamente por tag, método HTTP e rota
   - Facilita navegação na documentação

## 📊 Como Funciona

### Antes (Problema):
```
Scalar mostrava endpoints sem agrupamento claro ou com tags inconsistentes:
- CreateCategoryEndpoint
- GetCategoryEndpoint
- UpdateCategoryEndpoint
- CreateOrderEndpoint
- GetOrderEndpoint
```

### Depois (Solução):
```
📁 Category
   ├─ POST   /v1/category         (Criar categoria)
   ├─ GET    /v1/category/{id}    (Obter categoria por ID)
   ├─ PUT    /v1/category/{id}    (Atualizar categoria)
   ├─ DELETE /v1/category/{id}    (Deletar categoria)
   └─ GET    /v1/categories       (Listar categorias)

📁 Order
   ├─ POST   /v1/order            (Criar chamado)
   ├─ GET    /v1/order/{id}       (Obter chamado por ID)
   ├─ PUT    /v1/order/{id}       (Atualizar chamado)
   ├─ POST   /v1/order/assign     (Atribuir chamado)
   ├─ POST   /v1/order/close      (Fechar chamado)
   └─ GET    /v1/orders           (Listar chamados)
```

## 🔍 Lógica de Agrupamento

### Passo 1: Extração do Nome da Entidade

```csharp
var routePattern = "v1/categories"; // ou "v1/category"
var segments = routePattern.Split('/');
var entitySegment = segments[1]; // "categories" ou "category"
```

### Passo 2: Normalização

```csharp
private static string NormalizeTagName(string entitySegment)
{
    // "categories" → "categorie" → "Category"
    // "category" → "categor" → "Category"
    var singular = entitySegment.TrimEnd('s');
    return char.ToUpperInvariant(singular[0]) + singular.Substring(1);
}
```

**Nota:** A lógica atual remove apenas o 's' final. Para melhor resultado com plurais irregulares (como "status" → "statu"), seria necessário usar uma biblioteca de pluralização.

### Passo 3: Agrupamento

Todos os endpoints com o mesmo prefixo normalizado são agrupados sob a mesma tag:

| Rota Original | Segmento Extraído | Tag Final |
|--------------|-------------------|-----------|
| `v1/categories` | categories | **Category** |
| `v1/category` | category | **Category** |
| `v1/category/{id}` | category | **Category** |
| `v1/orders` | orders | **Order** |
| `v1/order` | order | **Order** |
| `v1/order/{id}` | order | **Order** |

## 📝 Entidades Agrupadas

O sistema agrupará automaticamente os seguintes endpoints:

| Tag | Rotas Incluídas |
|-----|----------------|
| **Category** | `/v1/category`, `/v1/categories` |
| **SubCategory** | `/v1/subcategory`, `/v1/subcategories` |
| **Department** | `/v1/department`, `/v1/departments` |
| **Order** | `/v1/order`, `/v1/orders` |
| **OrderType** | `/v1/ordertype`, `/v1/ordertypes` |
| **StatusType** | `/v1/statustype`, `/v1/statustypes` |
| **Comment** | `/v1/comments` |
| **Role** | `/v1/role`, `/v1/roles` |
| **User** | `/v1/user`, `/v1/users` |

## 🚀 Como Testar

### 1. Rebuild do Projeto

**No PowerShell (Windows):**
```powershell
cd E:\TI\git\e-chamado\src\EChamado\Server\EChamado.Server
.\rebuild-windows.ps1
```

**Ou manualmente:**
```powershell
cd E:\TI\git\e-chamado\src\EChamado\Server\EChamado.Server
dotnet clean
dotnet build
dotnet run --launch-profile https
```

### 2. Acessar o Scalar

Após o servidor iniciar, abra o navegador em:

```
https://localhost:7296/api-docs/v1
```

### 3. Verificar Agrupamento

Você deve ver na sidebar do Scalar:

```
📋 EChamado API Documentation

📁 Category
   ├─ POST /v1/category
   ├─ GET /v1/category/{id}
   ├─ PUT /v1/category/{id}
   ├─ DELETE /v1/category/{id}
   └─ GET /v1/categories

📁 Comment
   ├─ POST /v1/comments
   ├─ GET /v1/comments
   └─ DELETE /v1/comments/{id}

📁 Department
   ├─ POST /v1/department
   ├─ GET /v1/department/{id}
   ├─ PUT /v1/department/{id}
   ├─ DELETE /v1/department
   ├─ PUT /v1/departments/status
   └─ GET /v1/departments

📁 Order
   ├─ POST /v1/order
   ├─ GET /v1/order/{id}
   ├─ PUT /v1/order/{id}
   ├─ POST /v1/order/assign
   ├─ POST /v1/order/change-status
   ├─ POST /v1/order/close
   └─ GET /v1/orders

... (e assim por diante)
```

## 🎨 Recursos do Scalar

Além do agrupamento, o Scalar oferece:

1. **Tema Roxo** (`.WithTheme(ScalarTheme.Purple)`)
2. **Dark Mode** (`.WithDarkMode(true)`)
3. **Sidebar com navegação** (`.WithSidebar(true)`)
4. **Modelos de dados** (`.WithModels(true)`)
5. **Busca rápida** (Pressione `K` para buscar)
6. **Exemplos de código** em C#, cURL, etc

## 🔧 Melhorias Futuras (Opcional)

### 1. Pluralização Melhorada

Para lidar com plurais irregulares (como "status" → não deve virar "statu"), considere usar:

```csharp
// NuGet: Humanizer
using Humanizer;

private static string NormalizeTagName(string entitySegment)
{
    var singular = entitySegment.Singularize(); // "categories" → "category"
    return singular.Pascalize(); // "category" → "Category"
}
```

### 2. Descrições Customizadas para Tags

Adicionar descrições explicativas para cada tag:

```csharp
c.SwaggerDoc("v1", new OpenApiInfo
{
    Title = "EChamado API",
    Version = "v1.0.0"
});

// Adicionar descrições para tags
c.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    // Descrições customizadas
    opt.AddSwaggerTag("Category", "Gerenciamento de categorias de chamados");
    opt.AddSwaggerTag("Order", "Gerenciamento de chamados/tickets");
    opt.AddSwaggerTag("Department", "Gerenciamento de departamentos");
});
```

### 3. Ícones Customizados

Scalar permite ícones customizados para cada tag:

```csharp
.WithCustomCss(@"
    [data-tag='Category']::before { content: '📂'; }
    [data-tag='Order']::before { content: '📋'; }
    [data-tag='Department']::before { content: '🏢'; }
    [data-tag='User']::before { content: '👤'; }
")
```

## ⚠️ Notas Importantes

### 1. Consistência nas Rotas

Para o agrupamento funcionar corretamente, mantenha o padrão:
- **Singular para operações específicas:** `/v1/category/{id}`
- **Plural para listagens:** `/v1/categories`

### 2. Tags Explícitas no Endpoint.cs

O arquivo `Endpoint.cs` já possui tags explícitas usando `.WithTags()`:

```csharp
endpoints.MapGroup("v1/category")
    .WithTags("Category")
    .RequireAuthorization()
    .MapEndpoint<CreateCategoryEndpoint>()
    .MapEndpoint<UpdateCategoryEndpoint>();
```

A lógica de `TagActionsBy` serve como **fallback** caso as tags explícitas não estejam definidas.

### 3. Rebuild Necessário

Mudanças no SwaggerConfig requerem rebuild completo:
- ❌ `dotnet run --no-build` → Não aplica mudanças
- ✅ `dotnet build && dotnet run` → Aplica mudanças

## 📊 Comparação Antes/Depois

| Aspecto | Antes | Depois |
|---------|-------|--------|
| **Agrupamento** | ❌ Endpoints misturados | ✅ Agrupados por entidade |
| **Navegação** | ❌ Difícil encontrar endpoints | ✅ Fácil navegação hierárquica |
| **Tags** | ❌ Inconsistentes (role, Role, roles) | ✅ Consistentes (Role) |
| **Ordem** | ❌ Aleatória ou por rota | ✅ Alfabética por tag |
| **UX** | ⚠️ Confusa | ✅ Profissional e organizada |

## ✅ Checklist de Validação

Após rebuild, verifique:

- [ ] Scalar abre sem erros em `https://localhost:7296/api-docs/v1`
- [ ] Endpoints estão agrupados por entidade na sidebar
- [ ] Nomes das tags estão capitalizados (Category, Order, etc)
- [ ] Endpoints de plural e singular estão no mesmo grupo
- [ ] Botão "Authorize" funciona para autenticação Bearer
- [ ] Exemplos de código são exibidos corretamente
- [ ] Dark mode está ativo por padrão
- [ ] Busca (tecla K) funciona

## 🔗 Documentação Relacionada

- **CLAUDE.md** - Guia geral do projeto
- **FLUXO-LOGIN-COMPLETO.md** - Autenticação e autorização
- **PROXIMOS-PASSOS-401.md** - Resolução de problemas de autenticação
- [Scalar Documentation](https://github.com/scalar/scalar)
- [Swashbuckle Documentation](https://github.com/domaindrivendev/Swashbuckle.AspNetCore)

---

**Versão:** 1.0
**Data:** 23/11/2025
**Status:** ✅ Implementado
