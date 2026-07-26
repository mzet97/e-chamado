# EChamado - Sistema de Gerenciamento de Chamados

Sistema de gestao de tickets/chamados com autenticacao SSO/OIDC, desenvolvido com .NET 9, Blazor WebAssembly e MudBlazor.

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com/)
[![Blazor](https://img.shields.io/badge/Blazor-WebAssembly-512BD4)](https://blazor.net/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

---

## Funcionalidades

**Autenticacao e Autorizacao** - SSO/OIDC com Authorization Code Flow + PKCE, refresh token automatico, roles (Admin, User, Support), OpenIddict 6.1.1.

**Gestao de Chamados** - CRUD completo, listagem com paginacao server-side, 7 filtros avancados, atribuicao de responsavel, sistema de comentarios, subcategorias.

**Dashboard** - Cards com estatisticas, grafico donut (status), grafico de barras (departamento), ultimos chamados, acoes rapidas.

**API** - 55 endpoints Minimal API com Gridify (filtros dinamicos), OData (queries avancadas), AI Natural Language Query (converte linguagem natural para Gridify via OpenAI/Gemini), FluentValidation, responses padronizadas.

**Paginas Admin** - Categories, Departments, OrderTypes, StatusTypes, SubCategories (protegidas com `[Authorize(Roles = "Admin")]`).

**Monitoramento** - Health Checks (PostgreSQL, Redis, RabbitMQ), endpoints /health, /ready, /live, Serilog + ELK Stack.

---

## Arquitetura

**Backend** - Clean Architecture (Domain, Application, Infrastructure, API) com CQRS via Paramore.Brighter, Domain Events, Repository Pattern, Entity Framework Core 9 + PostgreSQL 15.

**Frontend** - Blazor WebAssembly + MudBlazor 8.x, HttpClient com autenticacao automatica, in-memory caching via LookupService, ErrorBoundary global.

**Infraestrutura** - Docker Compose (8 servicos: PostgreSQL, Redis, RabbitMQ, Elasticsearch, Logstash, Kibana, pgAdmin, setup), named volumes, secrets via environment variables.

---

## Tecnologias

| Categoria | Tecnologia |
|-----------|-----------|
| Backend | .NET 9, C# 13, ASP.NET Core |
| Frontend | Blazor WASM, MudBlazor 8.x |
| Autenticacao | OpenIddict 6.1.1, ASP.NET Core Identity |
| Banco de Dados | PostgreSQL 15, EF Core 9 |
| Queries | Gridify 2.16.3, OData 9.0 |
| IA | OpenAI GPT-4o-mini, Google Gemini 2.0, OpenRouter |
| Cache | Redis 7.x |
| Mensageria | RabbitMQ 3.x |
| Logging | Serilog 4.3.0, ELK Stack 8.15.1 |
| Testes | xUnit, FluentAssertions, Moq, Testcontainers, Playwright |

---

## Estrutura do Projeto

```
e-chamado/
├── src/EChamado/
│   ├── Server/
│   │   ├── EChamado.Server/              # API (Minimal API endpoints)
│   │   ├── EChamado.Server.Application/  # CQRS (Brighter)
│   │   ├── EChamado.Server.Domain/       # Entities, Events, Interfaces
│   │   └── EChamado.Server.Infrastructure/ # EF Core, Repositories
│   ├── Client/EChamado.Client/           # Blazor WASM + MudBlazor
│   ├── Echamado.Auth/                    # Auth server (OpenIddict 6.1.1)
│   ├── EChamado.Shared/                  # DTOs, Responses
│   └── Tests/                            # 6 test projects
│       ├── EChamado.Server.UnitTests/
│       ├── EChamado.Server.IntegrationTests/
│       ├── EChamado.E2E.Tests/
│       ├── EChamado.Shared.UnitTests/
│       ├── Echamado.Auth.UnitTests/
│       └── EChamado.Client.UnitTests/
├── docs/                                 # Technical documentation
├── docker-compose.yml
└── README.md
```

---

## Como Executar

### Pre-requisitos
- .NET 9 SDK
- Docker e Docker Compose

### Setup

```bash
git clone https://github.com/mzet97/e-chamado.git
cd e-chamado/src/EChamado
cp .env.example .env
# Edite .env com suas configuracoes
docker-compose up -d
```

### Banco de dados

```bash
cd Server/EChamado.Server
dotnet ef database update
```

### Executar (3 terminais)

```bash
# Auth server
cd Echamado.Auth && dotnet run

# API server
cd Server/EChamado.Server && dotnet run

# Blazor client
cd Client/EChamado.Client && dotnet run
```

### URLs

- **Cliente**: https://localhost:7274
- **Auth**: https://localhost:7132
- **API/Swagger**: https://localhost:7296/swagger
- **Kibana**: http://localhost:5601

### Usuarios padrao

| Perfil | Email | Senha |
|--------|-------|-------|
| Admin | admin@echamado.com | Admin@123 |
| User | user@echamado.com | User@123 |

---

## Testes

**276 unit tests passando** (222 Server + 24 Shared + 17 Auth + 13 Client). Integration tests (31) requerem Docker com Postgres e Redis. E2E tests (4) requerem ambiente completo rodando.

```bash
# Unit tests
dotnet test src/EChamado/EChamado.sln --filter "FullyQualifiedName~UnitTests"

# Todos (inclui integration - precisa de Docker)
dotnet test src/EChamado/EChamado.sln
```

Scripts de teste de autenticacao na raiz do projeto: `test-openiddict-login.sh`, `test-openiddict-login.ps1`, `test-openiddict-login.py`.

---

## Documentacao

| Documento | Descricao |
|-----------|-----------|
| [docs/README.md](docs/README.md) | Ponto de entrada da documentacao |
| [docs/INDEX.md](docs/INDEX.md) | Indice navegacional |
| [docs/architecture/overview.md](docs/architecture/overview.md) | Arquitetura geral com diagramas Mermaid |
| [docs/architecture/class-diagram.md](docs/architecture/class-diagram.md) | Diagramas de classes |
| [docs/architecture/sequence-diagrams.md](docs/architecture/sequence-diagrams.md) | Fluxos de processos |
| [docs/architecture/use-cases.md](docs/architecture/use-cases.md) | Cenarios de negocio |
| [docs/onboarding/developer-onboarding.md](docs/onboarding/developer-onboarding.md) | Guia para novos desenvolvedores |
| [docs/features/implementation-process.md](docs/features/implementation-process.md) | Processo de implementacao |
| [docs/style-guide/csharp-style.md](docs/style-guide/csharp-style.md) | Padroes de codigo C# |
| [docs/AI-NATURAL-LANGUAGE-QUERY.md](docs/AI-NATURAL-LANGUAGE-QUERY.md) | Feature AI (natural language query) |
| [docs/AI-QUICKSTART.md](docs/AI-QUICKSTART.md) | Setup rapido da feature AI |

---

## Licenca

MIT. Veja [LICENSE](LICENSE).

---

## Autor

**Marcelo Azevedo** - [@mzet97](https://github.com/mzet97)
