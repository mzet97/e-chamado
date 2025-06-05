# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Architecture Overview

EChamado is a ticket management system built with Clean Architecture using .NET 9, featuring:

**Frontend**: Blazor WebAssembly with MudBlazor UI components and PWA capabilities
**Backend**: ASP.NET Core Web API with CQRS pattern using MediatR
**Authentication**: OpenID Connect with Authorization Code + PKCE flow using OpenIddict
**Database**: PostgreSQL with Entity Framework Core
**Caching**: Redis for session storage and output caching
**Messaging**: RabbitMQ for event-driven communication
**Logging**: ELK Stack (Elasticsearch, Logstash, Kibana) with Serilog structured logging
**Observability**: OpenTelemetry for distributed tracing

## Project Structure

```
├── Client/
│   ├── EChamado.Client/              # Blazor WebAssembly app
│   └── EChamado.Client.Application/  # Client application layer
├── Server/
│   ├── EChamado.Server/              # Web API (presentation layer)
│   ├── EChamado.Server.Application/  # Application layer (CQRS handlers)
│   ├── EChamado.Server.Domain/       # Domain layer (entities, events)
│   └── EChamado.Server.Infrastructure/ # Infrastructure layer (EF, repositories)
├── EChamado.Shared/                  # Shared DTOs and contracts
└── Echamado.Auth/                    # Blazor Server authentication app
```

## Development Commands

### Infrastructure Setup
```bash
# Setup all infrastructure services (PostgreSQL, Redis, RabbitMQ, ELK Stack)
./run.sh

# Or manually start infrastructure
docker compose up -d

# Create network if needed
docker network create echamado-network
```

### Application Development
```bash
# Restore dependencies
dotnet restore EChamado.sln

# Build entire solution
dotnet build EChamado.sln

# Run API server (https://localhost:7296)
dotnet run --project Server/EChamado.Server

# Run Blazor WebAssembly client (https://localhost:7274)
dotnet run --project Client/EChamado.Client

# Run Blazor Server auth app (https://localhost:7132)
dotnet run --project Echamado.Auth
```

### Database Operations
```bash
# Add migration
dotnet ef migrations add MigrationName --project Server/EChamado.Server.Infrastructure --startup-project Server/EChamado.Server

# Update database
dotnet ef database update --project Server/EChamado.Server.Infrastructure --startup-project Server/EChamado.Server

# Remove last migration
dotnet ef migrations remove --project Server/EChamado.Server.Infrastructure --startup-project Server/EChamado.Server
```

### Development URLs
- **API Server**: https://localhost:7296
- **Swagger UI**: https://localhost:7296/swagger
- **Blazor Client**: https://localhost:7274
- **Auth Server**: https://localhost:7132
- **Kibana**: http://localhost:5601
- **pgAdmin**: http://localhost:15432
- **RabbitMQ Management**: http://localhost:15672

## Key Architecture Patterns

### CQRS with MediatR
- **Commands**: Modify state (CreateDepartmentCommand, UpdateDepartmentCommand)
- **Queries**: Read data (GetByIdDepartmentQuery, SearchDepartmentQuery)
- **Handlers**: Process commands/queries in `Server/EChamado.Server.Application/UseCases/*/Commands/Handlers/` and `Server/EChamado.Server.Application/UseCases/*/Queries/Handlers/`
- **Notifications**: Domain events handled by notification handlers

### Domain-Driven Design
- **Entities**: Domain objects with identity in `Server/EChamado.Server.Domain/Domains/`
- **Aggregate Roots**: Consistency boundaries (extend `AggregateRoot` from `EChamado.Shared`)
- **Domain Events**: Business events in `Server/EChamado.Server.Domain/Domains/Orders/Events/`
- **Value Objects**: Immutable objects representing concepts
- **Domain Services**: Business logic that doesn't belong to a single entity

### Repository Pattern
- **Interfaces**: `Server/EChamado.Server.Domain/Repositories/`
- **Implementations**: `Server/EChamado.Server.Infrastructure/Persistence/Repositories/`
- **Unit of Work**: Transaction management via `IUnitOfWork`

### Minimal API Endpoints
- **Endpoint Pattern**: `Server/EChamado.Server/Endpoints/` organized by feature
- **Base Class**: Inherit from `Endpoint` class for consistent structure
- **Registration**: Auto-discovery via reflection in `Program.cs`

## Authentication Flow

The system uses OpenID Connect with a separate Blazor Server authentication app:

1. **Client** redirects to **Auth Server** (`Echamado.Auth`) for login
2. **Auth Server** handles user authentication and consent
3. **Auth Server** redirects back with authorization code
4. **Client** exchanges code for access token using PKCE
5. **Client** uses access token for **API Server** calls

## Development Guidelines

### Adding New Features
1. **Domain**: Create entities, events, and validation in `Server/EChamado.Server.Domain/`
2. **Application**: Add commands/queries with handlers in `Server/EChamado.Server.Application/UseCases/`
3. **Infrastructure**: Add repositories and mappings in `Server/EChamado.Server.Infrastructure/`
4. **API**: Create endpoints in `Server/EChamado.Server/Endpoints/`
5. **Client**: Add Blazor components and services in `Client/EChamado.Client/`

### Database Changes
- Always create migrations for schema changes
- Use FluentAPI configuration in `Server/EChamado.Server.Infrastructure/Persistence/Mappings/`
- Entity validation is handled in domain layer with `EntityValidation` base class

### Testing
- No test projects currently exist
- When adding tests, follow AAA pattern (Arrange, Act, Assert)
- Use xUnit for unit tests and integration tests
- Mock external dependencies using interfaces

### Client Development
- Use MudBlazor components for consistent UI
- Implement authentication handlers for API calls
- Follow Blazor WebAssembly best practices for performance
- Use dependency injection for services

### Error Handling
- Domain exceptions in `Server/EChamado.Server.Domain/Exceptions/`
- Global exception handling via middleware in `Server/EChamado.Server/Extensions/CustomExceptionHandler.cs`
- Validation errors use FluentValidation with `ValidationBehaviour`

## Environment Configuration

Infrastructure services use environment variables from `.env` file:
- PostgreSQL: `POSTGRES_USER`, `POSTGRES_PASSWORD`, `POSTGRES_DB`
- Redis: `REDIS_PASSWORD`  
- RabbitMQ: `RABBITMQ_USER`, `RABBITMQ_PASS`
- Elasticsearch: `ELASTIC_PASSWORD`, `KIBANA_PASSWORD`
- pgAdmin: `PGADMIN_DEFAULT_EMAIL`, `PGADMIN_DEFAULT_PASSWORD`