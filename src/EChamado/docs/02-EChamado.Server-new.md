# 🖥️ EChamado.Server - API e Servidor OpenIddict

## 🌟 Visão Geral

O `EChamado.Server` é o coração do sistema EChamado, funcionando como **API REST** e **servidor de autorização OpenIddict**. Implementa Clean Architecture com CQRS e é responsável por toda a lógica de negócio, persistência de dados e emissão de tokens JWT.

## 🏗️ Arquitetura

### 📐 Clean Architecture

O projeto segue os princípios da Clean Architecture com separação em camadas:

```
EChamado.Server/
├── Domain/           # Entidades, Agregados, Interfaces
├── Application/      # Casos de Uso, Commands, Queries
├── Infrastructure/   # Implementações, EF Core, Repositórios
└── API/             # Controllers, Endpoints, Configuração
```

### 🔄 CQRS Pattern

- **Commands**: Operações de escrita (Create, Update, Delete)
- **Queries**: Operações de leitura (Get, Search, List)
- **Handlers**: Processam Commands e Queries
- **Validators**: Validam dados de entrada

## 🔐 Servidor OpenIddict

### Funcionalidades

- **Authorization Server**: Emite tokens JWT
- **Resource Server**: Protege APIs com tokens
- **PKCE Support**: Suporte para Authorization Code + PKCE
- **Refresh Tokens**: Renovação automática de tokens

### Endpoints OpenIddict

| Endpoint | Método | Descrição |
|----------|--------|-----------|
| `/connect/authorize` | GET/POST | Autorização OAuth |
| `/connect/token` | POST | Troca de código por token |
| `/connect/userinfo` | GET | Informações do usuário |

### Configuração

```csharp
services.AddOpenIddict()
    .AddCore(options =>
    {
        options.UseEntityFrameworkCore()
               .UseDbContext<ApplicationDbContext>();
    })
    .AddServer(options =>
    {
        options.SetAuthorizationEndpointUris("/connect/authorize")
               .SetTokenEndpointUris("/connect/token");
        
        options.AllowAuthorizationCodeFlow()
               .RequireProofKeyForCodeExchange();
        
        options.UseAspNetCore()
               .EnableAuthorizationEndpointPassthrough()
               .EnableTokenEndpointPassthrough();
    });
```

## 🗄️ Banco de Dados

### Tecnologias

- **PostgreSQL**: Banco de dados principal
- **Entity Framework Core**: ORM
- **Migrations**: Controle de versão do schema

### Principais Entidades

```csharp
// Entidades de Domínio
public class Chamado : AggregateRoot
{
    public string Titulo { get; set; }
    public string Descricao { get; set; }
    public StatusChamado Status { get; set; }
    public DateTime DataCriacao { get; set; }
    public string UsuarioId { get; set; }
    public ICollection<Comentario> Comentarios { get; set; }
}

// Entidades de Identidade
public class ApplicationUser : IdentityUser
{
    public string Nome { get; set; }
    public bool Ativo { get; set; }
}
```

## 🔧 Camadas da Aplicação

### 1. Domain Layer

**Responsabilidades:**

- Entidades de negócio
- Agregados e Value Objects
- Interfaces de repositório
- Regras de negócio

**Exemplo:**

```csharp
public class Chamado : AggregateRoot
{
    public void AdicionarComentario(string texto, string autorId)
    {
        var comentario = new Comentario(texto, autorId);
        _comentarios.Add(comentario);
        
        AddDomainEvent(new ComentarioAdicionadoEvent(Id, comentario.Id));
    }
}
```

### 2. Application Layer

**Responsabilidades:**

- Commands e Queries
- Handlers
- Validators
- DTOs/ViewModels

**Exemplo:**

```csharp
public record CreateChamadoCommand(
    string Titulo,
    string Descricao,
    string UsuarioId
) : IRequest<Result<Guid>>;

public class CreateChamadoCommandHandler : IRequestHandler<CreateChamadoCommand, Result<Guid>>
{
    private readonly IChamadoRepository _repository;
    
    public async Task<Result<Guid>> Handle(CreateChamadoCommand request, CancellationToken cancellationToken)
    {
        var chamado = new Chamado(request.Titulo, request.Descricao, request.UsuarioId);
        await _repository.AddAsync(chamado);
        return Result.Success(chamado.Id);
    }
}
```

### 3. Infrastructure Layer

**Responsabilidades:**

- Implementação de repositórios
- Configuração do EF Core
- Serviços externos
- Configuração do banco

**Exemplo:**

```csharp
public class ChamadoRepository : IChamadoRepository
{
    private readonly ApplicationDbContext _context;
    
    public async Task<Chamado?> GetByIdAsync(Guid id)
    {
        return await _context.Chamados
            .Include(c => c.Comentarios)
            .FirstOrDefaultAsync(c => c.Id == id);
    }
}
```

### 4. API Layer

**Responsabilidades:**

- Endpoints REST
- Configuração do OpenIddict
- Middleware
- Dependency Injection

**Exemplo:**

```csharp
public static class ChamadoEndpoints
{
    public static void MapEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/chamados", async (CreateChamadoCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command);
            return result.IsSuccess ? Results.Created($"/api/chamados/{result.Value}", result.Value) 
                                   : Results.BadRequest(result.Error);
        }).RequireAuthorization();
    }
}
```

## 🔒 Segurança

### Autenticação e Autorização

- **JWT Bearer Tokens**: Autenticação baseada em tokens
- **Role-Based Access**: Controle de acesso baseado em papéis
- **Scopes**: Granularidade de permissões
- **HTTPS Only**: Comunicação segura obrigatória

### Configuração de Segurança

```csharp
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "https://localhost:7296";
        options.Audience = "EChamado.Server";
        options.RequireHttpsMetadata = true;
    });

services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserScope", policy => policy.RequireScope("api"));
});
```

## 📊 Logging e Monitoramento

### Serilog Configuration

```csharp
services.AddSerilog(config =>
{
    config.ReadFrom.Configuration(configuration)
          .Enrich.FromLogContext()
          .Enrich.WithMachineName()
          .WriteTo.Console()
          .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://localhost:9200"))
          {
              IndexFormat = "logs-echamado-{0:yyyy.MM.dd}"
          });
});
```

### Métricas e Telemetria

- **Application Insights**: Monitoramento de performance
- **Health Checks**: Verificação de saúde da aplicação
- **Structured Logging**: Logs estruturados com Serilog

## 🧪 Testes

### Estratégia de Testes

1. **Unit Tests**: Testes de domínio e handlers
2. **Integration Tests**: Testes de repositórios e banco
3. **E2E Tests**: Testes de fluxo completo

### Exemplo de Teste

```csharp
[Fact]
public async Task CreateChamado_WithValidData_ShouldReturnSuccess()
{
    // Arrange
    var command = new CreateChamadoCommand("Título", "Descrição", "user123");
    var handler = new CreateChamadoCommandHandler(_repository);
    
    // Act
    var result = await handler.Handle(command, CancellationToken.None);
    
    // Assert
    result.IsSuccess.Should().BeTrue();
    result.Value.Should().NotBeEmpty();
}
```

## 🚀 Deployment

### Containerização

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["EChamado.Server.csproj", "."]
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "EChamado.Server.dll"]
```

### Configuração de Produção

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=prod-postgres;Database=echamado;Username=app;Password=***"
  },
  "OpenIddict": {
    "Issuer": "https://api.echamado.com",
    "SigningCertificate": "/certs/signing.pfx",
    "EncryptionCertificate": "/certs/encryption.pfx"
  },
  "Serilog": {
    "WriteTo": [
      {
        "Name": "ApplicationInsights",
        "Args": {
          "instrumentationKey": "***"
        }
      }
    ]
  }
}
```

## 📋 Checklist de Implementação

- [ ] Configurar Clean Architecture
- [ ] Implementar CQRS com MediatR
- [ ] Configurar OpenIddict
- [ ] Implementar Entity Framework Core
- [ ] Configurar PostgreSQL
- [ ] Implementar autenticação JWT
- [ ] Configurar logging com Serilog
- [ ] Implementar testes unitários
- [ ] Configurar CI/CD
- [ ] Implementar health checks
- [ ] Configurar monitoramento
- [ ] Documentar APIs com Swagger

## 🔄 Fluxo de Desenvolvimento

### Adicionando Nova Feature

1. **Domain**: Criar entidade e interface de repositório
2. **Application**: Implementar Command/Query e Handler
3. **Infrastructure**: Implementar repositório concreto
4. **API**: Criar endpoint e mapear rota
5. **Tests**: Implementar testes unitários e integração

### Exemplo Prático

```csharp
// 1. Domain
public interface IChamadoRepository
{
    Task<Chamado?> GetByIdAsync(Guid id);
    Task AddAsync(Chamado chamado);
}

// 2. Application
public record GetChamadoQuery(Guid Id) : IRequest<ChamadoViewModel>;

public class GetChamadoQueryHandler : IRequestHandler<GetChamadoQuery, ChamadoViewModel>
{
    private readonly IChamadoRepository _repository;
    
    public async Task<ChamadoViewModel> Handle(GetChamadoQuery request, CancellationToken cancellationToken)
    {
        var chamado = await _repository.GetByIdAsync(request.Id);
        return new ChamadoViewModel(chamado.Id, chamado.Titulo, chamado.Descricao);
    }
}

// 3. Infrastructure
public class ChamadoRepository : IChamadoRepository
{
    private readonly ApplicationDbContext _context;
    
    public async Task<Chamado?> GetByIdAsync(Guid id)
    {
        return await _context.Chamados.FindAsync(id);
    }
}

// 4. API
app.MapGet("/api/chamados/{id}", async (Guid id, IMediator mediator) =>
{
    var result = await mediator.Send(new GetChamadoQuery(id));
    return Results.Ok(result);
}).RequireAuthorization();
```

## 📚 Referências

- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [CQRS Pattern](https://docs.microsoft.com/en-us/azure/architecture/patterns/cqrs)
- [OpenIddict Documentation](https://documentation.openiddict.com/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [MediatR](https://github.com/jbogard/MediatR)
