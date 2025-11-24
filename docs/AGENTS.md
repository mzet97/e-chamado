# Repository Guidelines

## Project Structure & Module Organization
`src/EChamado/EChamado.sln` ties together three deployable apps: `Server/EChamado.Server` (ASP.NET Core API + CQRS pipeline), `Client/EChamado.Client` (Blazor WebAssembly), and `Echamado.Auth` (SSO host). Shared contracts live in `src/EChamado/EChamado.Shared`. Tests sit both under `/tests` and the mirrored tree in `src/EChamado/Tests` (Client, Server, Shared, Auth, E2E). Infra manifests (`docker-compose.yml`, `docker-compose.healthchecks.yml`, `coverlet.runsettings`) sit beside the solution for quick access, and reference docs such as `README.md` and `SSO-SETUP.md` stay at the repo root.

## Build, Test, and Development Commands
Run `dotnet restore src/EChamado/EChamado.sln` before any change. Build everything with `dotnet build src/EChamado/EChamado.sln -c Release`. Local services use Docker: `docker compose -f docker-compose.healthchecks.yml up -d` brings PostgreSQL, Redis, RabbitMQ, and ELK online. Launch apps individually with `dotnet run --project src/EChamado/Server/EChamado.Server`, `.../Client/EChamado.Client`, or `.../Echamado.Auth`.

## Coding Style & Naming Conventions
The codebase targets .NET 9/C# 13 with 4-space indentation and `nullable enable`. Follow Clean Architecture boundaries: Domain stays persistence-agnostic, Application exposes MediatR/Brighter handlers, Infrastructure wires EF Core/PostgreSQL, Server hosts controllers. Use `PascalCase` for public types, `camelCase` locals, suffix async work with `Async`, and prefix interfaces with `I`. Run `dotnet format src/EChamado/EChamado.sln` before committing; the solution’s analyzers flag spacing, ordering, and nullability issues.

## Testing Guidelines
Unit and integration tests use xUnit, FluentAssertions, Moq, AutoFixture, and Testcontainers (see `TESTING.md`). Keep filenames as `FeatureNameTests.cs`, and name methods `Method_State_Expectation`. Execute fast suites with `dotnet test tests/EChamado.Server.UnitTests/EChamado.Server.UnitTests.csproj`. Full coverage (target ≥70%) uses `dotnet test src/EChamado/EChamado.sln --settings src/EChamado/coverlet.runsettings /p:CollectCoverage=true`. Pull requests touching API contracts should add integration coverage inside `tests/EChamado.Server.IntegrationTests/Endpoints`.

## Commit & Pull Request Guidelines
History follows Conventional Commits (`feat:`, `fix:`, `chore:`); keep subjects in Portuguese to match existing log (`feat: Implementar FASE 6 - Testes e CI/CD completo`). Each PR must adopt `PR-TEMPLATE.md`, summarizing scope, listing features, and calling out remaining phases. Include testing evidence (command + result), screenshots for UI tweaks, and link any tracking issue or roadmap item (e.g., FASE 4-6). Rebase before opening a PR and ensure CI can run `dotnet build` and `dotnet test` without extra secrets.

## Security & Configuration Tips
Secrets never live in source; follow `SSO-SETUP.md` to provision identity secrets via user secrets or environment variables before running `dotnet user-secrets set`. Certificates and connection strings referenced in `docker-compose.healthchecks.yml` must match your local `.env`; avoid checking `.env` into git. When touching authentication, update both `Echamado.Auth` and the Blazor client `appsettings*.json` to keep redirect URIs aligned. Rotate seeded admin credentials in `EChamado.Server.Infrastructure` if you demo publicly.
