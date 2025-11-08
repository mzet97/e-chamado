# EChamado - Sistema de Gerenciamento de Chamados

Sistema completo de gestão de tickets/chamados com autenticação SSO/OIDC, desenvolvido com .NET 9, Blazor WebAssembly e MudBlazor.

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4)](https://dotnet.microsoft.com/)
[![Blazor](https://img.shields.io/badge/Blazor-WebAssembly-512BD4)](https://blazor.net/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

---

## 🚀 Status do Projeto

**Versão Atual**: 0.8.0 (75-80% completo)
**Status**: Em desenvolvimento - FASES 1-3 concluídas

| Componente | Status | Progresso |
|------------|--------|-----------|
| Backend (CQRS + API) | ✅ Completo | 85% |
| Frontend (Blazor WASM) | ✅ Principal completo | 70% |
| SSO/OIDC | ✅ Completo | 100% |
| Admin Pages | ⚠️ Em desenvolvimento | 0% |
| Testes Automatizados | ❌ Não iniciado | 0% |
| CI/CD | ❌ Não iniciado | 0% |

---

## 📋 Funcionalidades Implementadas

### ✅ Autenticação & Autorização
- Login com credenciais
- SSO/OIDC com Authorization Code Flow + PKCE
- Refresh Token automático
- Roles (Admin, User, Support)
- Cookie seguro (SameSite=None)

### ✅ Gestão de Chamados
- Criar, editar, visualizar chamados
- Listagem com paginação server-side
- 7 filtros avançados (texto, status, departamento, tipo, período, vencidos)
- Atribuição de responsável
- Mudança de status
- Sistema de comentários (frontend pronto, backend em desenvolvimento)

### ✅ Dashboard
- Cards com estatísticas (Total, Meus Chamados, Atribuídos, Vencidos)
- Gráfico Donut (distribuição por status)
- Gráfico de Barras (chamados por departamento)
- Tabela de últimos 5 chamados
- Ações rápidas

### ✅ APIs REST
- 31 endpoints RESTful
- 6 Controllers (Orders, Categories, Departments, OrderTypes, StatusTypes, Auth)
- Paginação, filtros, busca
- Validação com FluentValidation
- Responses padronizadas

---

## 🏗️ Arquitetura

### Backend
- **Clean Architecture** (Domain, Application, Infrastructure, API)
- **CQRS** com MediatR
- **Domain Events**
- **Repository Pattern**
- **FluentValidation**
- **Entity Framework Core** (PostgreSQL)

### Frontend
- **Blazor WebAssembly**
- **MudBlazor** (Material Design)
- **HttpClient** com autenticação automática
- **In-memory caching** (LookupService)

### Infraestrutura
- **Docker Compose** (8 serviços)
- **PostgreSQL** (banco principal)
- **Redis** (cache distribuído)
- **RabbitMQ** (mensageria)
- **ELK Stack** (Elasticsearch, Logstash, Kibana)
- **Serilog** (logging estruturado)

---

## 🛠️ Tecnologias

| Categoria | Tecnologia |
|-----------|-----------|
| Backend | .NET 9, C# 13, ASP.NET Core |
| Frontend | Blazor WASM, MudBlazor 7.x |
| Autenticação | OpenIddict 6.1.1, ASP.NET Core Identity |
| Banco de Dados | PostgreSQL 15, Entity Framework Core 9 |
| Cache | Redis 7.x |
| Mensageria | RabbitMQ 3.x |
| Logging | Serilog, ELK Stack |
| Containerização | Docker, Docker Compose |
| Testes | xUnit, FluentAssertions, Moq, Testcontainers (planejado) |

---

## 📦 Estrutura do Projeto

```
e-chamado/
├── src/
│   └── EChamado/
│       ├── Server/
│       │   ├── EChamado.Server/              # API REST
│       │   ├── EChamado.Server.Application/  # CQRS (Commands, Queries, Handlers)
│       │   ├── EChamado.Server.Domain/       # Entidades, Eventos, Interfaces
│       │   └── EChamado.Server.Infrastructure/ # EF Core, Repositories, Configurações
│       ├── Client/
│       │   └── EChamado.Client/              # Blazor WebAssembly
│       │       ├── Pages/                    # Páginas Razor
│       │       ├── Services/                 # HTTP Services
│       │       ├── Models/                   # DTOs
│       │       └── Layout/                   # Layouts e componentes
│       └── Echamado.Auth/                    # Servidor de autenticação (Blazor Server)
├── tests/ (planejado)
│   ├── EChamado.Server.UnitTests/
│   └── EChamado.Server.IntegrationTests/
├── docs/
│   ├── PLANO-IMPLEMENTACAO.md                # FASES 1-3 (concluídas)
│   ├── PLANO-FASES-4-6.md                    # Plano detalhado das próximas fases
│   ├── ANALISE-COMPLETA.md                   # Análise técnica completa
│   ├── MATRIZ-FEATURES.md                    # Matriz comparativa de features
│   ├── PRÓXIMOS-PASSOS.md                    # Resumo executivo
│   └── SSO-SETUP.md                          # Guia de configuração SSO
├── docker-compose.yml
└── README.md
```

---

## 🚀 Como Executar

### Pré-requisitos
- .NET 9 SDK
- Docker & Docker Compose
- PostgreSQL (ou usar o container)

### 1. Clonar o repositório
```bash
git clone https://github.com/mzet97/e-chamado.git
cd e-chamado
```

### 2. Subir serviços de infraestrutura
```bash
docker-compose up -d postgres redis rabbitmq elasticsearch logstash kibana
```

### 3. Configurar banco de dados
```bash
cd src/EChamado/Server/EChamado.Server
dotnet ef database update
```

### 4. Executar aplicações

**Servidor de Autenticação (porta 5000):**
```bash
cd src/EChamado/Echamado.Auth
dotnet run
```

**API Server (porta 5001):**
```bash
cd src/EChamado/Server/EChamado.Server
dotnet run
```

**Cliente Blazor (porta 5002):**
```bash
cd src/EChamado/Client/EChamado.Client
dotnet run
```

### 5. Acessar aplicação
- **Cliente**: https://localhost:5002
- **Auth**: https://localhost:5000
- **API**: https://localhost:5001/swagger
- **Kibana**: http://localhost:5601

### Usuários padrão
```
Admin:
  Email: admin@echamado.com
  Senha: Admin@123

User:
  Email: user@echamado.com
  Senha: User@123
```

---

## 📚 Documentação

### Guias de Implementação
- **[PRÓXIMOS-PASSOS.md](PRÓXIMOS-PASSOS.md)** - Resumo executivo do que falta implementar
- **[PLANO-FASES-4-6.md](PLANO-FASES-4-6.md)** - Plano detalhado (1.088 linhas) com código de exemplo
- **[PLANO-IMPLEMENTACAO.md](PLANO-IMPLEMENTACAO.md)** - Histórico das FASES 1-3 concluídas
- **[SSO-SETUP.md](SSO-SETUP.md)** - Guia completo de configuração SSO/OIDC

### Análises Técnicas
- **[ANALISE-COMPLETA.md](ANALISE-COMPLETA.md)** - Análise detalhada de cada camada do sistema
- **[MATRIZ-FEATURES.md](MATRIZ-FEATURES.md)** - Matriz comparativa de features implementadas

---

## 🎯 Roadmap

### ✅ FASES 1-3 (Concluídas)
- [x] SSO/OIDC com Authorization Code + PKCE
- [x] Backend CQRS completo (6 controllers, 31 endpoints)
- [x] Frontend - Dashboard, Lista, Criar/Editar, Detalhes
- [x] Navegação com MudDrawer
- [x] 4 serviços HTTP autenticados

### 🔄 FASE 4: Interface Completa (5-6 dias)
- [ ] Comments API (Backend)
- [ ] Admin/Categories.razor
- [ ] Admin/Departments.razor
- [ ] Admin/OrderTypes.razor
- [ ] Admin/StatusTypes.razor

### 🔄 FASE 5: Monitoramento (1-2 dias)
- [ ] Health Checks (PostgreSQL, Redis, RabbitMQ)
- [ ] Endpoints /health, /ready, /live
- [ ] Docker health checks
- [ ] Request/Performance logging

### 🔄 FASE 6: Qualidade & CI/CD (6-8 dias)
- [ ] 20+ Unit Tests (Handlers)
- [ ] 10+ Unit Tests (Validators)
- [ ] 15+ Integration Tests (API)
- [ ] GitHub Actions CI/CD pipeline
- [ ] Code coverage > 70%

### 📋 FASE 7: Features Avançadas (opcional)
- [ ] Sistema de Anexos (file storage)
- [ ] Notificações por Email
- [ ] Relatórios PDF/Excel
- [ ] Sistema de Auditoria (LGPD)
- [ ] SLA Tracking
- [ ] 2FA (Two-Factor Authentication)

---

## 🧪 Testes

**Status**: Planejado para FASE 6

### Estrutura Planejada
```bash
tests/
├── EChamado.Server.UnitTests/
│   ├── Application/Commands/
│   ├── Application/Queries/
│   ├── Application/Validators/
│   └── Domain/Entities/
└── EChamado.Server.IntegrationTests/
    ├── Controllers/
    └── Infrastructure/Repositories/
```

### Tecnologias de Teste
- xUnit
- FluentAssertions
- Moq
- AutoFixture
- Testcontainers (PostgreSQL)
- WebApplicationFactory

---

## 📊 Métricas

| Métrica | Valor |
|---------|-------|
| Arquivos C# | 242 |
| Páginas Blazor | 29 |
| Controllers | 6 |
| Endpoints REST | 31 |
| Linhas de Código | ~15.000 |
| Commits | 10+ |
| Documentação | 4.000+ linhas |

---

## 🤝 Contribuindo

1. Fork o projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

---

## 📝 Licença

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.

---

## 👨‍💻 Autor

**Marcelo Azevedo**
- GitHub: [@mzet97](https://github.com/mzet97)

---

## 📞 Suporte

Para reportar bugs ou solicitar features, abra uma [issue](https://github.com/mzet97/e-chamado/issues).

---

**Desenvolvido com ❤️ usando .NET 9 e Blazor WebAssembly**
