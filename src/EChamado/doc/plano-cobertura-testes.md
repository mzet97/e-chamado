# Plano de Execução para Alcançar 85% de Cobertura de Testes

## ?? Análise Atual da Cobertura

### Estado Atual dos Testes
- **Cobertura de Linha**: 4.4% (393 de 8.864 linhas cobertas)
- **Cobertura de Branch**: 2.1% (21 de 958 branches cobertos)
- **Cobertura de Método**: 7.6% (91 de 1.193 métodos cobertos)

### Projetos Existentes com Testes
- ? `EChamado.Server.UnitTests` - 22 testes passando
- ? `EChamado.Server.IntegrationTests` - Configurado com TestContainers

### Projetos sem Cobertura (0%)
- ?? `EChamado.Server` (API/Controllers/Endpoints)
- ?? `EChamado.Server.Infrastructure` (Repositories/Data Access)
- ?? `EChamado.Client` (Blazor WebAssembly)
- ?? `EChamado.Shared` (DTOs/Models compartilhados)
- ?? `Echamado.Auth` (Autenticação e autorização)

## ?? Meta de Cobertura

**Objetivos por tipo de teste:**
- **Testes Unitários**: 85% de cobertura
- **Testes de Integração**: 85% de cobertura  
- **Testes E2E (Playwright)**: 85% de cobertura dos fluxos principais

## ?? Fases de Execução

### **Fase 1: Infraestrutura de Testes e Configuração** ??

#### 1.1 Configuração de Ferramentas
- [ ] Criar projeto `EChamado.Client.UnitTests`
- [ ] Criar projeto `EChamado.Shared.UnitTests`
- [ ] Criar projeto `Echamado.Auth.UnitTests`
- [ ] Criar projeto `EChamado.E2E.Tests` com Playwright
- [ ] Configurar ReportGenerator para análise de cobertura
- [ ] Configurar GitHub Actions para CI/CD com cobertura

#### 1.2 Utilitários de Teste
- [ ] Criar builders para entidades de domínio
- [ ] Configurar AutoFixture para geração de dados
- [ ] Criar mocks base para serviços externos
- [ ] Configurar TestContainers para banco e Redis

#### 1.3 Configuração de Coverage
```xml
<!-- Adicionar em todos os projetos de teste -->
<PackageReference Include="coverlet.collector" Version="6.0.2" />
<PackageReference Include="coverlet.msbuild" Version="6.0.2" />
```

---

### **Fase 2: Testes Unitários (85% de cobertura)** ??

#### 2.1 Domain Layer (`EChamado.Server.Domain`)
**Entidades e Value Objects**
- [ ] `Order` - Ciclo de vida completo, validações
- [ ] `Category` - CRUD e hierarquia
- [ ] `Department` - Estados e transições
- [ ] `Comment` - Associações e validações
- [ ] `StatusType` - Enums e comportamentos
- [ ] `OrderType` - Classificações

**Validações de Domínio**
- [ ] `OrderValidation` - Regras de negócio complexas
- [ ] `CategoryValidation` - Hierarquia e dependências
- [ ] `CommentValidation` - Conteúdo e associações
- [ ] `DepartmentValidation` - Estados válidos

**Domain Events**
- [ ] `OrderCreated`, `OrderUpdated`, `OrderClosed`
- [ ] `CategoryCreated`, `CategoryUpdated`
- [ ] `CommentCreated`, `CommentDeleted`
- [ ] Handlers de eventos de domínio

**Domain Services**
- [ ] `IUserClaimService` - Gerenciamento de claims
- [ ] `IRedisService` - Cache distribuído
- [ ] `IUserTokenService` - Tokens JWT

#### 2.2 Application Layer (`EChamado.Server.Application`)
**Command Handlers (CQRS)**
- [ ] Orders: `CreateOrderCommandHandler`, `UpdateOrderCommandHandler`, `CloseOrderCommandHandler`
- [ ] Categories: `CreateCategoryCommandHandler`, `UpdateCategoryCommandHandler`, `DeleteCategoryCommandHandler`
- [ ] Comments: `CreateCommentCommandHandler`, `DeleteCommentCommandHandler`
- [ ] Departments: `CreateDepartmentCommandHandler`, `UpdateDepartmentCommandHandler`
- [ ] Auth: `LoginUserCommandHandler`, `RegisterUserCommandHandler`

**Query Handlers**
- [ ] `SearchOrdersQueryHandler` - Paginação e filtros
- [ ] `GetOrderByIdQueryHandler` - Busca específica
- [ ] `SearchCategoriesQueryHandler` - Hierarquia
- [ ] `GetCategoryByIdQueryHandler` - Detalhes

**Application Services**
- [ ] `ApplicationUserService` - Gerenciamento de usuários
- [ ] `OpenIddictService` - OAuth/OpenID Connect
- [ ] `UserRoleService` - Permissões e papéis

**Behaviors (Pipeline)**
- [ ] `ValidationBehaviour` - Validação automática
- [ ] `UnhandledExceptionBehaviour` - Tratamento de erros

#### 2.3 Shared & Auth Projects
**EChamado.Shared**
- [ ] DTOs e Models
- [ ] Extensões e utilitários
- [ ] Constantes e enums

**Echamado.Auth**
- [ ] Configurações de autenticação
- [ ] Políticas de autorização
- [ ] Middleware de segurança

**EChamado.Client.Application**
- [ ] Services do cliente Blazor
- [ ] Handlers de autenticação
- [ ] Models do cliente

---

### **Fase 3: Testes de Integração (85% de cobertura)** ??

#### 3.1 API Endpoints (`EChamado.Server`)
**Controllers e Minimal APIs**
- [ ] `AuthorizationController` - OAuth flows
- [ ] Endpoints de Orders - CRUD completo
- [ ] Endpoints de Categories - Hierarquia
- [ ] Endpoints de Comments - Associações
- [ ] Endpoints de Departments - Estados

**Middleware**
- [ ] Authentication middleware
- [ ] Authorization middleware  
- [ ] Exception handling middleware
- [ ] Request/Response logging

#### 3.2 Infrastructure Layer (`EChamado.Server.Infrastructure`)
**Repositories**
- [ ] `OrderRepository` - Queries complexas, joins
- [ ] `CategoryRepository` - Hierarquia e árvore
- [ ] `CommentRepository` - Associações
- [ ] `DepartmentRepository` - Estados
- [ ] `IUnitOfWork` - Transações

**Data Access**
- [ ] Entity Framework contexts
- [ ] Configurações de entidades
- [ ] Migrations e seed data
- [ ] Connection strings e configurações

**External Services**
- [ ] Redis cache implementation
- [ ] Email services
- [ ] File storage services

#### 3.3 Database Integration
- [ ] PostgreSQL com TestContainers
- [ ] Redis com TestContainers
- [ ] Testes de migração
- [ ] Testes de performance de queries

---

### **Fase 4: Testes E2E com Playwright (85% de cobertura)** ??

#### 4.1 Setup do Playwright
```csharp
// EChamado.E2E.Tests
- Configuração do Playwright
- Page Object Models
- Fixtures de teste
- Screenshots e vídeos de falhas
```

#### 4.2 Authentication Flow
- [ ] **Login** - Usuário válido/inválido, remember me
- [ ] **Logout** - Limpeza de sessão
- [ ] **Registration** - Validações de formulário
- [ ] **Token Refresh** - Renovação automática
- [ ] **Forgotten Password** - Reset de senha

#### 4.3 Core Functionality (Blazor WebAssembly)
**Orders Management**
- [ ] Criar novo chamado - Formulário completo
- [ ] Listar chamados - Paginação e filtros
- [ ] Visualizar detalhes - Comentários e histórico
- [ ] Editar chamado - Validações e estados
- [ ] Fechar chamado - Workflow completo

**Categories Management**
- [ ] CRUD de categorias - Hierarquia
- [ ] Subcategorias - Dependências
- [ ] Validações de formulário

**Comments System**
- [ ] Adicionar comentário - Real-time updates
- [ ] Excluir comentário - Confirmações
- [ ] Anexos de arquivos

#### 4.4 Admin Features
**User Management**
- [ ] Listar usuários - Paginação
- [ ] Criar usuário - Validações
- [ ] Editar permissões - Roles e claims
- [ ] Desativar usuário

**Configuration**
- [ ] Departments - CRUD completo
- [ ] Order Types - Configurações
- [ ] Status Types - Workflow

**Reports and Analytics**
- [ ] Dashboard - Métricas principais
- [ ] Relatórios - Filtros e exportação
- [ ] Gráficos - Interatividade

#### 4.5 Cross-Browser Testing
- [ ] Chrome/Chromium
- [ ] Firefox
- [ ] Safari (se disponível)
- [ ] Edge

---

### **Fase 5: Otimização e Validação** ??

#### 5.1 Análise de Cobertura
- [ ] Executar análise completa de cobertura
- [ ] Identificar gaps restantes
- [ ] Priorizar testes para áreas críticas
- [ ] Atingir meta de 85% em cada categoria

#### 5.2 Performance e Qualidade
- [ ] Otimizar testes lentos (>1s)
- [ ] Paralelizar execução de testes
- [ ] Configurar retry policies
- [ ] Implementar test data builders

#### 5.3 CI/CD Integration
```yaml
# GitHub Actions pipeline
- Build & Test
- Code Coverage Report
- Quality Gates (85% coverage)
- Deployment gates
```

#### 5.4 Documentação
- [ ] Guias de contribuição para testes
- [ ] Padrões e convenções
- [ ] Troubleshooting comum
- [ ] Métricas e KPIs

---

## ??? Ferramentas e Tecnologias

### Testing Frameworks
- **xUnit** - Framework principal
- **FluentAssertions** - Assertions mais legíveis
- **AutoFixture** - Geração de dados de teste
- **Moq** - Mocking framework

### Integration Testing
- **Microsoft.AspNetCore.Mvc.Testing** - TestServer
- **TestContainers** - PostgreSQL e Redis
- **WebApplicationFactory** - Factory personalizada

### E2E Testing
- **Microsoft.Playwright** - Automação web
- **Playwright Test** - Test runner
- **Screenshots/Videos** - Evidências de falhas

### Coverage Tools
- **Coverlet** - Code coverage collector
- **ReportGenerator** - Relatórios HTML
- **SonarQube** - Análise de qualidade (opcional)

---

## ?? Métricas de Sucesso

### Objetivos Quantitativos
- ? **85%** Line Coverage
- ? **85%** Branch Coverage  
- ? **85%** Method Coverage
- ? **<1s** Tempo médio por teste unitário
- ? **<30s** Tempo médio por teste de integração
- ? **<2min** Tempo médio por teste E2E

### Objetivos Qualitativos
- ? Testes determinísticos (não flaky)
- ? Testes independentes (sem ordem)
- ? Feedback rápido para desenvolvedores
- ? Documentação viva através dos testes

---

## ?? Cronograma Estimado

| Fase | Duração | Esforço |
|------|---------|---------|
| **Fase 1** - Infraestrutura | 1-2 semanas | 40-60h |
| **Fase 2** - Testes Unitários | 3-4 semanas | 120-160h |
| **Fase 3** - Testes Integração | 2-3 semanas | 80-120h |
| **Fase 4** - Testes E2E | 2-3 semanas | 80-120h |
| **Fase 5** - Otimização | 1 semana | 20-40h |
| **Total** | **9-13 semanas** | **340-500h** |

---

## ?? Próximos Passos

1. **Aprovação do plano** - Review e ajustes necessários
2. **Setup inicial** - Configurar ferramentas e ambiente
3. **Execução incremental** - Implementar fase por fase
4. **Monitoramento contínuo** - Acompanhar métricas de cobertura
5. **Refinamento** - Ajustar com base nos resultados

---

*Documento criado em: {{date}}*  
*Última atualização: {{date}}*  
*Responsável: Equipe de Desenvolvimento*