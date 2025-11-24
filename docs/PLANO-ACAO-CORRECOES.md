# Plano de Ação - Correções e Próximos Passos do Projeto EChamado

**Data de Criação:** 2025-11-12
**Responsável:** Equipe de Desenvolvimento
**Status Atual:** 🔴 Aplicação não inicia devido a dependências obrigatórias (Redis/RabbitMQ)

---

## 📊 Status Atual do Projeto

### ✅ O que está funcionando
- ✅ Build compila com sucesso (45 warnings, 0 errors)
- ✅ Paramore.Brighter configurado corretamente
- ✅ Swagger configurado no Program.cs
- ✅ Clean Architecture implementada
- ✅ CQRS com Commands, Queries e Notifications
- ✅ Pipeline de validação e logging configurado
- ✅ OpenIddict/Identity configurado

### 🔴 Problemas Críticos Identificados

#### 1. **Dependências Obrigatórias Falhando ao Iniciar**
```
Unable to resolve service for type 'IMessageBusClient'
Unable to resolve service for type 'IDistributedCache'
```

**Causa:**
- Redis e RabbitMQ são configurados como dependências **obrigatórias**
- Servidores de dev não estão disponíveis nos endereços configurados:
  - Redis: `redis.home.arpa:30379`
  - RabbitMQ: `rabbitmq-mgmt.home.arpa:5672`

**Impacto:**
- ⚠️ **CRÍTICO** - Aplicação não inicia

---

## 🎯 Plano de Ação em Fases

---

## 📋 FASE 1: Correção Imediata - Dependências Opcionais (URGENTE)
**Tempo Estimado:** 2-3 horas
**Prioridade:** 🔴 CRÍTICA
**Objetivo:** Fazer a aplicação iniciar sem Redis e RabbitMQ

### 1.1. Tornar Redis Opcional

**Arquivo:** `EChamado.Server.Infrastructure/Configuration/RedisConfig.cs`

**Ação:**
```csharp
public static IServiceCollection AddRedisCache(
    this IServiceCollection services,
    IConfiguration configuration,
    bool optional = true)
{
    var redisConfiguration = configuration.GetSection("Redis:ConnectionString").Value;

    if (string.IsNullOrEmpty(redisConfiguration) && optional)
    {
        // Fallback para MemoryCache se Redis não estiver configurado
        services.AddDistributedMemoryCache();
        Console.WriteLine("⚠️ Redis não configurado - usando MemoryCache");
        return services;
    }

    try
    {
        services.AddSingleton<IConnectionMultiplexer>(sp =>
            ConnectionMultiplexer.Connect(redisConfiguration));

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConfiguration;
            options.InstanceName = configuration.GetSection("Redis:InstanceName").Value;
        });

        Console.WriteLine("✅ Redis configurado com sucesso");
    }
    catch (Exception ex)
    {
        if (optional)
        {
            Console.WriteLine($"⚠️ Erro ao conectar ao Redis: {ex.Message}");
            Console.WriteLine("⚠️ Usando MemoryCache como fallback");
            services.AddDistributedMemoryCache();
        }
        else
        {
            throw;
        }
    }

    return services;
}
```

### 1.2. Tornar RabbitMQ Opcional

**Arquivo:** `EChamado.Server.Infrastructure/Configuration/MessageBusConfig.cs`

**Ação:**
```csharp
public static IServiceCollection AddMessageBus(
    this IServiceCollection services,
    IConfiguration configuration,
    bool optional = true)
{
    var rabbitMqSection = configuration.GetSection("RabbitMq");
    var rabbitMq = rabbitMqSection.Get<RabbitMq>();

    if (rabbitMq == null && optional)
    {
        // Registrar implementação fake/null
        services.AddSingleton<IMessageBusClient, NullMessageBusClient>();
        Console.WriteLine("⚠️ RabbitMQ não configurado - usando NullMessageBusClient");
        return services;
    }

    try
    {
        var connectionFactory = new ConnectionFactory
        {
            HostName = rabbitMq.HostName,
            Port = rabbitMq.Port,
            UserName = rabbitMq.Username,
            Password = rabbitMq.Password
        };

        services.AddSingleton(async serviceProvider =>
        {
            var connection = await connectionFactory.CreateConnectionAsync(
                rabbitMq.ClientProviderName);
            return connection;
        });

        services.AddSingleton(serviceProvider =>
        {
            var connectionTask = serviceProvider.GetRequiredService<Task<IConnection>>();
            var connection = connectionTask.GetAwaiter().GetResult();
            return new ProducerConnection(connection);
        });

        services.AddSingleton<IMessageBusClient>(serviceProvider =>
        {
            var producerConnection = serviceProvider.GetRequiredService<ProducerConnection>();
            return new RabbitMqClient(producerConnection);
        });

        Console.WriteLine("✅ RabbitMQ configurado com sucesso");
    }
    catch (Exception ex)
    {
        if (optional)
        {
            Console.WriteLine($"⚠️ Erro ao conectar ao RabbitMQ: {ex.Message}");
            Console.WriteLine("⚠️ Usando NullMessageBusClient como fallback");
            services.AddSingleton<IMessageBusClient, NullMessageBusClient>();
        }
        else
        {
            throw;
        }
    }

    return services;
}
```

### 1.3. Criar NullMessageBusClient

**Arquivo:** `EChamado.Server.Infrastructure/MessageBus/NullMessageBusClient.cs` (NOVO)

**Ação:**
```csharp
using EChamado.Server.Domain.Services.Interface;
using Microsoft.Extensions.Logging;

namespace EChamado.Server.Infrastructure.MessageBus;

/// <summary>
/// Implementação nula do IMessageBusClient para desenvolvimento sem RabbitMQ
/// </summary>
public class NullMessageBusClient : IMessageBusClient
{
    private readonly ILogger<NullMessageBusClient> _logger;

    public NullMessageBusClient(ILogger<NullMessageBusClient> logger)
    {
        _logger = logger;
    }

    public Task Publish(object message, string routingKey, string exchange,
                       string type, string queueName)
    {
        _logger.LogInformation(
            "[DEV-MODE] Mensagem NÃO publicada (RabbitMQ desabilitado): " +
            "Type={Type}, RoutingKey={RoutingKey}, Exchange={Exchange}",
            type, routingKey, exchange);

        return Task.CompletedTask;
    }
}
```

### 1.4. Atualizar Program.cs

**Arquivo:** `EChamado.Server/Program.cs`

**Ação:**
```csharp
// Redis Configuration (opcional em desenvolvimento)
builder.Services.AddRedisCache(builder.Configuration, optional: true);
builder.Services.AddRedisOutputCache(builder.Configuration);

// MessageBus Configuration (opcional em desenvolvimento)
builder.Services.AddMessageBus(builder.Configuration, optional: true);
```

**Resultado Esperado:**
- ✅ Aplicação inicia mesmo sem Redis/RabbitMQ
- ⚠️ Logs indicam uso de fallback
- ✅ Funcionalidades principais funcionam (sem cache distribuído e eventos)

---

## 📋 FASE 2: Configuração de Ambiente de Desenvolvimento (1-2 dias)
**Tempo Estimado:** 4-6 horas
**Prioridade:** 🟡 ALTA
**Objetivo:** Configurar ambiente dev com Docker Compose local

### 2.1. Criar docker-compose.dev.yml Simplificado

**Arquivo:** `src/EChamado/docker-compose.dev.yml` (NOVO)

**Ação:**
```yaml
version: '3.8'

services:
  postgres-dev:
    image: postgres:16-alpine
    container_name: echamado-postgres-dev
    environment:
      POSTGRES_USER: app
      POSTGRES_PASSWORD: Admin@123
      POSTGRES_DB: e-chamado
    ports:
      - "5432:5432"
    volumes:
      - postgres_dev_data:/var/lib/postgresql/data
    networks:
      - echamado-dev

  redis-dev:
    image: redis:7-alpine
    container_name: echamado-redis-dev
    command: redis-server --requirepass Admin@123
    ports:
      - "6379:6379"
    volumes:
      - redis_dev_data:/data
    networks:
      - echamado-dev

  rabbitmq-dev:
    image: rabbitmq:3-management-alpine
    container_name: echamado-rabbitmq-dev
    environment:
      RABBITMQ_DEFAULT_USER: admin
      RABBITMQ_DEFAULT_PASS: Admin@123
    ports:
      - "5672:5672"   # AMQP
      - "15672:15672" # Management UI
    volumes:
      - rabbitmq_dev_data:/var/lib/rabbitmq
    networks:
      - echamado-dev

volumes:
  postgres_dev_data:
  redis_dev_data:
  rabbitmq_dev_data:

networks:
  echamado-dev:
    driver: bridge
```

### 2.2. Criar appsettings.Development.json

**Arquivo:** `EChamado.Server/appsettings.Development.json`

**Ação:**
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "EChamado": "Debug"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=e-chamado;User Id=app;Password=Admin@123;"
  },
  "Redis": {
    "ConnectionString": "localhost:6379,password=Admin@123",
    "InstanceName": "EChamado_Dev_"
  },
  "RabbitMQ": {
    "HostName": "localhost",
    "Port": 5672,
    "UserName": "admin",
    "Password": "Admin@123",
    "ClientProviderName": "EChamado-Dev-Client"
  }
}
```

### 2.3. Script de Inicialização

**Arquivo:** `scripts/start-dev.sh` (NOVO)

**Ação:**
```bash
#!/bin/bash

echo "🚀 Iniciando ambiente de desenvolvimento EChamado..."

# Navegar para diretório do docker-compose
cd "$(dirname "$0")/../src/EChamado"

# Iniciar serviços
echo "📦 Iniciando PostgreSQL, Redis e RabbitMQ..."
docker-compose -f docker-compose.dev.yml up -d

# Aguardar serviços ficarem prontos
echo "⏳ Aguardando serviços iniciarem..."
sleep 10

# Verificar status
echo "✅ Verificando status dos serviços..."
docker-compose -f docker-compose.dev.yml ps

echo ""
echo "🎉 Ambiente pronto!"
echo ""
echo "📊 URLs disponíveis:"
echo "  - PostgreSQL: localhost:5432"
echo "  - Redis: localhost:6379"
echo "  - RabbitMQ Management: http://localhost:15672 (admin/Admin@123)"
echo ""
echo "🏃 Execute o servidor:"
echo "  cd src/EChamado/Server/EChamado.Server"
echo "  dotnet run"
```

**Resultado Esperado:**
- ✅ Infraestrutura local disponível
- ✅ Aplicação inicia com todos os recursos
- ✅ Desenvolvimento completo funcional

---

## 📋 FASE 3: Correção de Warnings e Code Quality (2-3 dias)
**Tempo Estimado:** 8-12 horas
**Prioridade:** 🟢 MÉDIA
**Objetivo:** Reduzir warnings de 45 para < 10

### 3.1. Corrigir Warnings de Nullability (CS8767, CS8604)

**Arquivos Afetados:** ~25 arquivos

**Ação:**
```csharp
// Antes (BrighterRequest.cs)
public Id CorrelationId { get; set; } = new Id(Guid.NewGuid().ToString());

// Depois
public Id CorrelationId { get; set; } = new(Guid.NewGuid().ToString());

// Notificações - adicionar null-forgiving operator quando apropriado
public Id CorrelationId { get; set; } = default!;
```

### 3.2. Corrigir Property Hiding (CS0108)

**Arquivos:** Commands com propriedade `Id`

**Ação:**
```csharp
// Antes
public Guid Id { get; set; }

// Depois - Opção 1: Usar 'new'
public new Guid Id { get; set; }

// Depois - Opção 2: Renomear (RECOMENDADO)
public Guid EntityId { get; set; }

// Depois - Opção 3: Remover e usar BrighterRequest.Id
// (remover propriedade completamente)
```

### 3.3. Revisar Null Reference Warnings

**Arquivos:** Handlers com potenciais null references

**Ação:**
```csharp
// Adicionar null checks apropriados
if (entity == null)
{
    command.Result = new BaseResult(null, false, "Entidade não encontrada");
    return await base.HandleAsync(command, cancellationToken);
}

// Ou usar null-forgiving operator quando garantido
var result = entity!.Property;
```

**Resultado Esperado:**
- ✅ < 10 warnings no build
- ✅ Código mais seguro e limpo
- ✅ Melhor experiência de desenvolvimento

---

## 📋 FASE 4: Implementação de Testes (1-2 semanas)
**Tempo Estimado:** 40-60 horas
**Prioridade:** 🟡 ALTA
**Objetivo:** Cobertura de testes > 70%

### 4.1. Setup de Testes

**Estrutura:**
```
tests/
├── EChamado.Server.UnitTests/
│   ├── Application/
│   │   ├── Commands/
│   │   │   └── LoginUserCommandHandlerTests.cs
│   │   └── Queries/
│   │       └── GetOrderByIdQueryHandlerTests.cs
│   ├── Domain/
│   │   └── Entities/
│   │       └── OrderTests.cs
│   └── Infrastructure/
│       └── Repositories/
│           └── OrderRepositoryTests.cs
└── EChamado.Server.IntegrationTests/
    ├── Endpoints/
    │   ├── AuthEndpointsTests.cs
    │   └── OrderEndpointsTests.cs
    └── Infrastructure/
        └── IntegrationTestWebAppFactory.cs
```

### 4.2. Testes Unitários - Handlers

**Exemplo:** `LoginUserCommandHandlerTests.cs`

```csharp
public class LoginUserCommandHandlerTests
{
    private readonly Mock<IApplicationUserService> _mockUserService;
    private readonly Mock<IAmACommandProcessor> _mockCommandProcessor;
    private readonly LoginUserCommandHandler _handler;

    public LoginUserCommandHandlerTests()
    {
        _mockUserService = new Mock<IApplicationUserService>();
        _mockCommandProcessor = new Mock<IAmACommandProcessor>();
        _handler = new LoginUserCommandHandler(
            _mockUserService.Object,
            _mockCommandProcessor.Object);
    }

    [Fact]
    public async Task HandleAsync_ValidCredentials_ReturnsSuccess()
    {
        // Arrange
        var command = new LoginUserCommand("user@test.com", "Password123!");

        _mockUserService
            .Setup(x => x.PasswordSignInAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                false, false))
            .ReturnsAsync(SignInResult.Success);

        var tokenCommand = new GetTokenCommand { Email = command.Email };
        tokenCommand.Result = new BaseResult<LoginResponseViewModel>(
            new LoginResponseViewModel { Token = "fake-token" },
            true,
            "Login realizado com sucesso");

        _mockCommandProcessor
            .Setup(x => x.SendAsync(It.IsAny<GetTokenCommand>(),
                                   It.IsAny<CancellationToken>()))
            .ReturnsAsync(tokenCommand);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        result.Result.Should().NotBeNull();
        result.Result!.Success.Should().BeTrue();
        result.Result.Data.Should().NotBeNull();
        result.Result.Data!.Token.Should().NotBeEmpty();
    }

    [Fact]
    public async Task HandleAsync_InvalidCredentials_ReturnsFailure()
    {
        // Arrange
        var command = new LoginUserCommand("user@test.com", "WrongPassword");

        _mockUserService
            .Setup(x => x.PasswordSignInAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                false, false))
            .ReturnsAsync(SignInResult.Failed);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        result.Result.Should().NotBeNull();
        result.Result!.Success.Should().BeFalse();
        result.Result.Message.Should().Contain("Erro ao fazer login");
    }
}
```

### 4.3. Testes de Integração - Endpoints

**Exemplo:** `AuthEndpointsTests.cs`

```csharp
public class AuthEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public AuthEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Mock de serviços externos para testes
            });
        });

        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsOkWithToken()
    {
        // Arrange
        var loginRequest = new
        {
            Email = "admin@echamado.com",
            Password = "Admin@123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content
            .ReadFromJsonAsync<BaseResult<LoginResponseViewModel>>();

        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data!.Token.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Login_InvalidCredentials_ReturnsBadRequest()
    {
        // Arrange
        var loginRequest = new
        {
            Email = "admin@echamado.com",
            Password = "WrongPassword"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
```

### 4.4. Meta de Cobertura

| Camada | Meta de Cobertura | Prioridade |
|--------|------------------|------------|
| **Domain** | 90%+ | Alta |
| **Application (Handlers)** | 80%+ | Alta |
| **Application (Validators)** | 100% | Alta |
| **Infrastructure** | 60%+ | Média |
| **API (Endpoints)** | 70%+ | Alta |

**Resultado Esperado:**
- ✅ > 70% de cobertura total
- ✅ CI/CD com validação automática
- ✅ Confiança para refatorações

---

## 📋 FASE 5: Melhorias no Paramore Brighter (3-5 dias)
**Tempo Estimado:** 12-20 horas
**Prioridade:** 🟢 BAIXA
**Objetivo:** Aproveitar recursos avançados do Brighter

### 5.1. Implementar Retry Policies

**Arquivos:** Handlers críticos (Auth, Orders)

**Ação:**
```csharp
using Polly;
using Paramore.Brighter;

public class CreateOrderCommandHandler : RequestHandlerAsync<CreateOrderCommand>
{
    [RequestLogging(0, HandlerTiming.Before)]
    [RequestValidation(1, HandlerTiming.Before)]
    [UsePolicy(CommandProcessor.RETRYPOLICY, step: 2)]
    public override async Task<CreateOrderCommand> HandleAsync(
        CreateOrderCommand command,
        CancellationToken cancellationToken = default)
    {
        // Handler logic
    }
}

// Em DependencyInjection.cs
services.AddBrighter(options =>
{
    options.HandlerLifetime = ServiceLifetime.Scoped;
    options.PolicyRegistry = new PolicyRegistry
    {
        {
            CommandProcessor.RETRYPOLICY,
            Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(
                    3,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (exception, timeSpan, retryCount, context) =>
                    {
                        Console.WriteLine(
                            $"Retry {retryCount} após {timeSpan.TotalSeconds}s " +
                            $"devido a: {exception.Message}");
                    })
        }
    };
});
```

### 5.2. Implementar Outbox Pattern

**Objetivo:** Garantir entrega de eventos mesmo com falhas

**Ação:**
```csharp
// 1. Adicionar pacote
// Install-Package Paramore.Brighter.Outbox.PostgreSql

// 2. Configurar Outbox
services.AddBrighter(options => { ... })
    .UseOutbox(new PostgreSqlOutboxConfiguration(
        connectionString: configuration.GetConnectionString("DefaultConnection"),
        outboxTableName: "Outbox"))
    .UseOutboxSweeper(options =>
    {
        options.TimerInterval = 5; // segundos
        options.MinimumMessageAge = 5000; // ms
    });

// 3. Usar DepositPost ao invés de Send
await commandProcessor.DepositPostAsync(new OrderCreatedEvent
{
    OrderId = order.Id,
    CreatedAt = DateTime.UtcNow
});

// 4. Background worker para processar Outbox
// (já configurado com UseOutboxSweeper)
```

### 5.3. Configurar Telemetria Avançada

**Ação:**
```csharp
// Em Program.cs
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing =>
    {
        tracing
            .AddBrighterInstrumentation()
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddEntityFrameworkCoreInstrumentation()
            .AddSource("EChamado.*")
            .SetResourceBuilder(ResourceBuilder
                .CreateDefault()
                .AddService("EChamado.Server"))
            .AddOtlpExporter(options =>
            {
                options.Endpoint = new Uri("http://localhost:4317");
            });
    })
    .WithMetrics(metrics =>
    {
        metrics
            .AddBrighterInstrumentation()
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddRuntimeInstrumentation()
            .AddPrometheusExporter();
    });
```

**Resultado Esperado:**
- ✅ Maior resiliência a falhas transientes
- ✅ Garantia de entrega de eventos
- ✅ Observabilidade completa

---

## 📋 FASE 6: Documentação e DevOps (1 semana)
**Tempo Estimado:** 20-30 horas
**Prioridade:** 🟡 ALTA
**Objetivo:** Documentação completa e CI/CD funcional

### 6.1. Documentação de API

**Ferramentas:** Swagger + Markdown

**Ação:**
- Adicionar XML Comments em todos os endpoints
- Gerar documentação automática
- Criar exemplos de requisições

**Arquivo:** `docs/API.md`

### 6.2. Documentação de Arquitetura

**Ação:**
- Criar diagramas C4 (Context, Container, Component)
- Documentar fluxos principais
- Explicar decisões arquiteturais

**Arquivos:**
- `docs/ARCHITECTURE.md`
- `docs/diagrams/` (usando Mermaid ou PlantUML)

### 6.3. CI/CD Pipeline

**Arquivo:** `.github/workflows/ci-cd.yml`

**Melhorias:**
```yaml
name: CI/CD Pipeline

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main ]

jobs:
  build-and-test:
    runs-on: ubuntu-latest

    services:
      postgres:
        image: postgres:16-alpine
        env:
          POSTGRES_USER: app
          POSTGRES_PASSWORD: Admin@123
          POSTGRES_DB: e-chamado-test
        ports:
          - 5432:5432
        options: >-
          --health-cmd pg_isready
          --health-interval 10s
          --health-timeout 5s
          --health-retries 5

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

    - name: Test
      run: dotnet test --no-build --configuration Release --collect:"XPlat Code Coverage"

    - name: Code Coverage Report
      uses: codecov/codecov-action@v4
      with:
        files: ./coverage.cobertura.xml
        fail_ci_if_error: true

    - name: SonarCloud Scan
      uses: SonarSource/sonarcloud-github-action@master
      env:
        GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
        SONAR_TOKEN: ${{ secrets.SONAR_TOKEN }}
```

### 6.4. Docker para Produção

**Arquivo:** `Dockerfile` (otimizado)

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar apenas arquivos de projeto primeiro (cache de layers)
COPY ["Server/EChamado.Server/EChamado.Server.csproj", "Server/EChamado.Server/"]
COPY ["Server/EChamado.Server.Application/EChamado.Server.Application.csproj", "Server/EChamado.Server.Application/"]
COPY ["Server/EChamado.Server.Domain/EChamado.Server.Domain.csproj", "Server/EChamado.Server.Domain/"]
COPY ["Server/EChamado.Server.Infrastructure/EChamado.Server.Infrastructure.csproj", "Server/EChamado.Server.Infrastructure/"]
COPY ["EChamado.Shared/EChamado.Shared.csproj", "EChamado.Shared/"]

RUN dotnet restore "Server/EChamado.Server/EChamado.Server.csproj"

# Copiar todo o código
COPY . .

WORKDIR "/src/Server/EChamado.Server"
RUN dotnet build "EChamado.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EChamado.Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Criar usuário não-root
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

COPY --from=publish /app/publish .

EXPOSE 8080
EXPOSE 8081

ENTRYPOINT ["dotnet", "EChamado.Server.dll"]
```

**Resultado Esperado:**
- ✅ Documentação completa e acessível
- ✅ CI/CD automatizado
- ✅ Deploy simplificado

---

## 📊 Métricas de Sucesso

### Critérios de Aceitação por Fase

| Fase | Critério | Meta |
|------|----------|------|
| **Fase 1** | Aplicação inicia | ✅ 100% |
| **Fase 2** | Ambiente dev funcional | ✅ 100% |
| **Fase 3** | Warnings | < 10 |
| **Fase 4** | Cobertura de testes | > 70% |
| **Fase 5** | Features avançadas | 3+ implementadas |
| **Fase 6** | Docs completa | 100% |

### KPIs do Projeto

- **Build Time:** < 2 minutos
- **Test Execution Time:** < 5 minutos
- **Code Coverage:** > 70%
- **Technical Debt:** < 5% (SonarQube)
- **Security Vulnerabilities:** 0 críticas
- **API Response Time (P95):** < 200ms

---

## 🚨 Riscos e Mitigações

### Riscos Identificados

| Risco | Probabilidade | Impacto | Mitigação |
|-------|--------------|---------|-----------|
| Dependências externas indisponíveis | Alta | Alto | ✅ Fase 1 - Fallbacks |
| Testes levam muito tempo | Média | Médio | Paralelização, mocks |
| Cobertura < 70% | Média | Alto | Revisão contínua |
| Performance issues | Baixa | Alto | Load testing, APM |

---

## 📅 Cronograma Estimado

```
Semana 1: Fase 1 + Início Fase 2
├─ Dia 1-2: Dependências opcionais ✅
├─ Dia 3-4: Docker Compose local
└─ Dia 5: Testes e ajustes

Semana 2: Fase 2 + Fase 3
├─ Dia 1-2: Finalizar ambiente dev
├─ Dia 3-5: Corrigir warnings
└─ Fim da semana: Code review

Semana 3-4: Fase 4
├─ Setup de testes
├─ Testes unitários (handlers)
├─ Testes de integração
└─ Atingir meta de cobertura

Semana 5: Fase 5
├─ Retry policies
├─ Outbox pattern
└─ Telemetria avançada

Semana 6: Fase 6
├─ Documentação completa
├─ CI/CD otimizado
└─ Deploy em staging
```

---

## 🎯 Conclusão

### Priorização IMEDIATA (Esta Semana)

1. ✅ **FASE 1** - Tornar dependências opcionais
2. ✅ **Validar** - Aplicação iniciando
3. ✅ **Testar** - Login e endpoints básicos funcionando

### Próximos 30 Dias

1. Concluir Fase 2 e 3
2. Iniciar Fase 4 (testes)
3. Atingir 50%+ de cobertura

### Próximos 90 Dias

1. Concluir todas as 6 fases
2. Preparar para produção
3. Lançar v1.0

---

**Documento Criado Por:** Claude AI (Senior Software Engineer)
**Data:** 2025-11-12
**Versão:** 1.0
**Status:** 📋 Aguardando Aprovação
