# 📋 Plano de Implementação - Sistema de Chamados (e-Chamado)

Este documento contém o plano detalhado para implementar o sistema completo de gerenciamento de chamados.

---

## 📊 Status Atual do Projeto

### ✅ O que JÁ está implementado (70%)

#### **1. Camada de Domínio - COMPLETA**
- ✅ **Order (Chamado)**: Entidade principal com todas propriedades e métodos
- ✅ **Category/SubCategory**: Hierarquia de categorias
- ✅ **Department**: Departamentos
- ✅ **OrderType**: Tipos de chamado
- ✅ **StatusType**: Status dos chamados
- ✅ **Domain Events**: OrderCreated, OrderUpdated, OrderClosed
- ✅ **Identity**: ApplicationUser com integração ASP.NET Identity

**Arquivos:**
- `EChamado.Server.Domain/Domains/Orders/Order.cs`
- `EChamado.Server.Domain/Domains/Categories/Category.cs`
- `EChamado.Server.Domain/Domains/Categories/SubCategory.cs`
- `EChamado.Server.Domain/Domains/Departments/Department.cs`

#### **2. Banco de Dados - COMPLETO**
- ✅ Migrations aplicadas
- ✅ Schema completo (Orders, Categories, Departments, etc)
- ✅ Seed de dados (Admin, User padrão)
- ✅ Relacionamentos configurados

**Arquivo:**
- `EChamado.Server.Infrastructure/Persistence/Migrations/20241208152102_start.cs`

#### **3. Repositórios - COMPLETOS**
- ✅ OrderRepository
- ✅ CategoryRepository, SubCategoryRepository
- ✅ DepartmentRepository
- ✅ OrderTypeRepository, StatusTypeRepository

#### **4. Autenticação - COMPLETA**
- ✅ SSO com OpenIddict (Authorization Code + PKCE)
- ✅ Login/Registro funcionando
- ✅ Refresh Token
- ✅ CORS configurado

#### **5. CQRS Parcial**
- ✅ **Departments**: CreateDepartment, UpdateDepartment, DeleteDepartment, GetById, Search
- ✅ **Auth**: Login, Register, GetToken
- ✅ **Users**: GetAll, GetById, GetByEmail
- ✅ **Roles**: CRUD completo

---

### ❌ O que PRECISA ser implementado (30%)

#### **1. CQRS para Chamados (PRIORIDADE ALTA)**
- ❌ Commands: Create, Update, Close, Assign, ChangeStatus
- ❌ Queries: GetById, GetAll, Search, GetByUser, GetByDepartment, GetByStatus
- ❌ ViewModels: OrderViewModel, OrderListViewModel

#### **2. API Controllers (PRIORIDADE ALTA)**
- ❌ OrdersController
- ❌ CategoriesController
- ❌ DepartmentsController (expor endpoints existentes)
- ❌ OrderTypesController
- ❌ StatusTypesController

#### **3. Blazor Pages - Frontend (PRIORIDADE ALTA)**
- ❌ Dashboard (Home com estatísticas)
- ❌ Lista de Chamados (com filtros)
- ❌ Criar Chamado
- ❌ Editar Chamado
- ❌ Detalhes do Chamado
- ❌ Administração (Categories, Departments, Types)

#### **4. Serviços HTTP no Client (PRIORIDADE ALTA)**
- ❌ OrderService
- ❌ CategoryService
- ❌ DepartmentService
- ❌ LookupService (para dropdowns)

#### **5. Funcionalidades Avançadas (PRIORIDADE MÉDIA)**
- ❌ Workflow de status
- ❌ Atribuição automática
- ❌ SLA tracking
- ❌ Notificações

---

## 🎯 Plano de Implementação em Fases

### **FASE 1: Backend - CQRS e API** (Estimativa: 1-2 dias)

Esta fase foca em criar toda a lógica de negócios e expor via API.

#### **1.1 - CQRS para Orders (Chamados)**

**Localização:** `EChamado.Server.Application/UseCases/Orders/`

**Commands a criar:**

```
Commands/
├── CreateOrderCommand.cs
│   ├── OrderId Create(string title, string description, ...)
│   ├── Validações: título obrigatório, usuário solicitante válido
│   └── Dispara: OrderCreated event
│
├── UpdateOrderCommand.cs
│   ├── void Update(Guid orderId, string title, string description, ...)
│   ├── Validações: chamado existe, usuário tem permissão
│   └── Dispara: OrderUpdated event
│
├── CloseOrderCommand.cs
│   ├── void Close(Guid orderId, int evaluation)
│   ├── Validações: chamado existe, não está fechado
│   └── Dispara: OrderClosed event
│
├── AssignOrderCommand.cs
│   ├── void Assign(Guid orderId, Guid responsibleUserId)
│   ├── Validações: usuário existe, tem permissão
│   └── Atualiza ResponsibleUserId
│
└── ChangeStatusOrderCommand.cs
    ├── void ChangeStatus(Guid orderId, Guid statusId)
    ├── Validações: status válido, transição permitida
    └── Atualiza StatusId
```

**Queries a criar:**

```
Queries/
├── GetOrderByIdQuery.cs
│   └── OrderViewModel GetById(Guid orderId)
│
├── SearchOrdersQuery.cs
│   ├── Filtros: status, department, category, dateRange, userId
│   └── PagedResult<OrderListViewModel>
│
├── GetOrdersByUserQuery.cs
│   └── Lista chamados do usuário (solicitante ou responsável)
│
├── GetOrdersByDepartmentQuery.cs
│   └── Lista chamados por departamento
│
└── GetOrdersByStatusQuery.cs
    └── Lista chamados por status
```

**ViewModels a criar:**

```csharp
public record OrderViewModel(
    Guid Id,
    string Title,
    string Description,
    int? Evaluation,
    DateTime OpeningDate,
    DateTime? ClosingDate,
    DateTime? DueDate,
    Guid StatusId,
    string StatusName,
    Guid TypeId,
    string TypeName,
    Guid? CategoryId,
    string? CategoryName,
    Guid? SubCategoryId,
    string? SubCategoryName,
    Guid? DepartmentId,
    string? DepartmentName,
    Guid RequestingUserId,
    string RequestingUserEmail,
    Guid? ResponsibleUserId,
    string? ResponsibleUserEmail
);

public record OrderListViewModel(
    Guid Id,
    string Title,
    DateTime OpeningDate,
    DateTime? DueDate,
    string StatusName,
    string DepartmentName,
    string RequestingUserEmail
);

public record CreateOrderRequest(
    string Title,
    string Description,
    Guid TypeId,
    Guid? CategoryId,
    Guid? SubCategoryId,
    Guid? DepartmentId,
    DateTime? DueDate
);

public record UpdateOrderRequest(
    string Title,
    string Description,
    Guid? CategoryId,
    Guid? SubCategoryId,
    Guid? DepartmentId,
    DateTime? DueDate
);
```

**Handlers:**
- Cada Command/Query precisa de seu Handler
- Usar MediatR pattern já existente no projeto
- Incluir validações usando FluentValidation

---

#### **1.2 - CQRS para Categories e SubCategories**

**Localização:** `EChamado.Server.Application/UseCases/Categories/`

**Commands:**
- CreateCategoryCommand
- UpdateCategoryCommand
- DeleteCategoryCommand
- CreateSubCategoryCommand
- UpdateSubCategoryCommand
- DeleteSubCategoryCommand

**Queries:**
- GetCategoryByIdQuery
- GetAllCategoriesQuery (com SubCategories)
- GetSubCategoriesByCategoryIdQuery

**ViewModels:**
```csharp
public record CategoryViewModel(
    Guid Id,
    string Name,
    string Description,
    List<SubCategoryViewModel> SubCategories
);

public record SubCategoryViewModel(
    Guid Id,
    string Name,
    string Description,
    Guid CategoryId
);
```

---

#### **1.3 - CQRS para OrderTypes e StatusTypes**

**Localização:** `EChamado.Server.Application/UseCases/OrderTypes/` e `StatusTypes/`

**Commands:**
- CreateOrderTypeCommand, UpdateOrderTypeCommand, DeleteOrderTypeCommand
- CreateStatusTypeCommand, UpdateStatusTypeCommand, DeleteStatusTypeCommand

**Queries:**
- GetAllOrderTypesQuery
- GetAllStatusTypesQuery
- GetByIdQuery para cada

**ViewModels:**
```csharp
public record OrderTypeViewModel(Guid Id, string Name, string Description);
public record StatusTypeViewModel(Guid Id, string Name, string Description);
```

---

#### **1.4 - Criar Controllers**

**Localização:** `EChamado.Server/Controllers/`

**OrdersController.cs:**
```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize] // Requer autenticação
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateOrderCommand command)

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateOrderCommand command)

    [HttpPost("{id}/close")]
    public async Task<IActionResult> Close(Guid id, [FromBody] CloseOrderCommand command)

    [HttpPost("{id}/assign")]
    public async Task<IActionResult> Assign(Guid id, [FromBody] AssignOrderCommand command)

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderViewModel>> GetById(Guid id)

    [HttpGet]
    public async Task<ActionResult<PagedResult<OrderListViewModel>>> Search([FromQuery] SearchOrdersQuery query)

    [HttpGet("my-tickets")]
    public async Task<ActionResult<List<OrderListViewModel>>> GetMyTickets()
}
```

**CategoriesController.cs:**
```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")] // Apenas admins gerenciam categorias
public class CategoriesController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<CategoryViewModel>>> GetAll()

    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateCategoryCommand command)

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryCommand command)

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
}
```

**DepartmentsController.cs:**
**OrderTypesController.cs:**
**StatusTypesController.cs:**
(Seguir mesmo padrão)

---

### **FASE 2: Frontend - Serviços HTTP** (Estimativa: 1 dia)

Criar serviços que consomem a API no Client.

#### **2.1 - Criar HttpClient Services**

**Localização:** `EChamado.Client/Services/`

**OrderService.cs:**
```csharp
public class OrderService
{
    private readonly HttpClient _httpClient;

    public OrderService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Guid> CreateAsync(CreateOrderRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/orders", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Guid>();
    }

    public async Task UpdateAsync(Guid id, UpdateOrderRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/orders/{id}", request);
        response.EnsureSuccessStatusCode();
    }

    public async Task<OrderViewModel> GetByIdAsync(Guid id)
    {
        return await _httpClient.GetFromJsonAsync<OrderViewModel>($"api/orders/{id}");
    }

    public async Task<PagedResult<OrderListViewModel>> SearchAsync(SearchParameters parameters)
    {
        var query = BuildQueryString(parameters);
        return await _httpClient.GetFromJsonAsync<PagedResult<OrderListViewModel>>($"api/orders?{query}");
    }

    public async Task CloseAsync(Guid id, int evaluation)
    {
        var response = await _httpClient.PostAsJsonAsync($"api/orders/{id}/close", new { evaluation });
        response.EnsureSuccessStatusCode();
    }

    public async Task AssignAsync(Guid id, Guid responsibleUserId)
    {
        var response = await _httpClient.PostAsJsonAsync($"api/orders/{id}/assign", new { responsibleUserId });
        response.EnsureSuccessStatusCode();
    }
}
```

**CategoryService.cs:**
**DepartmentService.cs:**
**LookupService.cs:** (para popular dropdowns)

#### **2.2 - Registrar Serviços**

**Program.cs:**
```csharp
builder.Services.AddHttpClient<OrderService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["BackendUrl"]);
})
.AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

builder.Services.AddHttpClient<CategoryService>(...)
builder.Services.AddHttpClient<DepartmentService>(...)
builder.Services.AddHttpClient<LookupService>(...)
```

---

### **FASE 3: Frontend - Páginas Blazor** (Estimativa: 2-3 dias)

Criar todas as páginas usando MudBlazor.

#### **3.1 - Dashboard (Home)**

**Localização:** `EChamado.Client/Pages/Dashboard.razor`

**Funcionalidades:**
- Cards com estatísticas:
  - Total de chamados abertos
  - Chamados atribuídos a mim
  - Chamados vencidos
  - Chamados fechados (mês atual)
- Gráfico de chamados por status (MudChart)
- Gráfico de chamados por departamento
- Lista dos últimos 5 chamados criados
- Atalhos rápidos (Criar Chamado, Ver Meus Chamados)

**Componentes MudBlazor:**
- MudCard, MudCardContent
- MudChart (Donut, Bar)
- MudTable (resumida)
- MudButton

---

#### **3.2 - Lista de Chamados**

**Localização:** `EChamado.Client/Pages/Orders/OrderList.razor`

**Funcionalidades:**
- MudTable com paginação
- Colunas: ID, Título, Status, Departamento, Solicitante, Data Abertura, Prazo
- Filtros:
  - Status (dropdown múltiplo)
  - Departamento (dropdown)
  - Categoria (dropdown)
  - Data (range picker)
  - Texto livre (busca)
- Ações por linha:
  - Ver detalhes (ícone olho)
  - Editar (ícone lápis, se permitido)
  - Atribuir (ícone pessoa)
- Botão "Novo Chamado" no topo
- Badges coloridos para status

**Componentes MudBlazor:**
- MudTable com ServerData
- MudTextField para busca
- MudSelect para filtros
- MudDateRangePicker
- MudIconButton
- MudChip para status

**Código exemplo:**
```razor
@page "/orders"
@inject OrderService OrderService
@attribute [Authorize]

<MudContainer MaxWidth="MaxWidth.ExtraExtraLarge" Class="mt-4">
    <MudText Typo="Typo.h4" Class="mb-4">Chamados</MudText>

    <MudPaper Class="pa-4 mb-4">
        <MudGrid>
            <MudItem xs="12" md="3">
                <MudTextField @bind-Value="searchText"
                              Label="Buscar"
                              Variant="Variant.Outlined"
                              Adornment="Adornment.Start"
                              AdornmentIcon="@Icons.Material.Filled.Search" />
            </MudItem>
            <MudItem xs="12" md="3">
                <MudSelect @bind-Value="selectedStatus"
                           Label="Status"
                           Variant="Variant.Outlined">
                    @foreach (var status in statuses)
                    {
                        <MudSelectItem Value="status.Id">@status.Name</MudSelectItem>
                    }
                </MudSelect>
            </MudItem>
            <MudItem xs="12" md="2">
                <MudButton Variant="Variant.Filled"
                           Color="Color.Primary"
                           OnClick="Search">
                    Filtrar
                </MudButton>
            </MudItem>
        </MudGrid>
    </MudPaper>

    <MudTable ServerData="@(new Func<TableState, Task<TableData<OrderListViewModel>>>(ServerReload))"
              Hover="true"
              @ref="table">
        <ToolBarContent>
            <MudSpacer />
            <MudButton Variant="Variant.Filled"
                       Color="Color.Primary"
                       StartIcon="@Icons.Material.Filled.Add"
                       Href="/orders/create">
                Novo Chamado
            </MudButton>
        </ToolBarContent>
        <HeaderContent>
            <MudTh>Título</MudTh>
            <MudTh>Status</MudTh>
            <MudTh>Departamento</MudTh>
            <MudTh>Solicitante</MudTh>
            <MudTh>Data Abertura</MudTh>
            <MudTh>Ações</MudTh>
        </HeaderContent>
        <RowTemplate>
            <MudTd DataLabel="Título">@context.Title</MudTd>
            <MudTd DataLabel="Status">
                <MudChip Color="GetStatusColor(context.StatusName)">
                    @context.StatusName
                </MudChip>
            </MudTd>
            <MudTd DataLabel="Departamento">@context.DepartmentName</MudTd>
            <MudTd DataLabel="Solicitante">@context.RequestingUserEmail</MudTd>
            <MudTd DataLabel="Data">@context.OpeningDate.ToString("dd/MM/yyyy")</MudTd>
            <MudTd>
                <MudIconButton Icon="@Icons.Material.Filled.Visibility"
                               Size="Size.Small"
                               Href="@($"/orders/{context.Id}")" />
                <MudIconButton Icon="@Icons.Material.Filled.Edit"
                               Size="Size.Small"
                               Href="@($"/orders/{context.Id}/edit")" />
            </MudTd>
        </RowTemplate>
        <PagerContent>
            <MudTablePager />
        </PagerContent>
    </MudTable>
</MudContainer>

@code {
    private MudTable<OrderListViewModel> table;
    private string searchText = "";
    private Guid? selectedStatus;
    private List<StatusTypeViewModel> statuses = new();

    private async Task<TableData<OrderListViewModel>> ServerReload(TableState state)
    {
        var result = await OrderService.SearchAsync(new SearchParameters
        {
            Page = state.Page,
            PageSize = state.PageSize,
            SearchText = searchText,
            StatusId = selectedStatus
        });

        return new TableData<OrderListViewModel>
        {
            TotalItems = result.TotalCount,
            Items = result.Items
        };
    }

    private Color GetStatusColor(string status) => status switch
    {
        "Aberto" => Color.Info,
        "Em Andamento" => Color.Warning,
        "Fechado" => Color.Success,
        _ => Color.Default
    };
}
```

---

#### **3.3 - Criar/Editar Chamado**

**Localização:** `EChamado.Client/Pages/Orders/OrderForm.razor`

**Funcionalidades:**
- Formulário com validação
- Campos:
  - Título (required)
  - Descrição (textarea, required)
  - Tipo (dropdown, required)
  - Categoria (dropdown, optional)
  - SubCategoria (dropdown, optional, filtra por categoria)
  - Departamento (dropdown, optional)
  - Prazo (date picker, optional)
- Botões: Salvar, Cancelar
- Feedback visual de erros

**Componentes MudBlazor:**
- MudForm com validação
- MudTextField
- MudSelect com cascata (Category → SubCategory)
- MudDatePicker
- MudButton

---

#### **3.4 - Detalhes do Chamado**

**Localização:** `EChamado.Client/Pages/Orders/OrderDetails.razor`

**Funcionalidades:**
- Visualização completa do chamado
- Informações em cards:
  - Dados principais (título, descrição, status)
  - Datas (abertura, prazo, fechamento)
  - Classificação (tipo, categoria, departamento)
  - Pessoas (solicitante, responsável)
  - Avaliação (se fechado)
- Ações disponíveis (se permitido):
  - Editar
  - Atribuir para mim
  - Mudar status
  - Fechar chamado
- Timeline de atividades (futuro)

**Componentes MudBlazor:**
- MudCard para cada seção
- MudDivider
- MudChip para status
- MudRating (para avaliação)
- MudButton para ações
- MudDialog para confirmações

---

#### **3.5 - Administração**

**Páginas:**

1. **Categories/CategoryList.razor**
   - CRUD de categorias
   - Expansão para mostrar subcategorias
   - MudExpansionPanel

2. **Departments/DepartmentList.razor**
   - CRUD de departamentos
   - MudTable simples

3. **Admin/OrderTypes.razor**
   - CRUD de tipos de chamado
   - MudTable simples

4. **Admin/StatusTypes.razor**
   - CRUD de status
   - MudTable simples

---

### **FASE 4: Melhorias e Polimento** (Estimativa: 1-2 dias)

#### **4.1 - Navegação e Layout**

**Atualizar NavMenu:**
```razor
<MudNavMenu>
    <MudNavLink Href="/" Icon="@Icons.Material.Filled.Dashboard">Dashboard</MudNavLink>
    <MudNavLink Href="/orders" Icon="@Icons.Material.Filled.ConfirmationNumber">Chamados</MudNavLink>
    <MudNavLink Href="/orders/create" Icon="@Icons.Material.Filled.Add">Novo Chamado</MudNavLink>

    <MudNavGroup Title="Administração" Icon="@Icons.Material.Filled.Settings" Expanded="false">
        <MudNavLink Href="/admin/departments" Icon="@Icons.Material.Filled.Business">Departamentos</MudNavLink>
        <MudNavLink Href="/admin/categories" Icon="@Icons.Material.Filled.Category">Categorias</MudNavLink>
        <MudNavLink Href="/admin/types" Icon="@Icons.Material.Filled.Label">Tipos</MudNavLink>
        <MudNavLink Href="/admin/status" Icon="@Icons.Material.Filled.Flag">Status</MudNavLink>
    </MudNavGroup>
</MudNavMenu>
```

#### **4.2 - Tratamento de Erros**

- Criar componente ErrorBoundary
- Adicionar try/catch nos serviços
- Mostrar Snackbar do MudBlazor para erros
- Logging de erros

#### **4.3 - Loading States**

- MudProgressCircular durante carregamento
- Skeleton loaders para tabelas
- Disable buttons durante submit

#### **4.4 - Validações**

- FluentValidation no backend
- DataAnnotations nos ViewModels
- Validação em tempo real no frontend

---

## 🗂️ Estrutura de Pastas Final

```
EChamado/
├── Server/
│   ├── EChamado.Server/
│   │   └── Controllers/
│   │       ├── AuthorizationController.cs ✅
│   │       ├── OrdersController.cs ❌ CRIAR
│   │       ├── CategoriesController.cs ❌ CRIAR
│   │       ├── DepartmentsController.cs ❌ CRIAR
│   │       ├── OrderTypesController.cs ❌ CRIAR
│   │       └── StatusTypesController.cs ❌ CRIAR
│   │
│   ├── EChamado.Server.Application/
│   │   └── UseCases/
│   │       ├── Orders/ ❌ CRIAR COMPLETO
│   │       │   ├── Commands/
│   │       │   │   ├── CreateOrderCommand.cs
│   │       │   │   ├── UpdateOrderCommand.cs
│   │       │   │   ├── CloseOrderCommand.cs
│   │       │   │   └── AssignOrderCommand.cs
│   │       │   ├── Queries/
│   │       │   │   ├── GetOrderByIdQuery.cs
│   │       │   │   └── SearchOrdersQuery.cs
│   │       │   └── ViewModels/
│   │       │       └── OrderViewModel.cs
│   │       │
│   │       ├── Categories/ ❌ CRIAR
│   │       ├── OrderTypes/ ❌ CRIAR
│   │       ├── StatusTypes/ ❌ CRIAR
│   │       └── Departments/ ✅ (expor via controller)
│   │
│   └── EChamado.Server.Domain/ ✅ COMPLETO
│
└── Client/
    └── EChamado.Client/
        ├── Services/ ❌ CRIAR
        │   ├── OrderService.cs
        │   ├── CategoryService.cs
        │   ├── DepartmentService.cs
        │   └── LookupService.cs
        │
        └── Pages/
            ├── Dashboard.razor ❌ CRIAR
            ├── Orders/
            │   ├── OrderList.razor ❌ CRIAR
            │   ├── OrderForm.razor ❌ CRIAR
            │   └── OrderDetails.razor ❌ CRIAR
            │
            └── Admin/
                ├── Categories.razor ❌ CRIAR
                ├── Departments.razor ❌ CRIAR
                ├── OrderTypes.razor ❌ CRIAR
                └── StatusTypes.razor ❌ CRIAR
```

---

## 📅 Cronograma Sugerido

| Fase | Tarefas | Estimativa | Prioridade |
|------|---------|------------|------------|
| **FASE 1** | Backend - CQRS e API | 1-2 dias | 🔴 ALTA |
| **FASE 2** | Frontend - Serviços HTTP | 1 dia | 🔴 ALTA |
| **FASE 3** | Frontend - Páginas Blazor | 2-3 dias | 🔴 ALTA |
| **FASE 4** | Melhorias e Polimento | 1-2 dias | 🟡 MÉDIA |
| **TOTAL** | | **5-8 dias** | |

---

## 🎯 Ordem de Implementação Recomendada

### **Dia 1-2: Backend Core**
1. ✅ CQRS para Orders (Commands + Queries + ViewModels)
2. ✅ OrdersController
3. ✅ CQRS para Categories/SubCategories
4. ✅ CategoriesController

### **Dia 3: Backend Auxiliar + Frontend Services**
5. ✅ CQRS para OrderTypes e StatusTypes
6. ✅ Controllers auxiliares (Types, Status, Departments)
7. ✅ HttpClient Services no Client

### **Dia 4-5: Frontend Principal**
8. ✅ Dashboard
9. ✅ OrderList
10. ✅ OrderForm (Create/Edit)
11. ✅ OrderDetails

### **Dia 6-7: Frontend Admin**
12. ✅ Categories Management
13. ✅ Departments Management
14. ✅ Types/Status Management

### **Dia 8: Polimento**
15. ✅ Navegação e layout
16. ✅ Tratamento de erros
17. ✅ Loading states
18. ✅ Validações
19. ✅ Testes end-to-end

---

## 🧪 Checklist de Testes

### **Backend**
- [ ] Criar chamado com dados válidos
- [ ] Validação de campos obrigatórios
- [ ] Atualizar chamado existente
- [ ] Fechar chamado com avaliação
- [ ] Atribuir chamado a usuário
- [ ] Buscar chamados com filtros
- [ ] Permissões (usuário só edita seus chamados)
- [ ] CRUD de categorias
- [ ] CRUD de departamentos

### **Frontend**
- [ ] Login e navegação
- [ ] Dashboard carrega estatísticas
- [ ] Lista de chamados com paginação
- [ ] Filtros funcionam corretamente
- [ ] Criar novo chamado
- [ ] Editar chamado existente
- [ ] Visualizar detalhes
- [ ] Cascata Category → SubCategory
- [ ] Responsividade mobile
- [ ] Mensagens de erro amigáveis

---

## 📚 Recursos e Referências

### **MudBlazor**
- Documentação: https://mudblazor.com/
- Componentes úteis:
  - MudTable: https://mudblazor.com/components/table
  - MudForm: https://mudblazor.com/components/form
  - MudDialog: https://mudblazor.com/components/dialog
  - MudChart: https://mudblazor.com/components/chart
  - MudDatePicker: https://mudblazor.com/components/datepicker

### **MediatR**
- Pattern CQRS: https://github.com/jbogard/MediatR

### **FluentValidation**
- Validações: https://docs.fluentvalidation.net/

---

## ✅ Próximo Passo

**Começar pela FASE 1 - Item 1.1: CQRS para Orders**

Criar a seguinte estrutura:
```
EChamado.Server.Application/UseCases/Orders/
├── Commands/
│   ├── CreateOrderCommand.cs
│   ├── CreateOrderCommandHandler.cs
│   └── CreateOrderCommandValidator.cs
├── Queries/
│   ├── GetOrderByIdQuery.cs
│   └── GetOrderByIdQueryHandler.cs
└── ViewModels/
    └── OrderViewModel.cs
```

Posso começar a implementar agora se você quiser! 🚀
