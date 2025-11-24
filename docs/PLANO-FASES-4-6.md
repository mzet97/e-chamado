# PLANO DE AÇÃO - FASES 4, 5 e 6
## EChamado - Caminho para Produção

**Período Estimado Total**: 2-3 semanas
**Status Atual**: 75-80% completo (FASES 1-3 concluídas)
**Objetivo**: Levar o sistema a 95-100% production-ready

---

## 📊 VISÃO GERAL DAS FASES

| Fase | Nome | Duração | Objetivo | Prioridade |
|------|------|---------|----------|-----------|
| **FASE 4** | Interface Completa | 5-6 dias | Completar UI Admin + Comments | 🔴 Crítico |
| **FASE 5** | Monitoramento | 1-2 dias | Health Checks + Observabilidade | 🔴 Crítico |
| **FASE 6** | Qualidade & CI/CD | 1 semana | Testes + Automação | 🔴 Crítico |

**Total**: 11-15 dias úteis (2-3 semanas)

---

## 🎯 FASE 4: INTERFACE COMPLETA
**Duração**: 5-6 dias
**Prioridade**: 🔴 Crítico

### Objetivos
- ✅ Completar 100% das interfaces administrativas
- ✅ Implementar sistema de comentários end-to-end
- ✅ Tornar o sistema 100% utilizável por usuários finais

### Tarefas Detalhadas

#### 4.1 Comments API (Backend) - 1 dia
**Arquivos a criar/modificar:**

1. **Domain Layer**
   - `Server/EChamado.Server.Domain/Entities/Comment.cs` (CRIAR)
   - `Server/EChamado.Server.Domain/Events/CommentAddedEvent.cs` (CRIAR)

2. **Application Layer - Commands**
   - `Server/EChamado.Server.Application/UseCases/Comments/Commands/AddCommentCommand.cs` (CRIAR)
   - `Server/EChamado.Server.Application/UseCases/Comments/Commands/AddCommentCommandHandler.cs` (CRIAR)
   - `Server/EChamado.Server.Application/UseCases/Comments/Commands/DeleteCommentCommand.cs` (CRIAR)
   - `Server/EChamado.Server.Application/UseCases/Comments/Commands/DeleteCommentCommandHandler.cs` (CRIAR)

3. **Application Layer - Queries**
   - `Server/EChamado.Server.Application/UseCases/Comments/Queries/GetCommentsByOrderIdQuery.cs` (CRIAR)
   - `Server/EChamado.Server.Application/UseCases/Comments/Queries/GetCommentsByOrderIdQueryHandler.cs` (CRIAR)

4. **Application Layer - ViewModels**
   - `Server/EChamado.Server.Application/UseCases/Comments/ViewModels/CommentViewModel.cs` (CRIAR)

5. **Infrastructure Layer**
   - `Server/EChamado.Server.Infrastructure/Persistence/Configurations/CommentConfiguration.cs` (CRIAR)
   - `Server/EChamado.Server.Infrastructure/Persistence/ApplicationDbContext.cs` (MODIFICAR - adicionar DbSet)
   - `Server/EChamado.Server.Infrastructure/Repositories/CommentRepository.cs` (CRIAR)

6. **API Layer**
   - `Server/EChamado.Server/Controllers/CommentsController.cs` (CRIAR)

7. **Migration**
   - Executar: `dotnet ef migrations add AddComments`

**Endpoints a criar:**
```
POST   /api/orders/{orderId}/comments    - Adicionar comentário
GET    /api/orders/{orderId}/comments    - Listar comentários
DELETE /api/comments/{id}                 - Deletar comentário (apenas autor ou admin)
```

**Critérios de Aceitação:**
- [ ] Entidade Comment criada com validações
- [ ] CQRS completo (Commands + Queries + Handlers)
- [ ] Migration aplicada no banco
- [ ] Controller com 3 endpoints funcionais
- [ ] Validação: apenas autor ou admin pode deletar
- [ ] Testes manuais via Swagger

---

#### 4.2 Admin - Categories Page - 1 dia
**Arquivo a criar:**
- `Client/EChamado.Client/Pages/Admin/Categories.razor` (CRIAR)

**Funcionalidades:**
- MudExpansionPanels para categorias
- Cada painel expande mostrando subcategorias
- Botões: Adicionar Categoria, Adicionar Subcategoria
- Dialog para criar/editar categoria
- Dialog para criar/editar subcategoria
- Botões de ação: Editar, Deletar (com confirmação)
- Suporte a ícones nas categorias

**Componentes MudBlazor:**
- MudExpansionPanels
- MudTable (para subcategorias)
- MudDialog
- MudForm
- MudTextField
- MudButton

**Estimativa**: ~350 linhas de código

**Critérios de Aceitação:**
- [ ] Listagem hierárquica de categorias/subcategorias
- [ ] CRUD completo de categorias
- [ ] CRUD completo de subcategorias
- [ ] Validação de formulários
- [ ] Confirmação antes de deletar
- [ ] Mensagens de sucesso/erro com Snackbar

---

#### 4.3 Admin - Departments Page - 1 dia
**Arquivo a criar:**
- `Client/EChamado.Client/Pages/Admin/Departments.razor` (CRIAR)

**Funcionalidades:**
- MudTable com paginação
- Colunas: Nome, Descrição, Ativo, Ações
- Botão: Adicionar Departamento
- Dialog para criar/editar
- Ações inline: Editar, Ativar/Desativar, Deletar
- Filtro por status (Ativo/Inativo)
- Busca por nome

**Componentes MudBlazor:**
- MudTable com paginação server-side
- MudDialog
- MudForm
- MudSwitch (para ativo/inativo)
- MudChip (status)

**Estimativa**: ~300 linhas de código

**Critérios de Aceitação:**
- [ ] Listagem com paginação
- [ ] CRUD completo
- [ ] Toggle ativo/inativo funcional
- [ ] Filtros funcionais
- [ ] Validação de formulários
- [ ] Confirmação antes de deletar

---

#### 4.4 Admin - Order Types Page - 1 dia
**Arquivo a criar:**
- `Client/EChamado.Client/Pages/Admin/OrderTypes.razor` (CRIAR)

**Funcionalidades:**
- MudTable simples
- Colunas: Nome, Descrição, Cor (chip), Ícone, Ações
- Botão: Adicionar Tipo
- Dialog para criar/editar
- Seletor de cor (MudColorPicker)
- Seletor de ícone (lista Material Icons)

**Componentes MudBlazor:**
- MudTable
- MudDialog
- MudColorPicker
- MudSelect (ícones)
- MudChip (preview da cor)

**Estimativa**: ~280 linhas de código

**Critérios de Aceitação:**
- [ ] CRUD completo
- [ ] Color picker funcional
- [ ] Preview visual da cor
- [ ] Validação de formulários

---

#### 4.5 Admin - Status Types Page - 1 dia
**Arquivo a criar:**
- `Client/EChamado.Client/Pages/Admin/StatusTypes.razor` (CRIAR)

**Funcionalidades:**
- MudTable com drag-and-drop para ordenação
- Colunas: Ordem, Nome, Cor, Tipo (Inicial/Intermediário/Final), Ações
- Botão: Adicionar Status
- Dialog para criar/editar
- Reordenação visual (drag & drop)
- Categorização: Inicial, Em Progresso, Fechado

**Componentes MudBlazor:**
- MudTable
- MudDropContainer (drag & drop)
- MudDialog
- MudColorPicker
- MudSelect (tipo)
- MudChip

**Estimativa**: ~320 linhas de código

**Critérios de Aceitação:**
- [ ] CRUD completo
- [ ] Drag & drop para reordenar
- [ ] Categorização por tipo
- [ ] Color picker funcional
- [ ] Validação: pelo menos 1 status inicial e 1 final

---

#### 4.6 Integração Frontend Comments - 0.5 dia
**Arquivo a modificar:**
- `Client/EChamado.Client/Pages/Orders/OrderDetails.razor` (MODIFICAR)

**Mudanças:**
- Conectar ao endpoint real de comments
- Implementar delete de comentário (se for autor)
- Atualizar lista após adicionar/deletar
- Loading states

**Critérios de Aceitação:**
- [ ] Adicionar comentário funcional
- [ ] Listar comentários do backend
- [ ] Deletar comentário (se autor ou admin)
- [ ] UI atualiza em tempo real

---

### Resumo FASE 4

**Arquivos a criar**: 20 novos arquivos
**Arquivos a modificar**: 3 arquivos
**Linhas de código estimadas**: ~2.500 linhas
**Endpoints novos**: 3 endpoints

**Checklist Final FASE 4:**
- [ ] Comments API completa (backend + frontend)
- [ ] Admin/Categories.razor completa
- [ ] Admin/Departments.razor completa
- [ ] Admin/OrderTypes.razor completa
- [ ] Admin/StatusTypes.razor completa
- [ ] Migration aplicada
- [ ] Testes manuais de todas as páginas
- [ ] Commit e push

---

## 🏥 FASE 5: MONITORAMENTO E OBSERVABILIDADE
**Duração**: 1-2 dias
**Prioridade**: 🔴 Crítico

### Objetivos
- ✅ Implementar Health Checks para todos os serviços
- ✅ Configurar endpoints de monitoramento
- ✅ Preparar para deploy em Kubernetes/Docker Swarm
- ✅ Integrar com ferramentas de APM (Application Performance Monitoring)

### Tarefas Detalhadas

#### 5.1 Health Checks - Backend - 1 dia

**Pacotes NuGet a adicionar:**
```xml
<PackageReference Include="AspNetCore.HealthChecks.UI" Version="8.0.2" />
<PackageReference Include="AspNetCore.HealthChecks.UI.Client" Version="8.0.1" />
<PackageReference Include="AspNetCore.HealthChecks.UI.InMemory.Storage" Version="8.0.1" />
<PackageReference Include="AspNetCore.HealthChecks.Npgsql" Version="8.0.2" />
<PackageReference Include="AspNetCore.HealthChecks.Redis" Version="8.0.1" />
<PackageReference Include="AspNetCore.HealthChecks.RabbitMQ" Version="8.0.2" />
```

**Arquivos a criar/modificar:**

1. **Health Check Classes**
   - `Server/EChamado.Server/HealthChecks/DatabaseHealthCheck.cs` (CRIAR)
   - `Server/EChamado.Server/HealthChecks/RedisHealthCheck.cs` (CRIAR)
   - `Server/EChamado.Server/HealthChecks/RabbitMQHealthCheck.cs` (CRIAR)

2. **Configuration**
   - `Server/EChamado.Server/Program.cs` (MODIFICAR)

3. **UI Health Check**
   - `Server/EChamado.Server/Pages/HealthUI.razor` (CRIAR - opcional)

**Endpoints a criar:**
```
GET /health              - Health check geral (retorna JSON)
GET /health/ready        - Readiness probe (Kubernetes)
GET /health/live         - Liveness probe (Kubernetes)
GET /health-ui           - UI visual (opcional)
```

**Implementação em Program.cs:**
```csharp
// Health Checks
builder.Services.AddHealthChecks()
    .AddNpgSql(
        connectionString: builder.Configuration.GetConnectionString("DefaultConnection")!,
        name: "postgresql",
        failureStatus: HealthStatus.Unhealthy,
        tags: new[] { "db", "sql", "postgresql" })
    .AddRedis(
        redisConnectionString: builder.Configuration.GetConnectionString("Redis")!,
        name: "redis",
        failureStatus: HealthStatus.Unhealthy,
        tags: new[] { "cache", "redis" })
    .AddRabbitMQ(
        rabbitConnectionString: builder.Configuration.GetConnectionString("RabbitMQ")!,
        name: "rabbitmq",
        failureStatus: HealthStatus.Unhealthy,
        tags: new[] { "messaging", "rabbitmq" });

// Health Checks UI (opcional - para visualização)
builder.Services
    .AddHealthChecksUI(settings =>
    {
        settings.SetEvaluationTimeInSeconds(10);
        settings.MaximumHistoryEntriesPerEndpoint(50);
    })
    .AddInMemoryStorage();

// Endpoints
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready"),
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false, // Apenas verifica se a aplicação está rodando
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecksUI(options => options.UIPath = "/health-ui");
```

**Resposta esperada** (`/health`):
```json
{
  "status": "Healthy",
  "totalDuration": "00:00:00.0123456",
  "entries": {
    "postgresql": {
      "status": "Healthy",
      "duration": "00:00:00.0056789"
    },
    "redis": {
      "status": "Healthy",
      "duration": "00:00:00.0034567"
    },
    "rabbitmq": {
      "status": "Healthy",
      "duration": "00:00:00.0045678"
    }
  }
}
```

**Critérios de Aceitação:**
- [ ] Health checks para PostgreSQL, Redis, RabbitMQ
- [ ] Endpoint `/health` retorna status geral
- [ ] Endpoint `/health/ready` para readiness
- [ ] Endpoint `/health/live` para liveness
- [ ] Respostas em formato JSON padronizado
- [ ] Testes com serviços online e offline

---

#### 5.2 Docker Health Checks - 0.5 dia

**Arquivo a modificar:**
- `docker-compose.yml` (MODIFICAR)

**Adicionar health checks aos serviços:**
```yaml
services:
  echamado-server:
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:5001/health"]
      interval: 30s
      timeout: 10s
      retries: 3
      start_period: 40s

  echamado-client:
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:5002/"]
      interval: 30s
      timeout: 10s
      retries: 3
      start_period: 40s

  postgres:
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U echamado"]
      interval: 10s
      timeout: 5s
      retries: 5

  redis:
    healthcheck:
      test: ["CMD", "redis-cli", "ping"]
      interval: 10s
      timeout: 3s
      retries: 3

  rabbitmq:
    healthcheck:
      test: ["CMD", "rabbitmq-diagnostics", "ping"]
      interval: 30s
      timeout: 10s
      retries: 3
```

**Critérios de Aceitação:**
- [ ] Health checks em todos os serviços Docker
- [ ] `docker ps` mostra status "healthy"
- [ ] Containers reiniciam automaticamente se unhealthy

---

#### 5.3 Logging Enhancements - 0.5 dia

**Arquivo a modificar:**
- `Server/EChamado.Server/Program.cs` (MODIFICAR)

**Adicionar:**
1. **Request Logging Middleware**
2. **Performance Logging**
3. **Exception Logging Global**

```csharp
// Middleware para logar requisições
app.UseMiddleware<RequestLoggingMiddleware>();

// Middleware para logar performance
app.UseMiddleware<PerformanceLoggingMiddleware>();
```

**Arquivos a criar:**
- `Server/EChamado.Server/Middlewares/RequestLoggingMiddleware.cs` (CRIAR)
- `Server/EChamado.Server/Middlewares/PerformanceLoggingMiddleware.cs` (CRIAR)

**Critérios de Aceitação:**
- [ ] Todas as requisições HTTP logadas
- [ ] Tempo de resposta logado
- [ ] Exceptions globais capturadas e logadas
- [ ] Logs estruturados (JSON) no ELK

---

### Resumo FASE 5

**Pacotes NuGet**: 6 novos
**Arquivos a criar**: 5 novos arquivos
**Arquivos a modificar**: 2 arquivos
**Endpoints novos**: 4 endpoints
**Linhas de código estimadas**: ~400 linhas

**Checklist Final FASE 5:**
- [ ] Health checks implementados
- [ ] Endpoints `/health`, `/health/ready`, `/health/live` funcionais
- [ ] Docker health checks configurados
- [ ] Request logging middleware
- [ ] Performance logging middleware
- [ ] Testes de health checks
- [ ] Commit e push

---

## 🧪 FASE 6: QUALIDADE & CI/CD
**Duração**: 6-8 dias
**Prioridade**: 🔴 Crítico

### Objetivos
- ✅ Implementar testes automatizados (Unit + Integration)
- ✅ Configurar CI/CD com GitHub Actions
- ✅ Atingir >70% code coverage
- ✅ Automatizar build, test, e deploy

### Tarefas Detalhadas

#### 6.1 Estrutura de Testes - 1 dia

**Projetos a criar:**
```bash
dotnet new xunit -n EChamado.Server.UnitTests -o tests/EChamado.Server.UnitTests
dotnet new xunit -n EChamado.Server.IntegrationTests -o tests/EChamado.Server.IntegrationTests
```

**Pacotes NuGet a adicionar:**
```xml
<!-- UnitTests -->
<PackageReference Include="xUnit" Version="2.8.1" />
<PackageReference Include="xUnit.runner.visualstudio" Version="2.8.1" />
<PackageReference Include="FluentAssertions" Version="6.12.0" />
<PackageReference Include="Moq" Version="4.20.70" />
<PackageReference Include="AutoFixture" Version="4.18.1" />
<PackageReference Include="AutoFixture.Xunit2" Version="4.18.1" />

<!-- IntegrationTests -->
<PackageReference Include="Microsoft.AspNetCore.Mvc.Testing" Version="9.0.0" />
<PackageReference Include="Testcontainers" Version="3.9.0" />
<PackageReference Include="Testcontainers.PostgreSql" Version="3.9.0" />
```

**Estrutura de pastas:**
```
tests/
├── EChamado.Server.UnitTests/
│   ├── Application/
│   │   ├── Commands/
│   │   │   ├── CreateOrderCommandHandlerTests.cs
│   │   │   ├── UpdateOrderCommandHandlerTests.cs
│   │   │   └── ...
│   │   ├── Queries/
│   │   │   ├── GetOrderByIdQueryHandlerTests.cs
│   │   │   └── ...
│   │   └── Validators/
│   │       ├── CreateOrderCommandValidatorTests.cs
│   │       └── ...
│   ├── Domain/
│   │   ├── Entities/
│   │   │   ├── OrderTests.cs
│   │   │   └── ...
│   │   └── ValueObjects/
│   └── Fixtures/
│       └── AutoMoqDataAttribute.cs
└── EChamado.Server.IntegrationTests/
    ├── Controllers/
    │   ├── OrdersControllerTests.cs
    │   ├── CategoriesControllerTests.cs
    │   └── ...
    ├── Fixtures/
    │   ├── WebApplicationFactory.cs
    │   └── DatabaseFixture.cs
    └── Infrastructure/
        └── Repositories/
            └── OrderRepositoryTests.cs
```

**Critérios de Aceitação:**
- [ ] Projetos de teste criados
- [ ] Pacotes NuGet instalados
- [ ] Estrutura de pastas organizada
- [ ] Base classes e fixtures criadas

---

#### 6.2 Unit Tests - Handlers - 2 dias

**Testes a criar (mínimo 20 testes):**

1. **CreateOrderCommandHandlerTests.cs**
   - ✅ Should_Create_Order_When_Valid_Command
   - ✅ Should_Throw_Exception_When_Invalid_UserId
   - ✅ Should_Throw_Exception_When_Invalid_TypeId
   - ✅ Should_Publish_OrderCreated_Event

2. **UpdateOrderCommandHandlerTests.cs**
   - ✅ Should_Update_Order_When_Valid_Command
   - ✅ Should_Throw_Exception_When_Order_Not_Found
   - ✅ Should_Publish_OrderUpdated_Event

3. **GetOrderByIdQueryHandlerTests.cs**
   - ✅ Should_Return_Order_When_Exists
   - ✅ Should_Return_Null_When_Not_Exists

4. **SearchOrdersQueryHandlerTests.cs**
   - ✅ Should_Return_Paged_Results
   - ✅ Should_Filter_By_Status
   - ✅ Should_Filter_By_Department
   - ✅ Should_Search_By_Text

**Exemplo de teste:**
```csharp
public class CreateOrderCommandHandlerTests
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly CreateOrderCommandHandler _handler;

    public CreateOrderCommandHandlerTests()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _mediatorMock = new Mock<IMediator>();
        _handler = new CreateOrderCommandHandler(
            _orderRepositoryMock.Object,
            _userRepositoryMock.Object,
            _mediatorMock.Object
        );
    }

    [Fact]
    public async Task Should_Create_Order_When_Valid_Command()
    {
        // Arrange
        var command = new CreateOrderCommand(
            Title: "Test Order",
            Description: "Test Description",
            TypeId: Guid.NewGuid(),
            CategoryId: null,
            SubCategoryId: null,
            DepartmentId: null,
            DueDate: null,
            RequestingUserId: Guid.NewGuid(),
            RequestingUserEmail: "test@test.com"
        );

        var user = new ApplicationUser { Id = command.RequestingUserId.ToString() };
        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(command.RequestingUserId, default))
            .ReturnsAsync(user);

        _orderRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Order>(), default))
            .ReturnsAsync((Order o, CancellationToken _) => o);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.Should().NotBeEmpty();
        _orderRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Order>(), default), Times.Once);
        _mediatorMock.Verify(x => x.Publish(It.IsAny<OrderCreatedEvent>(), default), Times.Once);
    }
}
```

**Critérios de Aceitação:**
- [ ] Mínimo 20 testes de handlers
- [ ] Todos os testes passando
- [ ] Uso de Moq para mocks
- [ ] Uso de FluentAssertions
- [ ] Coverage > 70% nos handlers

---

#### 6.3 Unit Tests - Validators - 1 dia

**Testes a criar:**

1. **CreateOrderCommandValidatorTests.cs**
   - ✅ Should_Have_Error_When_Title_Is_Empty
   - ✅ Should_Have_Error_When_Title_Exceeds_MaxLength
   - ✅ Should_Have_Error_When_Description_Is_Empty
   - ✅ Should_Not_Have_Error_When_Valid

2. **UpdateOrderCommandValidatorTests.cs**
3. **CreateCategoryCommandValidatorTests.cs**

**Exemplo:**
```csharp
public class CreateOrderCommandValidatorTests
{
    private readonly CreateOrderCommandValidator _validator;

    public CreateOrderCommandValidatorTests()
    {
        _validator = new CreateOrderCommandValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Title_Is_Empty()
    {
        // Arrange
        var command = new CreateOrderCommand(
            Title: "",
            Description: "Valid description",
            // ... outros campos
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == nameof(command.Title));
    }
}
```

**Critérios de Aceitação:**
- [ ] Testes para todos os validators principais
- [ ] Cobertura de casos válidos e inválidos
- [ ] Todos os testes passando

---

#### 6.4 Integration Tests - API - 2 dias

**Setup - WebApplicationFactory:**
```csharp
public class EChamadoWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly PostgreSqlContainer _dbContainer;

    public EChamadoWebApplicationFactory()
    {
        _dbContainer = new PostgreSqlBuilder()
            .WithImage("postgres:15-alpine")
            .WithDatabase("echamado_test")
            .WithUsername("test")
            .WithPassword("test")
            .Build();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            // Remove o DbContext real
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            // Adiciona DbContext de teste
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(_dbContainer.GetConnectionString());
            });

            // Seed de dados de teste
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Database.EnsureCreated();
            SeedTestData(db);
        });
    }

    public async Task InitializeAsync() => await _dbContainer.StartAsync();
    public async Task DisposeAsync() => await _dbContainer.StopAsync();
}
```

**Testes a criar:**

1. **OrdersControllerTests.cs**
   - ✅ GET_Orders_Returns_Paged_Results
   - ✅ GET_Order_By_Id_Returns_Order
   - ✅ GET_Order_By_Id_Returns_404_When_Not_Found
   - ✅ POST_Order_Creates_New_Order
   - ✅ POST_Order_Returns_400_When_Invalid
   - ✅ PUT_Order_Updates_Order
   - ✅ POST_Close_Closes_Order

2. **CategoriesControllerTests.cs**
3. **AuthenticationTests.cs**

**Exemplo:**
```csharp
public class OrdersControllerTests : IClassFixture<EChamadoWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly EChamadoWebApplicationFactory _factory;

    public OrdersControllerTests(EChamadoWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GET_Orders_Returns_Paged_Results()
    {
        // Arrange
        await AuthenticateAsync();

        // Act
        var response = await _client.GetAsync("/api/orders?pageNumber=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<OrderListViewModel>>();
        result.Should().NotBeNull();
        result!.Items.Should().NotBeEmpty();
    }

    private async Task AuthenticateAsync()
    {
        var token = await GetJwtTokenAsync();
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }
}
```

**Critérios de Aceitação:**
- [ ] Testcontainers configurado
- [ ] WebApplicationFactory funcionando
- [ ] Testes para principais endpoints (mínimo 15)
- [ ] Testes de autenticação/autorização
- [ ] Todos os testes passando

---

#### 6.5 CI/CD Pipeline - GitHub Actions - 2 dias

**Arquivo a criar:**
- `.github/workflows/ci-cd.yml` (CRIAR)

**Pipeline stages:**

1. **Build & Test**
```yaml
name: CI/CD Pipeline

on:
  push:
    branches: [ main, develop, 'claude/**' ]
  pull_request:
    branches: [ main, develop ]

jobs:
  build-and-test:
    runs-on: ubuntu-latest

    services:
      postgres:
        image: postgres:15-alpine
        env:
          POSTGRES_DB: echamado_test
          POSTGRES_USER: test
          POSTGRES_PASSWORD: test
        options: >-
          --health-cmd pg_isready
          --health-interval 10s
          --health-timeout 5s
          --health-retries 5
        ports:
          - 5432:5432

    steps:
      - uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '9.0.x'

      - name: Restore dependencies
        run: dotnet restore

      - name: Build
        run: dotnet build --no-restore --configuration Release

      - name: Run Unit Tests
        run: dotnet test tests/EChamado.Server.UnitTests --no-build --verbosity normal --logger "trx;LogFileName=unit-tests.trx"

      - name: Run Integration Tests
        run: dotnet test tests/EChamado.Server.IntegrationTests --no-build --verbosity normal --logger "trx;LogFileName=integration-tests.trx"
        env:
          ConnectionStrings__DefaultConnection: "Host=localhost;Database=echamado_test;Username=test;Password=test"

      - name: Test Report
        uses: dorny/test-reporter@v1
        if: always()
        with:
          name: Test Results
          path: '**/*.trx'
          reporter: dotnet-trx
```

2. **Code Coverage**
```yaml
      - name: Code Coverage
        run: |
          dotnet test --collect:"XPlat Code Coverage" --results-directory ./coverage
          dotnet tool install -g dotnet-reportgenerator-globaltool
          reportgenerator -reports:./coverage/**/coverage.cobertura.xml -targetdir:./coverage/report -reporttypes:Html

      - name: Upload Coverage
        uses: codecov/codecov-action@v4
        with:
          files: ./coverage/**/coverage.cobertura.xml
          fail_ci_if_error: true
```

3. **Docker Build & Push**
```yaml
  docker-build:
    needs: build-and-test
    runs-on: ubuntu-latest
    if: github.ref == 'refs/heads/main'

    steps:
      - uses: actions/checkout@v4

      - name: Set up Docker Buildx
        uses: docker/setup-buildx-action@v3

      - name: Login to Docker Hub
        uses: docker/login-action@v3
        with:
          username: ${{ secrets.DOCKER_USERNAME }}
          password: ${{ secrets.DOCKER_PASSWORD }}

      - name: Build and push Server
        uses: docker/build-push-action@v5
        with:
          context: ./src/EChamado/Server
          file: ./src/EChamado/Server/Dockerfile
          push: true
          tags: |
            ${{ secrets.DOCKER_USERNAME }}/echamado-server:latest
            ${{ secrets.DOCKER_USERNAME }}/echamado-server:${{ github.sha }}

      - name: Build and push Client
        uses: docker/build-push-action@v5
        with:
          context: ./src/EChamado/Client
          file: ./src/EChamado/Client/Dockerfile
          push: true
          tags: |
            ${{ secrets.DOCKER_USERNAME }}/echamado-client:latest
            ${{ secrets.DOCKER_USERNAME }}/echamado-client:${{ github.sha }}
```

4. **Semantic Release** (opcional)
```yaml
  release:
    needs: docker-build
    runs-on: ubuntu-latest
    if: github.ref == 'refs/heads/main'

    steps:
      - uses: actions/checkout@v4

      - name: Semantic Release
        uses: cycjimmy/semantic-release-action@v4
        with:
          semantic_version: 19
        env:
          GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
```

**Secrets a configurar no GitHub:**
- `DOCKER_USERNAME`
- `DOCKER_PASSWORD`
- `CODECOV_TOKEN` (opcional)

**Critérios de Aceitação:**
- [ ] Pipeline rodando em todos os pushes
- [ ] Build automatizado
- [ ] Testes automatizados (Unit + Integration)
- [ ] Code coverage reportado
- [ ] Docker images buildadas automaticamente
- [ ] Deploy automático em main (opcional)
- [ ] Badge de status no README

---

### Resumo FASE 6

**Projetos novos**: 2 (UnitTests + IntegrationTests)
**Arquivos de teste**: ~30 arquivos
**Testes mínimos**: 50+ testes
**Linhas de código**: ~3.000 linhas de testes
**Workflows CI/CD**: 1 arquivo

**Checklist Final FASE 6:**
- [ ] Projetos de teste criados
- [ ] 20+ unit tests (handlers)
- [ ] 10+ unit tests (validators)
- [ ] 15+ integration tests (API)
- [ ] Code coverage > 70%
- [ ] CI/CD pipeline funcionando
- [ ] Docker build automatizado
- [ ] Badges no README
- [ ] Commit e push

---

## 📈 MÉTRICAS DE SUCESSO

### Após FASE 4
- ✅ 100% das interfaces implementadas
- ✅ Sistema totalmente utilizável por usuários finais
- ✅ 0 endpoints sem UI correspondente

### Após FASE 5
- ✅ Health checks em todos os serviços
- ✅ Endpoints de monitoramento funcionais
- ✅ Logs estruturados no ELK
- ✅ Pronto para deploy em produção (infraestrutura)

### Após FASE 6
- ✅ Code coverage > 70%
- ✅ 50+ testes automatizados
- ✅ CI/CD pipeline funcional
- ✅ Build e deploy automatizados
- ✅ **PRODUCTION READY** 🚀

---

## 🎯 CRONOGRAMA SUGERIDO

### Semana 1 (5 dias úteis)
- **Dia 1**: Comments API (Backend)
- **Dia 2**: Admin - Categories Page
- **Dia 3**: Admin - Departments + Order Types
- **Dia 4**: Admin - Status Types + Integração Comments
- **Dia 5**: Health Checks + Docker Health Checks

### Semana 2 (5 dias úteis)
- **Dia 6**: Estrutura de Testes + Unit Tests (Handlers - parte 1)
- **Dia 7**: Unit Tests (Handlers - parte 2 + Validators)
- **Dia 8**: Integration Tests (setup + Controllers parte 1)
- **Dia 9**: Integration Tests (Controllers parte 2)
- **Dia 10**: CI/CD Pipeline + Ajustes finais

### Semana 3 (contingência)
- Ajustes, refinamentos, documentação

---

## 🚀 PRÓXIMOS PASSOS

**Após concluir as FASES 4-6**, o projeto estará **95-100% production-ready**.

**Features futuras** (FASE 7 - opcional):
- Sistema de Anexos (file storage)
- Notificações por Email
- Relatórios PDF/Excel
- Sistema de Auditoria (LGPD)
- SLA Tracking
- 2FA (Two-Factor Auth)

---

## 📝 NOTAS IMPORTANTES

1. **Commits frequentes**: Fazer commits ao final de cada tarefa
2. **Testes antes de commit**: Sempre testar manualmente antes de commitar
3. **Code review**: Revisar código antes de merge em main
4. **Documentação**: Atualizar README conforme avança
5. **Backup**: Fazer backup do banco de dados antes de migrations

---

## ✅ CRITÉRIOS DE ACEITAÇÃO GERAL

### FASE 4 - Interface Completa
- [ ] 4 páginas admin criadas e funcionais
- [ ] Comments API completa (backend + frontend)
- [ ] Todas as funcionalidades testadas manualmente
- [ ] UI consistente com padrão MudBlazor
- [ ] Sem bugs críticos

### FASE 5 - Monitoramento
- [ ] Health checks implementados
- [ ] 4 endpoints funcionais (/health, /ready, /live, /health-ui)
- [ ] Docker health checks configurados
- [ ] Logging estruturado funcionando
- [ ] Testes de health checks bem-sucedidos

### FASE 6 - Qualidade & CI/CD
- [ ] Mínimo 50 testes criados
- [ ] Code coverage > 70%
- [ ] Todos os testes passando
- [ ] CI/CD pipeline rodando sem erros
- [ ] Docker images sendo buildadas automaticamente
- [ ] Badges de status no README

---

## 🎉 RESULTADO FINAL

Ao concluir as **FASES 4, 5 e 6**, o projeto EChamado estará:

✅ **Completo** - 100% das funcionalidades planejadas
✅ **Testado** - >70% code coverage
✅ **Monitorado** - Health checks + observabilidade
✅ **Automatizado** - CI/CD pipeline funcional
✅ **Production-Ready** - Pronto para deploy em produção

**Status Final: 95-100% COMPLETO** 🚀

---

**Documento criado em**: 2025-11-08
**Versão**: 1.0
**Autor**: Claude (Anthropic)
