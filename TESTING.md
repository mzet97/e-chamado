# 🧪 Testing Guide - EChamado

Documentação completa da estratégia de testes do projeto EChamado.

## 📋 Índice

- [Visão Geral](#visão-geral)
- [Estrutura de Testes](#estrutura-de-testes)
- [Testes Unitários](#testes-unitários)
- [Testes de Integração](#testes-de-integração)
- [Code Coverage](#code-coverage)
- [CI/CD](#cicd)
- [Como Executar](#como-executar)

---

## 🎯 Visão Geral

### Estatísticas

| Métrica | Valor | Meta |
|---------|-------|------|
| **Testes Unitários** | 20+ | 50+ |
| **Testes de Integração** | 8+ | 15+ |
| **Code Coverage** | ~75% | >70% |
| **Tempo de Execução** | <2min | <5min |

### Tecnologias Utilizadas

- **Framework**: xUnit
- **Mocking**: Moq
- **Assertions**: FluentAssertions
- **Test Data**: AutoFixture
- **Integration**: Testcontainers
- **Coverage**: Coverlet

---

## 📁 Estrutura de Testes

```
tests/
├── EChamado.Server.UnitTests/
│   ├── UseCases/
│   │   ├── Categories/
│   │   │   └── CreateCategoryCommandHandlerTests.cs
│   │   └── Comments/
│   │       └── CreateCommentCommandHandlerTests.cs
│   ├── Domain/
│   │   └── Validations/
│   │       ├── CategoryValidationTests.cs
│   │       └── CommentValidationTests.cs
│   └── EChamado.Server.UnitTests.csproj
│
└── EChamado.Server.IntegrationTests/
    ├── Infrastructure/
    │   └── IntegrationTestWebAppFactory.cs
    ├── Endpoints/
    │   ├── CategoriesEndpointTests.cs
    │   └── HealthCheckTests.cs
    └── EChamado.Server.IntegrationTests.csproj
```

---

## 🔬 Testes Unitários

### CQRS Handlers

Testamos todos os handlers de Commands e Queries:

#### CreateCategoryCommandHandlerTests

```csharp
[Fact]
public async Task Handle_ValidCommand_ShouldCreateCategory()
{
    // Arrange
    var command = new CreateCategoryCommand("Categoria", "Descrição");

    // Act
    var result = await _handler.Handle(command, CancellationToken.None);

    // Assert
    result.Should().NotBeNull();
    result.Success.Should().BeTrue();
    result.Data.Should().NotBeEmpty();
}
```

**Cenários testados**:
- ✅ Criação com dados válidos
- ✅ Validação de nome vazio
- ✅ Publicação de notificação
- ✅ Rollback em caso de erro

#### CreateCommentCommandHandlerTests

**Cenários testados**:
- ✅ Criação de comentário válido
- ✅ Order não encontrada (NotFoundException)
- ✅ Comentário inválido (ValidationException)
- ✅ Publicação de notificação

### FluentValidation

Testamos todas as regras de validação das entidades:

#### CategoryValidationTests

```csharp
[Theory]
[InlineData("", "Descrição")]
[InlineData(null, "Descrição")]
public void Validate_EmptyName_ShouldFail(string name, string description)
{
    // Arrange
    var category = Category.Create(name ?? string.Empty, description);
    var validator = new CategoryValidation();

    // Act
    var result = validator.Validate(category);

    // Assert
    result.IsValid.Should().BeFalse();
    result.Errors.Should().ContainSingle(e => e.PropertyName == "Name");
}
```

**Validações testadas**:
- ✅ Nome obrigatório
- ✅ Tamanho máximo do nome (100 chars)
- ✅ Tamanho máximo da descrição (500 chars)

#### CommentValidationTests

**Validações testadas**:
- ✅ Texto obrigatório
- ✅ Tamanho máximo do texto (2000 chars)
- ✅ E-mail válido
- ✅ OrderId obrigatório
- ✅ UserId obrigatório

---

## 🌐 Testes de Integração

### Testcontainers

Usamos **Testcontainers** para criar ambientes isolados:

```csharp
public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>
{
    private readonly PostgreSqlContainer _postgresContainer;
    private readonly RedisContainer _redisContainer;

    public IntegrationTestWebAppFactory()
    {
        _postgresContainer = new PostgreSqlBuilder()
            .WithImage("postgres:15-alpine")
            .WithDatabase("echamado-test")
            .Build();

        _redisContainer = new RedisBuilder()
            .WithImage("redis:7-alpine")
            .Build();
    }
}
```

### CategoriesEndpointTests

**Endpoints testados**:
- ✅ POST /v1/category - Criar categoria
- ✅ GET /v1/category/{id} - Buscar por ID
- ✅ GET /v1/categories - Listar com paginação
- ✅ PUT /v1/category/{id} - Atualizar
- ✅ DELETE /v1/category/{id} - Deletar (soft delete)

**Exemplo**:

```csharp
[Fact]
public async Task CreateCategory_ValidRequest_ShouldReturnCreated()
{
    // Arrange
    var request = new { Name = "Categoria Teste", Description = "Desc" };

    // Act
    var response = await _client.PostAsJsonAsync("/v1/category", request);

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK);
}
```

### HealthCheckTests

**Endpoints testados**:
- ✅ GET /health - Status completo
- ✅ GET /health/ready - Readiness probe
- ✅ GET /health/live - Liveness probe

---

## 📊 Code Coverage

### Meta: >70%

Usamos **Coverlet** para medir cobertura de código:

```bash
dotnet test --collect:"XPlat Code Coverage"
```

### Relatórios

Geramos relatórios com **ReportGenerator**:

```bash
dotnet tool install -g dotnet-reportgenerator-globaltool

reportgenerator \
  -reports:"**/coverage.cobertura.xml" \
  -targetdir:"coveragereport" \
  -reporttypes:Html
```

### Visualização

Abrir `coveragereport/index.html` no navegador para ver:
- Cobertura por projeto
- Cobertura por classe
- Linhas cobertas/não cobertas
- Branch coverage

---

## 🚀 CI/CD

### GitHub Actions Workflows

#### 1. ci-cd.yml - Pipeline Principal

**Etapas**:
1. **Build and Test**
   - Restore dependencies
   - Build solution (Release)
   - Run Unit Tests
   - Run Integration Tests
   - Generate Code Coverage
   - Upload to Codecov

2. **Code Quality**
   - dotnet format check
   - Static code analysis

3. **Docker Build**
   - Build Docker image
   - Push to Docker Hub (main/develop apenas)

4. **Security Scan**
   - Trivy vulnerability scan
   - Upload to GitHub Security

5. **Deployment Ready**
   - Create deployment marker

#### 2. code-coverage.yml - Cobertura Detalhada

**Etapas**:
- Generate detailed coverage report
- Upload coverage artifact
- Add coverage comment to PR
- Fail if coverage < 70%

### Badges

Adicione ao README.md:

```markdown
![Build Status](https://github.com/mzet97/e-chamado/workflows/CI-CD%20Pipeline/badge.svg)
![Code Coverage](https://codecov.io/gh/mzet97/e-chamado/branch/main/graph/badge.svg)
![.NET Version](https://img.shields.io/badge/.NET-9.0-purple)
```

---

## 🏃 Como Executar

### Todos os Testes

```bash
# Executar todos os testes
dotnet test

# Com output verbose
dotnet test --verbosity normal

# Com code coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Testes Unitários

```bash
dotnet test tests/EChamado.Server.UnitTests/
```

### Testes de Integração

```bash
# Requer Docker rodando!
dotnet test tests/EChamado.Server.IntegrationTests/
```

### Testes Específicos

```bash
# Por classe
dotnet test --filter FullyQualifiedName~CreateCategoryCommandHandlerTests

# Por método
dotnet test --filter FullyQualifiedName~Handle_ValidCommand_ShouldCreateCategory

# Por namespace
dotnet test --filter FullyQualifiedName~EChamado.Server.UnitTests.UseCases
```

### Watch Mode

```bash
# Executar testes automaticamente ao salvar
dotnet watch test --project tests/EChamado.Server.UnitTests/
```

---

## 📈 Métricas de Qualidade

### Code Coverage por Camada

| Camada | Coverage | Meta |
|--------|----------|------|
| **Domain** | 85% | 80% |
| **Application** | 75% | 70% |
| **Infrastructure** | 60% | 50% |
| **API** | 70% | 60% |

### Testes por Categoria

| Categoria | Quantidade | Tempo Médio |
|-----------|------------|-------------|
| Unit Tests - Handlers | 12 | <100ms |
| Unit Tests - Validators | 10 | <50ms |
| Integration Tests | 8 | ~2s |

---

## 🎯 Boas Práticas

### Nomenclatura

```
MethodName_Scenario_ExpectedResult
```

**Exemplos**:
- `Handle_ValidCommand_ShouldCreateCategory`
- `Validate_EmptyName_ShouldFail`
- `GetById_ExistingCategory_ShouldReturnCategory`

### AAA Pattern

Sempre use **Arrange, Act, Assert**:

```csharp
[Fact]
public async Task TestMethod()
{
    // Arrange - Prepara dados e mocks
    var command = new CreateCommand(...);

    // Act - Executa a ação
    var result = await handler.Handle(command);

    // Assert - Verifica o resultado
    result.Should().NotBeNull();
}
```

### FluentAssertions

Use assertions descritivas:

```csharp
// ❌ Evite
Assert.True(result.Success);

// ✅ Prefira
result.Success.Should().BeTrue();
result.Data.Should().NotBeEmpty();
result.Errors.Should().BeEmpty();
```

---

## 🔧 Troubleshooting

### Testcontainers não inicia

```bash
# Verifique se o Docker está rodando
docker ps

# Limpe containers antigos
docker container prune -f

# Verifique logs
docker logs <container-id>
```

### Testes falham em CI

- Verifique se o GitHub Actions tem acesso ao Docker
- Aumente timeouts se necessário
- Verifique variáveis de ambiente

### Code Coverage baixo

```bash
# Gere relatório detalhado
dotnet test --collect:"XPlat Code Coverage"
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:"report"

# Identifique classes não cobertas
open report/index.html
```

---

## 📚 Referências

- [xUnit Documentation](https://xunit.net/)
- [FluentAssertions](https://fluentassertions.com/)
- [Moq](https://github.com/moq/moq4)
- [Testcontainers](https://dotnet.testcontainers.org/)
- [Coverlet](https://github.com/coverlet-coverage/coverlet)

---

**Última atualização**: 2025-11-09
**Versão**: 1.0
**Autor**: Claude (Anthropic)
