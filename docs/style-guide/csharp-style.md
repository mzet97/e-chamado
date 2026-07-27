# 📝 Guia de Estilo de Código C#

## Padrões e Convenções para EChamado

### 🎯 Visão Geral

Este guia estabelece os padrões de escrita de código C# para o projeto EChamado, garantindo consistência, legibilidade e manutenibilidade do código.

---

## 📏 Formatação de Código

### 1. Estrutura de Arquivos

```csharp
// Exemplo: CreateOrderEndpoint.cs (Minimal API endpoint)

using EChamado.Server.Application.UseCases.Orders.Commands;
using EChamado.Server.Common.Api;
using EChamado.Shared.Responses;
using Paramore.Brighter;
using Microsoft.AspNetCore.Mvc;

namespace EChamado.Server.Endpoints.Orders;

/// <summary>
/// Endpoint para criação de uma nova ordem/chamado
/// </summary>
public class CreateOrderEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPost("/", HandleAsync)
            .WithName("Criar uma nova ordem")
            .Produces<BaseResult<Guid>>();

    private static async Task<IResult> HandleAsync(
        [FromServices] IAmACommandProcessor commandProcessor,
        [FromBody] CreateOrderRequest request)
    {
        var command = request.ToCommand();
        await commandProcessor.SendAsync(command);

        var result = command.Result;
        return result.Success
            ? TypedResults.Ok(result)
            : TypedResults.BadRequest(result);
    }
}
```

### 2. Convenções de Nomenclatura

#### 2.1 PascalCase (Classes, Interfaces, Métodos, Propriedades)
```csharp
// ✅ Correto
public class OrderService
public interface IOrderRepository
public void CreateOrder()
public Guid OrderId { get; set; }

// ❌ Incorreto
public class orderService
public interface iorderrepository
public void createOrder()
public Guid order_id { get; set; }
```

#### 2.2 camelCase (Parâmetros, Variáveis Locais, Campos privados)
```csharp
// ✅ Correto
public async Task<OrderDto> GetOrderById(Guid orderId)
{
    var order = await _repository.GetByIdAsync(orderId);
    var result = MapToViewModel(order);
    return result;
}

// Campos privados com underscore
private readonly IUnitOfWork _unitOfWork;
private readonly IDateTimeProvider _dateTimeProvider;
```

#### 2.3 Constantes — PascalCase
```csharp
// ✅ Correto (padrão real do projeto)
public static class ApplicationConstants
{
    public static class Validation
    {
        public const int MinPasswordLength = 12;
        public const int MaxTitleLength = 200;
    }
}
```

### 3. Espaçamento e Indentação

```csharp
// ✅ Correto - Usar 4 espaços para indentação
public class CreateCategoryCommandHandler(
    IUnitOfWork unitOfWork,
    IAmACommandProcessor commandProcessor,
    IDateTimeProvider dateTimeProvider,
    ILogger<CreateCategoryCommandHandler> logger) :
    RequestHandlerAsync<CreateCategoryCommand>
{
    [RequestLogging(0, HandlerTiming.Before)]
    [RequestValidation(1, HandlerTiming.Before)]
    public override async Task<CreateCategoryCommand> HandleAsync(
        CreateCategoryCommand command,
        CancellationToken cancellationToken = default)
    {
        var entity = Category.Create(command.Name, command.Description, dateTimeProvider);

        if (!entity.IsValid())
            throw new ValidationException("Validation failed", entity.Errors);

        await unitOfWork.BeginTransactionAsync();
        await unitOfWork.Categories.AddAsync(entity);
        await unitOfWork.CommitAsync();

        command.Result = new BaseResult<Guid>(entity.Id);
        return await base.HandleAsync(command, cancellationToken);
    }
}
```

---

## 🏗️ Estrutura de Classes

### 4.1 Entidades de Domínio (DDD)

```csharp
// Padrão: Factory Method estático + ctor privado + setters private
public class Category : SoftDeletableEntity<Category>
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;

    private Category() : base(new CategoryValidation()) { }

    // Factory Method — ÚNICA forma de criar
    public static Category Create(string name, string description, IDateTimeProvider dt)
    {
        var category = new Category();
        category.Id = Guid.NewGuid();
        category.Name = name;
        category.Description = description;
        category.MarkCreated(dt.UtcNow);
        category.Validate();
        category.AddEvent(new CategoryCreated(category.Id, category.Name, category.Description));
        return category;
    }

    // Métodos de comportamento — mutam estado + validam + emitem eventos
    public void Update(string name, string description, IDateTimeProvider dt)
    {
        Name = name;
        Description = description;
        MarkUpdated(dt.UtcNow);
        Validate();
        AddEvent(new CategoryUpdated(Id, Name, Description));
    }

    public override void Validate()
    {
        var result = new CategoryValidation().Validate(this);
        _errors = result.Errors.Select(x => x.ErrorMessage);
        _isValid = result.IsValid;
    }
}
```

### 4.2 Modificadores de Acesso

```csharp
// ✅ Correto - Sempre ser explícito
public class Order
{
    public Guid Id { get; private set; }
    private readonly List<Comment> _comments = new();
    internal Order(Guid id, ...) { }  // internal para testes/EF
}
```

---

## 🔧 Convenções de Código

### 5.1 Tratamento de Null

```csharp
// ✅ Correto - Usar verificações explícitas e null-coalescing
var categoryId = command.CategoryId ?? Guid.Empty;

if (categoryId == Guid.Empty)
    throw new ValidationException("Category is required");

// Usar pattern matching (C# 9+)
var statusName = order.Status?.Name ?? "Desconhecido";
```

### 5.2 Exceções e Error Handling

```csharp
// ✅ Correto - Exceções específicas do domínio
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}

public class ValidationException : Exception
{
    public IEnumerable<string> Errors { get; }
    public ValidationException(string message, IEnumerable<string> errors)
        : base(message) => Errors = errors;
}

// Em handlers: lançar, não capturar
if (order == null)
    throw new NotFoundException($"Order {command.OrderId} not found");

// Em endpoints: capturar e retornar BadRequest
catch (Exception ex)
{
    return TypedResults.BadRequest(new BaseResult<T>(
        data: default, success: false, message: ex.Message));
}
```

### 5.3 Async/Await Patterns

```csharp
// ✅ Correto - CancellationToken sempre como último parâmetro
public override async Task<CreateOrderCommand> HandleAsync(
    CreateOrderCommand command,
    CancellationToken cancellationToken = default)
{
    // ...
    await unitOfWork.CommitAsync();
}

// Evitar async void (exceto event handlers)
```

### 5.4 DateTime — Nunca usar DateTime.UtcNow direto

```csharp
// ❌ Incorreto
var now = DateTime.UtcNow;

// ✅ Correto — usar IDateTimeProvider (testabilidade)
public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
    DateTime Now { get; }
}

// Injeção via DI (registrado como singleton)
services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();
```

---

## 📋 Documentação de Código

### 6.1 XML Documentation

```csharp
/// <summary>
/// Handler para criar uma nova categoria
/// </summary>
public class CreateCategoryCommandHandler(
    IUnitOfWork unitOfWork,
    IAmACommandProcessor commandProcessor,
    IDateTimeProvider dateTimeProvider,
    ILogger<CreateCategoryCommandHandler> logger) :
    RequestHandlerAsync<CreateCategoryCommand>
{
    /// <summary>
    /// Processa o comando de criação de categoria
    /// </summary>
    /// <param name="command">Dados da categoria a ser criada</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>O comando com Result preenchido</returns>
    /// <exception cref="ValidationException">Quando dados são inválidos</exception>
    public override async Task<CreateCategoryCommand> HandleAsync(
        CreateCategoryCommand command,
        CancellationToken cancellationToken = default) { ... }
}
```

---

## 🧪 Padrões de Testes

### 7.1 Framework: xUnit + FluentAssertions + Moq

```csharp
// ✅ Correto — xUnit com [Fact] e [Theory]
public class CreateCategoryCommandHandlerTests : UnitTestBase
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IDateTimeProvider> _dateTimeProviderMock;
    private readonly CreateCategoryCommandHandler _handler;

    public CreateCategoryCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _dateTimeProviderMock = new Mock<IDateTimeProvider>();
        _dateTimeProviderMock.SetupGet(x => x.UtcNow).Returns(DateTime.UtcNow);
        _handler = new CreateCategoryCommandHandler(
            _unitOfWorkMock.Object,
            new Mock<IAmACommandProcessor>().Object,
            _dateTimeProviderMock.Object,
            new Mock<ILogger<CreateCategoryCommandHandler>>().Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCreateCategory()
    {
        // Arrange
        var command = new CreateCategoryCommand("Test", "Description");
        SetupValidMocks();

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        result.Result!.Success.Should().BeTrue();
        ((BaseResult<Guid>)result.Result).Data.Should().NotBeEmpty();
        _unitOfWorkMock.Verify(x => x.Categories.AddAsync(It.IsAny<Category>()), Times.Once);
    }

    [Theory]
    [InlineData("", "Description")]
    [InlineData("   ", "Description")]
    public async Task Handle_InvalidName_ShouldThrowValidationException(string name, string desc)
    {
        var command = new CreateCategoryCommand(name, desc);
        await _handler.Invoking(h => h.HandleAsync(command))
            .Should().ThrowAsync<Exception>();
    }
}
```

---

## 📊 Resumo das Convenções

### ✅ **Do's (Fazer):**
- Usar **PascalCase** para classes, métodos e propriedades
- Usar **camelCase** para variáveis e parâmetros (`_camelCase` para campos privados)
- Documentar métodos públicos com XML comments
- Usar `async/await` consistentemente
- Implementar tratamento de exceções robusto (`NotFoundException`, `ValidationException`)
- Usar validação com **FluentValidation** (no Domain)
- Escrever testes unitários com **xUnit** + padrão AAA
- Seguir princípios SOLID
- Usar dependency injection
- Implementar logging estruturado (ILogger<T>)
- Usar **IDateTimeProvider** em vez de `DateTime.UtcNow`

### ❌ **Don'ts (Não Fazer):**
- Usar abreviações em nomes de classes/métodos
- Deixar métodos sem documentação pública
- Ignorar warnings do compilador
- Usar `async void` (exceto event handlers)
- Tratar exceções genéricas sem logging
- Criar classes com muitas responsabilidades
- Usar strings mágicas sem constantes
- Misturar responsabilidades de diferentes camadas
- Usar `DateTime.UtcNow` diretamente (usar `IDateTimeProvider`)

---

## 🏛️ Padrões Arquiteturais do Projeto

### CQRS com Paramore.Brighter (Commands) + Paramore.Darker (Queries)

```csharp
// Command (Brighter) — escrita
public class CreateOrderCommand : BrighterRequest<BaseResult<Guid>>
{
    public string Title { get; set; } = string.Empty;
    // ...
}

// Handler (Brighter)
public class CreateOrderCommandHandler(
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider,
    ILogger<CreateOrderCommandHandler> logger) :
    RequestHandlerAsync<CreateOrderCommand>
{
    [RequestLogging(0, HandlerTiming.Before)]
    [RequestValidation(1, HandlerTiming.Before)]
    public override async Task<CreateOrderCommand> HandleAsync(
        CreateOrderCommand command, CancellationToken ct = default) { ... }
}

// Query (Darker) — leitura
public class GridifyOrderQuery : IGridifyQuery { ... }

// Handler (Darker)
public class GridifyOrderQueryHandler : QueryHandlerAsync<GridifyOrderQuery, BaseResultList<OrderViewModel>>
{
    public override async Task<BaseResultList<OrderViewModel>> ExecuteAsync(
        GridifyOrderQuery request, CancellationToken ct = default) { ... }
}
```

### Minimal API Endpoints

```csharp
// Cada endpoint implementa IEndpoint com Map estático
public class CreateCategoryEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPost("/", HandleAsync)
            .WithName("Criar categoria")
            .Produces<BaseResult<Guid>>();

    private static async Task<IResult> HandleAsync(
        [FromServices] IAmACommandProcessor commandProcessor,
        [FromBody] CreateCategoryRequest request)
    {
        var command = request.ToCommand();  // Extension method
        await commandProcessor.SendAsync(command);
        return command.Result.Success
            ? TypedResults.Ok(command.Result)
            : TypedResults.BadRequest(command.Result);
    }
}
```

### Domain Events

```csharp
// Na entidade: AddEvent() durante Create/Update
order.AddEvent(new OrderCreated(order.Id, order.Title, ...));

// Interceptor captura após SaveChanges
// BrighterEventMapper converte Domain Event → Brighter Event
// Brighter publica no barramento
```

### Responses (sempre)

```csharp
// Single result
new BaseResult<T>(data, success: true, message: "")

// Paged result
new BaseResultList<T>(data, pagedResult, success: true, message: "")
```

---

## 📚 Referências

| Tópico | Tecnologia real do projeto |
|--------|---------------------------|
| Commands | **Paramore.Brighter** (`RequestHandlerAsync<T>`) |
| Queries | **Paramore.Darker** (`QueryHandlerAsync<TQuery, TResult>`) |
| API | **Minimal API** (`IEndpoint`, `MapPost/MapGet`) |
| Mapeamento | **Manual** (extensions `ToCommand()`, constructors) |
| Validação | **FluentValidation** (no Domain) |
| Testes | **xUnit** (`[Fact]`/`[Theory]`) + **FluentAssertions** + **Moq** |
| DateTime | **IDateTimeProvider** (nunca `DateTime.UtcNow` direto) |
| Docs API | **Scalar** (substitui Swagger UI) |
| Cache | **Redis** (com fallback em memória) |
| Logging | **Serilog** → ELK Stack |

---

**Última atualização:** 26 de julho de 2026
**Versão:** 2.0.0
**Status:** ✅ Atualizado para refletir o código real (Brighter/Darker, Minimal API, xUnit, IDateTimeProvider)
