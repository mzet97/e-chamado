# 🏗️ Visão Geral da Arquitetura

## Arquitetura do Sistema EChamado

### 📋 Visão Geral

O EChamado é construído seguindo os princípios da **Clean Architecture** com **CQRS** (Command Query Responsibility Segregation), **DDD** (Domain-Driven Design) e padrões modernos do .NET 9.

---

## 🏛️ Arquitetura por Camadas

```mermaid
graph TB
    subgraph "Presentation Layer"
        A[Blazor WebAssembly Client]
        B[Blazor Server Auth]
        C[Swagger API]
    end
    
    subgraph "Application Layer"
        D[Commands & Queries]
        E[Handlers]
        F[Validators]
        G[DTOs]
    end
    
    subgraph "Domain Layer"
        H[Entities]
        I[Value Objects]
        J[Domain Services]
        K[Events]
        L[Interfaces]
    end
    
    subgraph "Infrastructure Layer"
        M[Repositories]
        N[External Services]
        O[Database]
        P[Cache]
        Q[Message Bus]
    end
    
    A --> D
    B --> D
    C --> D
    D --> H
    E --> M
    F --> O
    H --> M
    H --> O
    M --> O
    N --> Q
    N --> P
```

---

## 🏗️ Clean Architecture

### Estrutura de Projetos

```
src/
├── EChamado/
│   ├── Server/
│   │   ├── EChamado.Server/                    # API REST
│   │   ├── EChamado.Server.Application/       # CQRS (Commands, Queries, Handlers)
│   │   ├── EChamado.Server.Domain/            # Entidades, Eventos, Interfaces
│   │   └── EChamado.Server.Infrastructure/    # EF Core, Repositories, Configurações
│   ├── Client/
│   │   └── EChamado.Client/                   # Blazor WebAssembly
│   ├── Echamado.Auth/                         # Servidor de autenticação
│   └── EChamado.Shared/                       # DTOs e tipos compartilhados
```

### Princípios Aplicados

1. **Independence of Frameworks**: O domínio não depende de frameworks externos
2. **Testable**: Todas as regras de negócio podem ser testadas sem UI, banco de dados, etc.
3. **Independent of UI**: A UI pode mudar sem alterar as regras de negócio
4. **Independent of Database**: As regras de negócio não estão vinculadas ao banco
5. **Independent of External Agency**: As regras de negócio não sabem nada sobre o mundo exterior

---

## ⚡ CQRS (Command Query Responsibility Segregation)

### Separação de Responsabilidades

```mermaid
graph LR
    subgraph "Commands (Write)"
        A[Create Order]
        B[Update Order]
        C[Delete Order]
        D[Add Comment]
    end
    
    subgraph "Queries (Read)"
        E[Get Orders]
        F[Get Order by ID]
        G[Search Orders]
        H[Get Statistics]
    end
    
    subgraph "Command Handlers"
        I[CreateOrderHandler]
        J[UpdateOrderHandler]
        K[DeleteOrderHandler]
        L[AddCommentHandler]
    end
    
    subgraph "Query Handlers"
        M[GetOrdersHandler]
        N[GetOrderByIdHandler]
        O[SearchOrdersHandler]
        P[GetStatisticsHandler]
    end
    
    A --> I
    B --> J
    C --> K
    D --> L
    E --> M
    F --> N
    G --> O
    H --> P
```

### Vantagens do CQRS

- **Escalabilidade**: Comandos e queries podem escalar independentemente
- **Performance**: Otimizações específicas para leitura/escrita
- **Flexibilidade**: Modelos de escrita e leitura diferentes
- **Manutenibilidade**: Separação clara de responsabilidades

---

## 🔐 Autenticação e Autorização

### OpenIddict + OIDC

```mermaid
sequenceDiagram
    participant Client as Blazor Client
    participant Auth as Auth Server
    participant API as API Server
    participant DB as Database
    
    Client->>Auth: 1. Login Request
    Auth->>DB: 2. Validate Credentials
    DB-->>Auth: 3. User Data
    Auth->>Auth: 4. Generate Tokens
    Auth-->>Client: 5. Authorization Code
    Client->>Auth: 6. Exchange Code for Tokens
    Auth-->>Client: 7. Access + Refresh Token
    Client->>API: 8. Request with Access Token
    API->>Auth: 9. Validate Token
    Auth-->>API: 10. Token Valid
    API->>DB: 11. Query Data
    DB-->>API: 12. Data
    API-->>Client: 13. Response
```

### Fluxo de Autenticação

1. **Authorization Code Flow + PKCE**
2. **Refresh Token** para renovação automática
3. **Role-based Authorization** (Admin, User, Support)
4. **Secure Cookies** com SameSite=None

---

## 💾 Persistência de Dados

### Entity Framework Core 9

```mermaid
erDiagram
    ORDERS {
        Guid Id PK
        string Title
        string Description
        Guid CategoryId FK
        Guid OrderTypeId FK
        Guid StatusTypeId FK
        Guid DepartmentId FK
        Guid ResponsibleUserId FK
        string ResponsibleUserEmail
        DateTime OpeningDate
        DateTime? ClosingDate
        DateTime CreatedAt
        DateTime UpdatedAt
    }
    
    CATEGORIES {
        Guid Id PK
        string Name
        string Description
        DateTime CreatedAt
        DateTime UpdatedAt
    }
    
    DEPARTMENTS {
        Guid Id PK
        string Name
        string Description
        DateTime CreatedAt
        DateTime UpdatedAt
    }
    
    ORDERS ||--o{ COMMENTS : has
    CATEGORIES ||--o{ ORDERS : categorizes
    DEPARTMENTS ||--o{ ORDERS : handles
```

### PostgreSQL Features

- **JSON Fields**: Para dados flexíveis
- **Full-text Search**: Para busca avançada
- **Indexes**: Para performance otimizada
- **Transactions**: Para consistência de dados

---

## 🚀 Performance e Escalabilidade

### Cache Distribuído (Redis)

```mermaid
graph TB
    subgraph "Cache Strategy"
        A[Application Startup]
        B[Load Lookups]
        C[Store in Redis]
        D[Cache Results]
    end
    
    subgraph "Cache Layers"
        E[Redis Distributed Cache]
        F[In-Memory Cache]
        G[Browser Cache]
    end
    
    A --> B
    B --> C
    C --> D
    D --> E
    D --> F
    D --> G
```

### Otimizações

- **Output Caching**: Para páginas estáticas
- **Redis Cache**: Para dados frequentemente acessados
- **Connection Pooling**: Para banco de dados
- **Lazy Loading**: Para entidades relacionadas

---

## 📊 Monitoramento e Logging

### ELK Stack Integration

```mermaid
graph LR
    subgraph "Application"
        A[Serilog]
        B[Structured Logging]
    end
    
    subgraph "Transport"
        C[Logstash]
    end
    
    subgraph "Storage"
        D[Elasticsearch]
    end
    
    subgraph "Visualization"
        E[Kibana]
    end
    
    A --> C
    B --> C
    C --> D
    D --> E
```

### Health Checks

- **Database Health**: PostgreSQL connectivity
- **Cache Health**: Redis connectivity
- **Message Queue**: RabbitMQ status
- **Disk Space**: Storage availability
- **Memory Usage**: Memory consumption

---

## 🐳 Containerização

### Docker Compose Services

```mermaid
graph TB
    subgraph "Services"
        A[api-server]
        B[auth-server]
        C[client]
    end
    
    subgraph "Infrastructure"
        D[postgresql]
        E[redis]
        F[rabbitmq]
        G[elasticsearch]
        H[kibana]
        I[logstash]
    end
    
    A --> D
    A --> E
    A --> F
    A --> G
    A --> I
    B --> D
    C --> A
    C --> B
    I --> G
    G --> H
```

### Orquestração

- **Health Checks**: Monitoramento automático
- **Restart Policies**: Recuperação automática
- **Network Isolation**: Segurança entre serviços
- **Volume Persistence**: Dados persistentes

---

## 🧪 Testes

### Estrategia de Testes (289 Testes)

```mermaid
graph TB
    subgraph "Test Pyramid"
        A[Unit Tests<br/>289 cases]
        B[Integration Tests<br/>31 cases]
        C[E2E Tests<br/>4 cases]
    end
    
    subgraph "Coverage Areas"
        D[Domain Entities]
        E[Application Logic]
        F[Infrastructure]
        G[API Endpoints]
        H[UI Interactions]
    end
    
    A --> D
    A --> E
    B --> F
    B --> G
    C --> H
```

### Ferramentas

- **xUnit**: Framework principal
- **FluentAssertions**: Assertions expressivas
- **Moq**: Mocking framework
- **Testcontainers**: Integração real
- **Playwright**: E2E testing

---

## 🔄 CI/CD Pipeline

### GitHub Actions

```mermaid
graph LR
    A[Push/PR] --> B[Build]
    B --> C[Unit Tests]
    C --> D[Integration Tests]
    D --> E[Code Coverage]
    E --> F[Quality Gates]
    F --> G[Deploy]
```

### Quality Gates

- **Code Coverage**: >70%
- **Build Success**: 100%
- **Test Pass Rate**: >90%
- **Performance**: <3s response time

---

## 📈 Métricas e KPIs

### Performance Metrics

| Métrica | Target | Status |
|---------|--------|--------|
| API Response Time | <2s | ⚠️ Não medido (OpenTelemetry configurado, endpoint OTLP precisa de collector) |
| Page Load Time | <3s | ⚠️ Não medido |
| Database Query Time | <500ms | ⚠️ Não medido |
| Cache Hit Rate | >80% | ⚠️ Não medido |

> **Nota:** OpenTelemetry está configurado em `OpenTelemetryConfig.cs` (tracing + metrics via OTLP gRPC).
> Para ativar, configure um collector OTLP (ex: Jaeger, Grafana Tempo) e ajuste `OpenTelemetry:OtlpEndpoint` no appsettings.

### Business Metrics

| Métrica | Target | Status |
|---------|--------|--------|
| User Satisfaction | >4.5/5 | ⚠️ Não implementado |
| System Availability | >99.5% | ⚠️ Não implementado (health checks disponíveis) |
| Bug Rate | <1% | ✅ 0 falhas em 289 testes unitários |
| Feature Delivery | On-time | N/A |

---

## 🔮 Evolução Futura

### Microservices Migration

```mermaid
graph TB
    subgraph "Current Monolith"
        A[Single API Server]
    end
    
    subgraph "Future Microservices"
        B[Order Service]
        C[User Service]
        D[Notification Service]
        E[Reporting Service]
    end
    
    A --> B
    A --> C
    A --> D
    A --> E
```

### Tecnologias Planejadas

- **Event-Driven Architecture**: Apache Kafka
- **Service Mesh**: Istio
- **API Gateway**: Ocelot
- **Container Orchestration**: Kubernetes

---

## 📚 Próximos Passos

1. **[Diagrama de Classes](class-diagram.md)** - Modelos detalhados
2. **[Diagramas de Sequência](sequence-diagrams.md)** - Fluxos de processos
3. **[Casos de Uso](use-cases.md)** - Cenários de negócio
4. **[Padrões Arquiteturais](patterns.md)** - Implementação detalhada
5. **[Infraestrutura](infrastructure.md)** - Configurações e deploy

---

**Última atualização:** 26 de novembro de 2025  
**Versão:** 1.0.0  
**Status:** ✅ Arquitetura consolidada e testada