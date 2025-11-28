# 📝 Guia de Estilo de Código C#

## Padrões e Convenções para EChamado

### 🎯 Visão Geral

Este guia estabelece os padrões de escrita de código C# para o projeto EChamado, garantindo consistência, legibilidade e manutenibilidade do código.

---

## 📏 Formatação de Código

### 1. Estrutura de Arquivos

```csharp
// Exemplo: OrderController.cs

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using FluentValidation;

// Namespace principal
namespace EChamado.Server.Endpoints.Orders
{
    /// <summary>
    /// Controller responsável por gerenciar endpoints de Ordens
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;
        
        public OrdersController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        // Métodos endpoints aqui...
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

#### 2.2 camelCase (Parâmetros, Variáveis Locais)
```csharp
// ✅ Correto
public async Task<OrderDto> GetOrderById(Guid orderId)
{
    var order = await _repository.GetByIdAsync(orderId);
    var result = _mapper.Map<OrderDto>(order);
    return result;
}

// ❌ Incorreto
public async Task<OrderDto> GetOrderById(Guid OrderId)
{
    var Order = await _repository.GetByIdAsync(OrderId);
    var Result = _mapper.Map<OrderDto>(Order);
    return Result;
}
```

#### 2.3 UPPER_CASE (Constantes)
```csharp
// ✅ Correto
public const string DEFAULT_STATUS = "Open";
public const int MAX_RETRY_COUNT = 3;

// ❌ Incorreto
public const string defaultStatus = "Open";
public const int MaxRetryCount = 3;
```

### 3. Espaçamento e Indentação

```csharp
// ✅ Correto - Usar 4 espaços para indentação
public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;
    
    public OrderService(
        IOrderRepository orderRepository,
        IMapper mapper)
    {
        _orderRepository = orderRepository ?? 
            throw new ArgumentNullException(nameof(orderRepository));
        _mapper = mapper ?? 
            throw new ArgumentNullException(nameof(mapper));
    }
    
    public async Task<OrderDto> CreateOrderAsync(
        CreateOrderCommand command)
    {
        if (command == null)
            throw new ArgumentNullException(nameof(command));
            
        var order = await ProcessOrderCreation(command);
        return _mapper.Map<OrderDto>(order);
    }
}

// ❌ Incorreto - Indentação inconsistente
public class OrderService : IOrderService
{
private readonly IOrderRepository _orderRepository;
private readonly IMapper _mapper;

public OrderService(IOrderRepository orderRepository, IMapper mapper)
{
_orderRepository = orderRepository;
_mapper = mapper;
}
}
```

---

## 🏗️ Estrutura de Classes

### 4.1 Ordenação de Membros

```csharp
public class Order
{
    // Constantes
    public const int MIN_TITLE_LENGTH = 5;
    public const int MAX_TITLE_LENGTH = 200;
    
    // Campos privados (com underscore)
    private readonly List<Comment> _comments;
    private readonly DateTime _createdAt;
    
    // Propriedades
    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public OrderStatus Status { get; private set; }
    
    // Construtores
    public Order(string title, string description, Guid categoryId)
    {
        Id = Guid.NewGuid();
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        _comments = new List<Comment>();
        _createdAt = DateTime.UtcNow;
        Status = OrderStatus.Open;
    }
    
    // Métodos públicos
    public void ChangeStatus(OrderStatus newStatus)
    {
        if (!IsValidStatusTransition(Status, newStatus))
            throw new InvalidOperationException("Invalid status transition");
            
        Status = newStatus;
    }
    
    public void AddComment(string content, Guid userId)
    {
        var comment = new Comment(content, userId, Id);
        _comments.Add(comment);
    }
    
    // Métodos privados
    private bool IsValidStatusTransition(OrderStatus current, OrderStatus next)
    {
        // Lógica de validação de transição
        return true;
    }
}
```

### 4.2 Modificadores de Acesso

```csharp
// ✅ Correto - Sempre ser explícito
public class PublicClass
{
    public string PublicProperty { get; set; }
    internal string InternalProperty { get; set; }
    private string PrivateProperty { get; set; }
    protected string ProtectedProperty { get; set; }
}

// ❌ Incorreto - Não usar modificadores padrão
class PublicClass  // Faltou public
{
    string PublicProperty { get; set; }  // Faltou modificador
}
```

---

## 🔧 Convenções de Código

### 5.1 Tratamento de Null

```csharp
// ✅ Correto - Usar verificações explícitas e null-coalescing
public class OrderService
{
    private readonly IOrderRepository _orderRepository;
    
    public OrderService(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository ?? 
            throw new ArgumentNullException(nameof(orderRepository));
    }
    
    public async Task<OrderDto> GetOrderAsync(Guid? orderId)
    {
        if (!orderId.HasValue)
            return null;
            
        var order = await _orderRepository.GetByIdAsync(orderId.Value);
        return order?.MapToDto();
    }
    
    public void ProcessOrder(Order order)
    {
        // Usar operador null-conditional
        order?.ChangeStatus(OrderStatus.InProgress);
        
        // Usar null-coalescing operator
        var title = order?.Title ?? "Untitled Order";
        
        // Usar pattern matching (C# 9+)
        var isValid = order switch
        {
            { Status: OrderStatus.Open } => true,
            { Status: OrderStatus.Closed } => false,
            null => false,
            _ => false
        };
    }
}
```

### 5.2 Exceções e Error Handling

```csharp
// ✅ Correto - Usar exceções específicas e mensagens descritivas
public class OrderService
{
    public async Task<OrderDto> CreateOrderAsync(CreateOrderCommand command)
    {
        try
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));
                
            var validator = new CreateOrderCommandValidator();
            var validationResult = await validator.ValidateAsync(command);
            
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);
                
            var order = await ProcessOrderCreation(command);
            return _mapper.Map<OrderDto>(order);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation failed for order creation");
            throw; // Re-throw preserving stack trace
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "Null argument provided to CreateOrderAsync");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during order creation");
            throw new ApplicationException("Failed to create order", ex);
        }
    }
}
```

### 5.3 Async/Await Patterns

```csharp
// ✅ Correto - Padrões adequados para código assíncrono
public class OrderService
{
    public async Task<OrderDto> GetOrderAsync(Guid orderId)
    {
        // Sempre adicionar CancellationToken quando possível
        return await _orderRepository.GetByIdAsync(orderId, HttpContext.RequestAborted)
            .ConfigureAwait(false);
    }
    
    public async Task<IEnumerable<OrderDto>> GetOrdersByUserAsync(Guid userId)
    {
        // Usar Task.FromResult para operações síncronas
        if (userId == Guid.Empty)
            return Task.FromResult<IEnumerable<OrderDto>>(new List<OrderDto>());
            
        // Usar WhenAll para operações paralelas
        var ordersTask = _orderRepository.GetByUserIdAsync(userId);
        var userTask = _userRepository.GetByIdAsync(userId);
        
        await Task.WhenAll(ordersTask, userTask);
        
        var orders = await ordersTask;
        var user = await userTask;
        
        return orders.Select(o => MapOrderWithUser(o, user));
    }
    
    // Evitar async void (apenas para event handlers)
    protected override async void OnLoad()
    {
        await LoadOrdersAsync();
    }
}
```

---

## 📋 Documentação de Código

### 6.1 XML Documentation

```csharp
/// <summary>
/// Serviço responsável por gerenciar operações relacionadas a ordens.
/// </summary>
/// <remarks>
/// Este serviço implementa o padrão Repository e utiliza CQRS
/// para separar comandos de queries.
/// </remarks>
public interface IOrderService
{
    /// <summary>
    /// Cria uma nova ordem no sistema
    /// </summary>
    /// <param name="command">Dados necessários para criação da ordem</param>
    /// <returns>Dados da ordem criada</returns>
    /// <exception cref="ArgumentNullException">Quando command é nulo</exception>
    /// <exception cref="ValidationException">Quando dados são inválidos</exception>
    Task<OrderDto> CreateOrderAsync(CreateOrderCommand command);
    
    /// <summary>
    /// Obtém uma ordem pelo identificador único
    /// </summary>
    /// <param name="orderId">Identificador da ordem</param>
    /// <returns>Dados da ordem encontrada ou null se não existir</returns>
    Task<OrderDto?> GetOrderByIdAsync(Guid orderId);
    
    /// <summary>
    /// Atualiza o status de uma ordem
    /// </summary>
    /// <param name="orderId">Identificador da ordem</param>
    /// <param name="newStatus">Novo status da ordem</param>
    /// <returns>True se atualizado com sucesso, false caso contrário</returns>
    Task<bool> UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus);
}
```

### 6.2 Comentários Inline

```csharp
public class OrderService : IOrderService
{
    public async Task<OrderDto> ProcessOrderAsync(Order order)
    {
        // Validação de regras de negócio específicas
        // Esta validação é necessária para garantir SLA compliance
        if (!ValidateBusinessRules(order))
            throw new BusinessRuleException("Order violates business rules");
            
        // Processar em background para melhor performance
        // usando Task.Run para não bloquear thread principal
        await Task.Run(() => ProcessOrderInBackground(order))
                  .ConfigureAwait(false);
                  
        // Retornar resultado processado
        return MapToDto(order);
    }
}
```

---

## 🧪 Padrões de Testes

### 7.1 Estrutura de Testes Unitários

```csharp
[TestFixture]
public class OrderServiceTests
{
    private Mock<IOrderRepository> _orderRepositoryMock;
    private Mock<ILogger<OrderService>> _loggerMock;
    private Mock<IMapper> _mapperMock;
    private OrderService _orderService;
    
    [SetUp]
    public void SetUp()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _loggerMock = new Mock<ILogger<OrderService>>();
        _mapperMock = new Mock<IMapper>();
        
        _orderService = new OrderService(
            _orderRepositoryMock.Object,
            _loggerMock.Object,
            _mapperMock.Object);
    }
    
    [Test]
    public async Task CreateOrderAsync_WithValidData_ShouldReturnCreatedOrder()
    {
        // Arrange
        var command = new CreateOrderCommand
        {
            Title = "Test Order",
            Description = "Test Description",
            CategoryId = Guid.NewGuid()
        };
        
        var expectedOrder = new Order(
            command.Title,
            command.Description,
            command.CategoryId);
            
        _orderRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Order>()))
            .ReturnsAsync(expectedOrder);
            
        _mapperMock
            .Setup(x => x.Map<OrderDto>(It.IsAny<Order>()))
            .Returns((Order o) => new OrderDto { Id = o.Id, Title = o.Title });
        
        // Act
        var result = await _orderService.CreateOrderAsync(command);
        
        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be(command.Title);
        _orderRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<Order>()), 
            Times.Once);
    }
    
    [Test]
    public void CreateOrderAsync_WithNullCommand_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        FluentActions.Invoking(() => 
            _orderService.CreateOrderAsync(null))
            .Should().Throw<ArgumentNullException>();
    }
    
    [TearDown]
    public void TearDown()
    {
        // Cleanup se necessário
    }
}
```

### 7.2 Padrões de Teste (AAA)

```csharp
[Test]
public async Task OrderService_UpdateStatus_WithValidTransition_ShouldSucceed()
{
    // Arrange (Preparar)
    var order = CreateTestOrder(OrderStatus.Open);
    var newStatus = OrderStatus.InProgress;
    
    _orderRepositoryMock
        .Setup(x => x.GetByIdAsync(order.Id))
        .ReturnsAsync(order);
        
    // Act (Executar)
    var result = await _orderService.UpdateOrderStatusAsync(order.Id, newStatus);
    
    // Assert (Verificar)
    result.Should().BeTrue();
    order.Status.Should().Be(newStatus);
    _orderRepositoryMock.Verify(
        x => x.UpdateAsync(order), 
        Times.Once);
}
```

---

## 🔐 Padrões de Segurança

### 8.1 Validação de Input

```csharp
public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Título é obrigatório")
            .Length(5, 200)
            .WithMessage("Título deve ter entre 5 e 200 caracteres")
            .Matches(@"^[a-zA-Z0-9\s\-\._\+]+$")
            .WithMessage("Título contém caracteres inválidos");
            
        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Descrição é obrigatória")
            .Length(10, 2000)
            .WithMessage("Descrição deve ter entre 10 e 2000 caracteres");
            
        RuleFor(x => x.CategoryId)
            .NotEmpty()
            .WithMessage("Categoria é obrigatória")
            .MustAsync(BeValidCategory)
            .WithMessage("Categoria não encontrada ou inativa");
    }
    
    private async Task<bool> BeValidCategory(Guid categoryId, CancellationToken cancellationToken)
    {
        // Implementar validação contra banco de dados
        return await _categoryRepository.ExistsAsync(categoryId);
    }
}
```

### 8.2 Autenticação e Autorização

```csharp
[Authorize(Roles = "Admin,Manager,Agent")]
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDto>> GetOrder(Guid id)
    {
        // Verificar se usuário tem permissão para acessar esta ordem
        if (!await _authorizationService.CanAccessOrderAsync(User.GetUserId(), id))
            return Forbid();
            
        var order = await _orderService.GetOrderByIdAsync(id);
        return Ok(order);
    }
    
    [Authorize(Roles = "Agent,Manager,Admin")]
    [HttpPut("{id}/status")]
    public async Task<ActionResult> UpdateStatus(Guid id, [FromBody] UpdateOrderStatusCommand command)
    {
        if (id != command.OrderId)
            return BadRequest("Order ID mismatch");
            
        await _mediator.Send(command);
        return NoContent();
    }
}
```

---

## 📊 Configurações e Constantes

### 9.1 Constants vs Configuration

```csharp
// ✅ Correto - Usar constantes para valores fixos da aplicação
public static class OrderConstants
{
    public const int MIN_TITLE_LENGTH = 5;
    public const int MAX_TITLE_LENGTH = 200;
    public const string DEFAULT_STATUS = "Open";
    public static readonly TimeSpan DEFAULT_SLA = TimeSpan.FromDays(7);
}

// ✅ Correto - Usar IOptions para configurações
public class OrderSettings
{
    public int MaxTitleLength { get; set; } = 200;
    public int MaxDescriptionLength { get; set; } = 2000;
    public TimeSpan DefaultSla { get; set; } = TimeSpan.FromDays(7);
    public bool EnableNotifications { get; set; } = true;
}

public class OrderService
{
    private readonly OrderSettings _settings;
    
    public OrderService(IOptions<OrderSettings> settings)
    {
        _settings = settings.Value;
    }
    
    public void ValidateOrder(Order order)
    {
        if (order.Title.Length > _settings.MaxTitleLength)
            throw new ArgumentException($"Title exceeds maximum length of {_settings.MaxTitleLength}");
    }
}
```

---

## 🔄 Clean Code Principles

### 10.1 SOLID Principles

```csharp
// ✅ Correto - Princípio da Responsabilidade Única
public interface IOrderRepository
{
    Task<Order> GetByIdAsync(Guid id);
    Task<Order> AddAsync(Order order);
    Task<Order> UpdateAsync(Order order);
    Task DeleteAsync(Guid id);
}

public interface IOrderService
{
    Task<OrderDto> CreateOrderAsync(CreateOrderCommand command);
    Task<OrderDto> GetOrderAsync(Guid id);
    Task<bool> UpdateStatusAsync(Guid id, OrderStatus status);
}

// ✅ Correto - Princípio Aberto/Fechado
public abstract class OrderProcessor
{
    public abstract Task<OrderResult> ProcessAsync(Order order);
}

public class HighPriorityOrderProcessor : OrderProcessor
{
    public override async Task<OrderResult> ProcessAsync(Order order)
    {
        // Processamento específico para ordens de alta prioridade
        return await Task.FromResult(new OrderResult { Success = true });
    }
}

public class NormalOrderProcessor : OrderProcessor
{
    public override async Task<OrderResult> ProcessAsync(Order order)
    {
        // Processamento padrão
        return await Task.FromResult(new OrderResult { Success = true });
    }
}
```

### 10.2 Dependency Injection

```csharp
// ✅ Correto - Registrações claras e descritivas
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOrderServices(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        services.Configure<OrderSettings>(configuration.GetSection("OrderSettings"));
        
        services.AddScoped<IOrderRepository, EfOrderRepository>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IOrderProcessor, OrderProcessor>();
        
        // Com MediatR para CQRS
        services.AddMediatR(typeof(CreateOrderCommand).Assembly);
        
        return services;
    }
}
```

---

## 📚 Resumo das Convenções

### ✅ **Do's (Fazer):**

- Usar **PascalCase** para classes, métodos e propriedades
- Usar **camelCase** para variáveis e parâmetros
- Documentar métodos públicos com XML comments
- Usar `async/await` consistentemente
- Implementar tratamento de exceções robusto
- Usar validação de input com FluentValidation
- Escrever testes unitários com padrão AAA
- Seguir princípios SOLID
- Usar dependency injection
- Implementar logging estruturado

### ❌ **Don'ts (Não Fazer):**

- Usar abreviações em nomes de classes/métodos
- Deixar métodos sem documentação
- Ignorar warnings do compilador
- Usar `async void` (exceto event handlers)
- Tratar exceções genéricas sem logging
- Criar classes com muitas responsabilidades
- Usar strings mágicas sem constantes
- Misturar responsabilidades de diferentes camadas

### 📊 **Métricas de Qualidade:**

- **Complexidade ciclomática**: < 10 por método
- **Cobertura de testes**: > 80%
- **Documentação pública**: 100%
- **SonarQube Quality Gate**: Aprovado
- **Warnings**: Zero

---

**Próximos passos:**
- **[Blazor Guidelines](blazor-guidelines.md)** - Diretrizes específicas para Blazor
- **[Naming Conventions](naming-conventions.md)** - Convenções detalhadas de nomenclatura
- **[Documentation Standards](documentation.md)** - Padrões de documentação

---

**Última atualização:** 26 de novembro de 2025  
**Versão:** 1.0.0  
**Status:** ✅ Padrões consolidados e implementados