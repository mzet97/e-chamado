# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**EChamado** is a ticket/order management system built with .NET 9, Blazor WebAssembly, and OpenIddict for SSO/OIDC authentication. The project implements Clean Architecture with CQRS pattern using Paramore.Brighter (recently migrated from MediatR).

**Current Status**: 75-80% complete (Phases 1-3 done)
- Backend: ~85% complete
- Frontend: ~70% complete
- Testing: Planned for Phase 6
- CI/CD: Pipeline configured but tests incomplete

## Solution Structure

The solution contains 10 C# projects organized into three main application components:

### Server (Backend - Clean Architecture)
- **EChamado.Server** - ASP.NET Core API with Minimal API endpoints
- **EChamado.Server.Application** - CQRS handlers, commands, queries, validators (Paramore.Brighter)
- **EChamado.Server.Domain** - Domain entities, events, repository interfaces, domain services
- **EChamado.Server.Infrastructure** - EF Core, repositories, external services (PostgreSQL, Redis, RabbitMQ)

### Client (Frontend)
- **EChamado.Client** - Blazor WebAssembly with MudBlazor UI
- **EChamado.Client.Application** - Client-side handlers and use cases

### Auth Server
- **Echamado.Auth** - Blazor Server app for OpenIddict authentication flows

### Shared
- **EChamado.Shared** - Common DTOs, ViewModels, base classes shared between client and server

### Tests
- **EChamado.Server.UnitTests** - Unit tests (xUnit, Moq, FluentAssertions, AutoFixture)
- **EChamado.Server.IntegrationTests** - API integration tests (WebApplicationFactory)

## Architecture Patterns

### CQRS with Paramore.Brighter
The application recently migrated from MediatR to Paramore.Brighter for CQRS implementation. All commands and queries follow this pattern:

**Commands** (write operations):
- Located in `Server/EChamado.Server.Application/UseCases/{Entity}/Commands/`
- Handlers implement `IHandleRequests<TCommand>`
- Use `ValidationHandler<T>` and `UnhandledExceptionHandler<T>` as decorators
- Example: `CreateOrderCommand` → `CreateOrderCommandHandler`

**Queries** (read operations):
- Located in `Server/EChamado.Server.Application/UseCases/{Entity}/Queries/`
- Handlers implement `IHandleRequests<TQuery>`
- Example: `GetOrderByIdQuery` → `GetOrderByIdQueryHandler`

**Sending requests**:
```csharp
// Inject IAmACommandProcessor
private readonly IAmACommandProcessor _commandProcessor;

// Send command/query
var result = await _commandProcessor.SendAsync(new CreateOrderCommand(...));
```

### Domain Events
Domain entities raise events which are handled by notification handlers:
- Events: `Server/EChamado.Server.Domain/Domains/{Entity}/Events/`
- Handlers: `Server/EChamado.Server.Application/UseCases/{Entity}/Notifications/`

### Repository Pattern
- Generic `IRepository<TEntity>` interface with common CRUD operations
- Specialized repositories (e.g., `IOrderRepository`) extend the base interface
- `IUnitOfWork` manages transactions
- Located in `Server/EChamado.Server.Domain/Repositories/`

### Minimal API Endpoints
The API uses Minimal API pattern with endpoint classes:
- Each endpoint implements `IEndpoint` interface
- Located in `Server/EChamado.Server/Endpoints/{Entity}/`
- Registered via `MapEndpoints()` in `Program.cs`
- Example: `CreateOrderEndpoint.cs` contains route definition and handler

## Development Commands

### Build & Run

**Build entire solution:**
```bash
cd src/EChamado
dotnet restore
dotnet build
```

**Run all three applications (required for full functionality):**

```bash
# Terminal 1 - Auth Server (port 7132)
cd src/EChamado/Echamado.Auth
dotnet run

# Terminal 2 - API Server (port 7001)
cd src/EChamado/Server/EChamado.Server
dotnet run

# Terminal 3 - Blazor Client (port 7274)
cd src/EChamado/Client/EChamado.Client
dotnet run
```

**Run with Docker Compose (infrastructure only):**
```bash
cd src/EChamado
docker-compose up -d postgres redis rabbitmq elasticsearch logstash kibana
```

### Database Migrations

**Create new migration:**
```bash
cd src/EChamado/Server/EChamado.Server
dotnet ef migrations add <MigrationName> --project ../EChamado.Server.Infrastructure
```

**Apply migrations:**
```bash
cd src/EChamado/Server/EChamado.Server
dotnet ef database update
```

**Remove last migration:**
```bash
cd src/EChamado/Server/EChamado.Server
dotnet ef migrations remove --project ../EChamado.Server.Infrastructure
```

### Testing

**Run all tests:**
```bash
dotnet test
```

**Run specific test project:**
```bash
dotnet test tests/EChamado.Server.UnitTests/EChamado.Server.UnitTests.csproj
dotnet test tests/EChamado.Server.IntegrationTests/EChamado.Server.IntegrationTests.csproj
```

**Run single test class:**
```bash
dotnet test --filter FullyQualifiedName~CategoryValidationTests
```

**Run with coverage:**
```bash
dotnet test --collect:"XPlat Code Coverage" --results-directory ./TestResults
```

### Code Quality

**Format code:**
```bash
dotnet format
```

**Verify formatting (CI check):**
```bash
dotnet format --verify-no-changes
```

## Key Configuration Files

### Server Configuration
- `src/EChamado/Server/EChamado.Server/appsettings.json` - API server settings
- Connection strings for PostgreSQL, Redis, RabbitMQ
- OpenIddict authority URL
- Serilog configuration

### Client Configuration
- `src/EChamado/Client/EChamado.Client/wwwroot/appsettings.json`
- OIDC provider settings
- Backend API URL

### Auth Server Configuration
- `src/EChamado/Echamado.Auth/appsettings.json`
- OpenIddict server configuration

### Environment Variables
- `src/EChamado/.env` - Docker Compose environment variables (PostgreSQL, Redis, RabbitMQ, Elastic credentials)

## Authentication Flow

The system uses **OpenIddict** for OAuth 2.0 / OpenID Connect authentication.

### Architecture
1. **Echamado.Auth** (port 7132) - OpenIddict authorization server
2. **EChamado.Client** - Blazor WASM with OIDC authentication (Authorization Code + PKCE)
3. **EChamado.Server** - API validates tokens from OpenIddict

### Supported Grant Types

The OpenIddict server supports multiple authentication flows:

**1. Authorization Code + PKCE** (for SPAs like Blazor)
- Client: `bwa-client`
- Used by: EChamado.Client (Blazor WASM)
- Redirect URI: `https://localhost:7274/authentication/login-callback`

**2. Password Grant** (for mobile/desktop apps, scripts)
- Client: `mobile-client`
- Endpoint: `POST https://localhost:7132/connect/token`
- Used by: Mobile apps, desktop applications, CLI tools, automated scripts

**3. Client Credentials** (for M2M - Machine to Machine)
- Used by: Backend services, APIs, scheduled jobs
- Add new clients in `OpenIddictWorker.cs`

**4. Refresh Token**
- All clients support token refresh
- Endpoint: `POST https://localhost:7132/connect/token`

### Testing Authentication

Test scripts are available in the root directory:

```bash
# Bash/Linux/WSL
./test-openiddict-login.sh

# PowerShell/Windows
.\test-openiddict-login.ps1

# Python
python test-openiddict-login.py
```

### Obtaining Tokens for API Calls

**For testing/mobile apps (Password Grant):**
```bash
curl -X POST https://localhost:7132/connect/token \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=password" \
  -d "username=admin@admin.com" \
  -d "password=Admin@123" \
  -d "client_id=mobile-client" \
  -d "scope=openid profile email roles api chamados"
```

**Using the token:**
```bash
curl -X GET https://localhost:7296/v1/categories \
  -H "Authorization: Bearer {ACCESS_TOKEN}"
```

**For more examples:** See [`docs/AUTENTICACAO-SISTEMAS-EXTERNOS.md`](docs/AUTENTICACAO-SISTEMAS-EXTERNOS.md) and [`docs/exemplos-autenticacao-openiddict.md`](docs/exemplos-autenticacao-openiddict.md)

### Default Users
(seeded in database)
- Admin: `admin@admin.com` / `Admin@123`
- User: `user@echamado.com` / `User@123`

**CORS Configuration**: Client (7274) and Auth (7132) are whitelisted in API CORS policy.

## Domain Model

### Core Entities

**Order** - Main ticket/order entity
- Properties: Title, Description, DueDate, Status, Priority
- Relations: Category, SubCategory, Department, OrderType, StatusType, AssignedUser, CreatedByUser
- Navigation: Comments collection

**Category/SubCategory** - Hierarchical categorization system

**Department** - Organizational units

**OrderType** - Types of orders (Incident, Request, Change, etc.)

**StatusType** - Workflow states (Open, In Progress, Resolved, Closed)

**Comment** - Order comments/notes

### Identity Entities
Uses ASP.NET Core Identity with custom entities:
- `ApplicationUser`, `ApplicationRole`, `ApplicationUserClaim`, etc.
- Located in `Server/EChamado.Server.Domain/Domains/Identities/`

## Frontend Structure (Blazor WASM)

**Pages:**
- `Pages/Home.razor` - Dashboard with charts and statistics
- `Pages/Orders/OrderList.razor` - Main order listing with 7 filters
- `Pages/Orders/OrderForm.razor` - Create/Edit orders
- `Pages/Orders/OrderDetails.razor` - Order details with comments
- `Pages/Admin/*` - Admin pages for managing categories, departments, etc.

**Services:**
- `Services/OrderService.cs` - HTTP client for orders API
- `Services/CategoryService.cs` - Categories/subcategories API
- `Services/DepartmentService.cs` - Departments API
- `Services/LookupService.cs` - Cached lookups (statuses, types, etc.)

**Authentication:**
- Uses `RemoteAuthenticationService` with OIDC
- `BaseAddressAuthorizationMessageHandler` automatically adds auth tokens to API calls

## Infrastructure Services

### PostgreSQL (port 5432)
- Main database
- Entity Framework Core with code-first migrations
- PgAdmin available at http://localhost:15432

### Redis (port 6379)
- Distributed cache
- Output caching for API responses
- Session storage

### RabbitMQ (port 5672, management: 15672)
- Message bus for domain events
- Management UI: http://localhost:15672

### ELK Stack
- Elasticsearch (port 9200) - Log storage
- Logstash (ports 5044-5046) - Log processing
- Kibana (port 5601) - Log visualization UI
- Serilog writes structured logs to Logstash

## Development Workflow

### Adding a New Entity (Full CQRS)

1. **Domain Layer** - Create entity and events:
   - `Server/EChamado.Server.Domain/Domains/{EntityName}/Entity.cs`
   - `Server/EChamado.Server.Domain/Domains/{EntityName}/Events/`
   - `Server/EChamado.Server.Domain/Repositories/I{EntityName}Repository.cs`

2. **Infrastructure Layer** - Persistence:
   - `Server/EChamado.Server.Infrastructure/Persistence/Mappings/{EntityName}Mapping.cs`
   - `Server/EChamado.Server.Infrastructure/Persistence/Repositories/{EntityName}Repository.cs`
   - Register repository in `DependencyInjectionConfig.cs`
   - Create migration: `dotnet ef migrations add Add{EntityName}`

3. **Application Layer** - CQRS:
   - `Server/EChamado.Server.Application/UseCases/{EntityName}/Commands/`
   - `Server/EChamado.Server.Application/UseCases/{EntityName}/Queries/`
   - `Server/EChamado.Server.Application/UseCases/{EntityName}/ViewModels/`
   - Validators using FluentValidation

4. **API Layer** - Endpoints:
   - `Server/EChamado.Server/Endpoints/{EntityName}/` - Create endpoint classes
   - Register in `Endpoints/Endpoint.cs` MapEndpoints method

5. **Client Layer** - Blazor UI:
   - `Client/EChamado.Client/Services/{EntityName}Service.cs`
   - `Client/EChamado.Client/Pages/{EntityName}/` - Razor components
   - `Client/EChamado.Client/Models/{EntityName}Models.cs` - Client-side DTOs

### Adding a Unit Test

Tests use xUnit, Moq, FluentAssertions, and AutoFixture:

```csharp
public class CreateOrderCommandHandlerTests
{
    private readonly Mock<IOrderRepository> _mockRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly CreateOrderCommandHandler _handler;

    public CreateOrderCommandHandlerTests()
    {
        _mockRepository = new Mock<IOrderRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _handler = new CreateOrderCommandHandler(_mockRepository.Object, _mockUnitOfWork.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateOrder_WhenCommandIsValid()
    {
        // Arrange
        var command = new CreateOrderCommand { /* ... */ };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        _mockRepository.Verify(x => x.AddAsync(It.IsAny<Order>()), Times.Once);
    }
}
```

## Important Project Conventions

### Naming Conventions
- Commands: `{Action}{Entity}Command` (e.g., `CreateOrderCommand`)
- Handlers: `{Action}{Entity}CommandHandler` / `{Query}QueryHandler`
- Events: `{Entity}{Action}` (e.g., `OrderCreated`)
- Repositories: `I{Entity}Repository` / `{Entity}Repository`
- Endpoints: `{Action}{Entity}Endpoint`

### Response Patterns
- Use `BaseResult<T>` for single item responses
- Use `BaseResultList<T>` for paginated collections
- Include success/error status and messages

### Validation
- FluentValidation validators in `Application/UseCases/{Entity}/Validators/`
- Domain validation in entity constructors and methods
- Validation pipeline automatically applied via `ValidationHandler<T>`

### Error Handling
- `UnhandledExceptionHandler<T>` catches exceptions globally
- Custom exceptions: `NotFoundException`, `ValidationException`, `ForbiddenAccessException`
- Located in `Server/EChamado.Server.Domain/Exceptions/`

## Pending Work (Phases 4-6)

Detailed plans are available in [`docs/PLANO-FASES-4-6.md`](docs/PLANO-FASES-4-6.md) (1,088 lines). Key remaining work:

**Phase 4** (5-6 days): Complete admin UI
- Admin pages for Categories, Departments, OrderTypes, StatusTypes
- Comments system integration

**Phase 5** (1-2 days): Monitoring & Health Checks
- Health check endpoints for PostgreSQL, Redis, RabbitMQ
- Request/performance logging middleware

**Phase 6** (6-8 days): Testing & CI/CD
- 20+ unit tests for command/query handlers
- 10+ unit tests for validators
- 15+ integration tests for API endpoints
- Code coverage > 70%
- Complete CI/CD pipeline (already configured in `.github/workflows/ci-cd.yml`)

## Common Issues & Solutions

### Port Conflicts
If ports 5000-5002 are taken, update `launchSettings.json` in each project.

### CORS Errors
Ensure all three apps are running. Client (7274) must match CORS policy in API `Program.cs`.

### Database Connection
Check Docker containers are running: `docker ps`
Verify connection strings in `appsettings.json`

### Migration Issues
Always run migrations from `EChamado.Server` directory with `--project ../EChamado.Server.Infrastructure`

### Authentication Issues
Ensure `Echamado.Auth` is running on port 7132
Check OIDC configuration matches between client `appsettings.json` and server

## References

- **Architecture docs**: [`docs/ANALISE-COMPLETA.md`](docs/ANALISE-COMPLETA.md) - Detailed technical analysis
- **Features matrix**: [`docs/MATRIZ-FEATURES.md`](docs/MATRIZ-FEATURES.md) - Feature implementation status
- **Implementation plan**: [`docs/PLANO-IMPLEMENTACAO.md`](docs/PLANO-IMPLEMENTACAO.md) - Phases 1-3 (completed)
- **Future work**: [`docs/PLANO-FASES-4-6.md`](docs/PLANO-FASES-4-6.md) - Detailed plans for remaining phases
- **SSO setup**: [`docs/SSO-SETUP.md`](docs/SSO-SETUP.md) - Complete OpenIddict configuration guide
- **Testing guide**: [`docs/TESTING.md`](docs/TESTING.md) - Testing strategy and examples
- **Health checks**: [`docs/HEALTH-CHECKS.md`](docs/HEALTH-CHECKS.md) - Health check implementation details
