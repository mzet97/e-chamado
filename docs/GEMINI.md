# GEMINI Code Assistant Context

This document provides a comprehensive overview of the **EChamado** project, a ticket management system. It is intended to be used as a context for the Gemini code assistant to understand the project's architecture, technologies, and development conventions.

## Project Overview

EChamado is a complete ticket/issue management system with SSO/OIDC authentication, developed with .NET 9, Blazor WebAssembly, and MudBlazor.

The project follows a Clean Architecture and CQRS (Command Query Responsibility Segregation) pattern. It is containerized using Docker and consists of several services, including a PostgreSQL database, Redis for caching, and the ELK stack for logging.

### Key Technologies

*   **Backend:** .NET 9, C# 13, ASP.NET Core
*   **Frontend:** Blazor WebAssembly, MudBlazor 7.x
*   **Authentication:** OpenIddict 6.1.1, ASP.NET Core Identity
*   **Database:** PostgreSQL 15, Entity Framework Core 9
*   **Cache:** Redis 7.x
*   **Messaging:** RabbitMQ 3.x
*   **Logging:** Serilog, ELK Stack
*   **Containerization:** Docker, Docker Compose
*   **Testing:** xUnit, FluentAssertions, Moq, Testcontainers (planned)

### Architecture

The solution is divided into several projects:

*   `EChamado.Server`: The main API server, following a Clean Architecture with Domain, Application, and Infrastructure layers. It uses CQRS with MediatR.
*   `EChamado.Client`: The Blazor WebAssembly frontend application.
*   `Echamado.Auth`: A separate server for authentication, using OpenIddict.
*   `EChamado.Shared`: A shared library containing common code for the other projects.
*   A suite of test projects for unit, integration, and end-to-end testing.

## Building and Running

To build and run the project, follow these steps:

1.  **Start the infrastructure services:**
    ```bash
    docker-compose -f docker-compose.healthchecks.yml up -d
    ```

2.  **Apply database migrations:**
    ```bash
    cd src/EChamado/Server/EChamado.Server
    dotnet ef database update
    ```

3.  **Run the applications:**
    *   **Auth Server:**
        ```bash
        cd src/EChamado/Echamado.Auth
        dotnet run
        ```
    *   **API Server:**
        ```bash
        cd src/EChamado/Server/EChamado.Server
        dotnet run
        ```
    *   **Blazor Client:**
        ```bash
        cd src/EChamado/Client/EChamado.Client
        dotnet run
        ```

The applications will be available at the following URLs:

*   **Client:** `https://localhost:5002`
*   **Auth Server:** `https://localhost:5000`
*   **API:** `https://localhost:5001/swagger`

## Development Conventions

*   **CQRS:** The backend uses the CQRS pattern with MediatR. Commands and queries are located in the `EChamado.Server.Application` project.
*   **Validation:** FluentValidation is used for validating commands and other models.
*   **Testing:** The project plans to use xUnit, Moq, and FluentAssertions for unit testing, and Testcontainers for integration testing. The goal is to achieve a code coverage of over 70%.
*   **Git:** Follow the contribution guidelines in the `README.md` file. Create a feature branch, make your changes, and then open a pull request.
*   **Health Checks:** The project uses health checks to monitor the status of its services. The health check endpoints are available at `/health`, `/health/ready`, and `/health/live`.
