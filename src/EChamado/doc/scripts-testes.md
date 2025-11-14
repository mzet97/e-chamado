# Scripts para Execução de Testes e Cobertura

## Primeiro Setup (Execute apenas uma vez)
```powershell
# 1. Compilar todos os projetos
dotnet build

# 2. Instalar navegadores do Playwright
.\install-playwright.ps1
```

## Execução de Testes Unitários com Cobertura
```powershell
# Executar apenas testes unitários do servidor
dotnet test Tests\EChamado.Server.UnitTests\ --collect:"XPlat Code Coverage" --results-directory ./TestResults/UnitTests

# Executar testes unitários de todos os projetos
dotnet test Tests\EChamado.Server.UnitTests\ Tests\EChamado.Client.UnitTests\ Tests\EChamado.Shared.UnitTests\ Tests\Echamado.Auth.UnitTests\ --collect:"XPlat Code Coverage" --results-directory ./TestResults/UnitTests

# Executar com configuração de cobertura customizada
dotnet test Tests\EChamado.Server.UnitTests\ --collect:"XPlat Code Coverage" --settings coverlet.runsettings --results-directory ./TestResults/UnitTests
```

## Execução de Testes de Integração com Cobertura
```powershell
# Executar testes de integração (requer Docker rodando)
dotnet test Tests\EChamado.Server.IntegrationTests\ --collect:"XPlat Code Coverage" --results-directory ./TestResults/IntegrationTests

# Executar com variáveis de ambiente específicas
$env:ConnectionStrings__DefaultConnection="Host=localhost;Port=5432;Database=echamado_test;Username=postgres;Password=postgres"
$env:ConnectionStrings__Redis="localhost:6379"
dotnet test Tests\EChamado.Server.IntegrationTests\ --collect:"XPlat Code Coverage" --results-directory ./TestResults/IntegrationTests
```

## Execução de Testes E2E com Playwright
```powershell
# Executar testes E2E (requer aplicação rodando)
# Primeiro, iniciar a aplicação em um terminal separado:
# dotnet run --project Server\EChamado.Server\

# Em outro terminal, executar os testes E2E:
dotnet test Tests\EChamado.E2E.Tests\ --collect:"XPlat Code Coverage" --results-directory ./TestResults/E2E

# Executar com configuração específica
dotnet test Tests\EChamado.E2E.Tests\ --logger "console;verbosity=detailed" --results-directory ./TestResults/E2E
```

## Execução de Todos os Testes
```powershell
# Executar todos os testes (exceto E2E que requer aplicação rodando)
dotnet test --collect:"XPlat Code Coverage" --results-directory ./TestResults/All --filter "FullyQualifiedName!~E2E"

# Executar todos incluindo E2E (aplicação deve estar rodando)
dotnet test --collect:"XPlat Code Coverage" --results-directory ./TestResults/All
```

## Geração de Relatórios de Cobertura
```powershell
# Gerar relatório HTML completo
reportgenerator -reports:"TestResults\**\coverage.cobertura.xml" -targetdir:"TestResults\CoverageReport" -reporttypes:Html

# Gerar relatório de texto para análise rápida
reportgenerator -reports:"TestResults\**\coverage.cobertura.xml" -targetdir:"TestResults\CoverageReport" -reporttypes:TextSummary

# Gerar múltiplos formatos
reportgenerator -reports:"TestResults\**\coverage.cobertura.xml" -targetdir:"TestResults\CoverageReport" -reporttypes:"Html;JsonSummary;Cobertura;TextSummary"

# Visualizar relatório HTML
start TestResults\CoverageReport\index.html
```

## Configuração de Cobertura Específica
```powershell
# Executar com threshold de cobertura mínima (falha se não atingir 85%)
dotnet test Tests\EChamado.Server.UnitTests\ --collect:"XPlat Code Coverage" --settings coverlet.runsettings --results-directory ./TestResults

# Executar com exclusões específicas usando arquivo de configuração
dotnet test --collect:"XPlat Code Coverage" --settings coverlet.runsettings --results-directory ./TestResults

# Gerar relatório com threshold de falha
reportgenerator -reports:"TestResults\**\coverage.cobertura.xml" -targetdir:"TestResults\CoverageReport" -reporttypes:Html -assemblyfilters:"+EChamado.Server.Domain;+EChamado.Server.Application" -classfilters:"-*.Migrations.*"
```

## Scripts Avançados

### Análise de Cobertura por Projeto
```powershell
# Executar testes e analisar cobertura por projeto específico
function Test-ProjectCoverage {
    param($ProjectPath, $MinCoverage = 85)
    
    Write-Host "Testando cobertura para: $ProjectPath" -ForegroundColor Yellow
    
    dotnet test $ProjectPath --collect:"XPlat Code Coverage" --results-directory "./TestResults/Project"
    
    $reportPath = "./TestResults/ProjectReport"
    reportgenerator -reports:"TestResults\Project\**\coverage.cobertura.xml" -targetdir:$reportPath -reporttypes:JsonSummary
    
    $coverage = (Get-Content "$reportPath\Summary.json" | ConvertFrom-Json).summary.linecoverage
    
    if ($coverage -lt $MinCoverage) {
        Write-Host "FALHA: Cobertura $coverage% abaixo do mínimo $MinCoverage%" -ForegroundColor Red
        return $false
    } else {
        Write-Host "SUCESSO: Cobertura $coverage% atingiu o mínimo $MinCoverage%" -ForegroundColor Green
        return $true
    }
}

# Usar a função
Test-ProjectCoverage "Tests\EChamado.Server.UnitTests\" 85
```

### Execução Completa com Validação
```powershell
# Script completo de validação de cobertura
function Run-FullTestSuite {
    Write-Host "=== INICIANDO SUITE COMPLETA DE TESTES ===" -ForegroundColor Cyan
    
    # Limpar resultados anteriores
    if (Test-Path "TestResults") {
        Remove-Item "TestResults" -Recurse -Force
    }
    
    # 1. Testes Unitários
    Write-Host "1. Executando Testes Unitários..." -ForegroundColor Yellow
    dotnet test Tests\EChamado.Server.UnitTests\ Tests\EChamado.Client.UnitTests\ Tests\EChamado.Shared.UnitTests\ Tests\Echamado.Auth.UnitTests\ --collect:"XPlat Code Coverage" --results-directory ./TestResults/Unit --no-build
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "FALHA nos testes unitários!" -ForegroundColor Red
        return
    }
    
    # 2. Testes de Integração (apenas se Docker estiver disponível)
    Write-Host "2. Verificando Docker e executando Testes de Integração..." -ForegroundColor Yellow
    try {
        docker ps | Out-Null
        dotnet test Tests\EChamado.Server.IntegrationTests\ --collect:"XPlat Code Coverage" --results-directory ./TestResults/Integration --no-build
        
        if ($LASTEXITCODE -ne 0) {
            Write-Host "FALHA nos testes de integração!" -ForegroundColor Red
            return
        }
    } catch {
        Write-Host "Docker não disponível, pulando testes de integração" -ForegroundColor Yellow
    }
    
    # 3. Gerar Relatório de Cobertura
    Write-Host "3. Gerando Relatório de Cobertura..." -ForegroundColor Yellow
    reportgenerator -reports:"TestResults\**\coverage.cobertura.xml" -targetdir:"TestResults\CoverageReport" -reporttypes:"Html;JsonSummary;TextSummary"
    
    # 4. Analisar Resultados
    Write-Host "4. Analisando Resultados..." -ForegroundColor Yellow
    if (Test-Path "TestResults\CoverageReport\Summary.json") {
        $summary = Get-Content "TestResults\CoverageReport\Summary.json" | ConvertFrom-Json
        $lineCoverage = $summary.summary.linecoverage
        $branchCoverage = $summary.summary.branchcoverage
        
        Write-Host "=== RESULTADOS FINAIS ===" -ForegroundColor Cyan
        Write-Host "Cobertura de Linha: $lineCoverage%" -ForegroundColor $(if($lineCoverage -ge 85) {"Green"} else {"Red"})
        Write-Host "Cobertura de Branch: $branchCoverage%" -ForegroundColor $(if($branchCoverage -ge 85) {"Green"} else {"Red"})
        
        if ($lineCoverage -ge 85 -and $branchCoverage -ge 85) {
            Write-Host "? META DE COBERTURA ATINGIDA!" -ForegroundColor Green
        } else {
            Write-Host "? Meta de cobertura não atingida (85%)" -ForegroundColor Red
        }
    }
    
    # 5. Abrir Relatório
    Write-Host "5. Abrindo Relatório de Cobertura..." -ForegroundColor Yellow
    start TestResults\CoverageReport\index.html
    
    Write-Host "=== SUITE DE TESTES CONCLUÍDA ===" -ForegroundColor Cyan
}

# Executar suite completa
Run-FullTestSuite
```

## Scripts de CI/CD

### GitHub Actions Workflow
```yaml
name: Tests and Coverage

on: 
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main ]

jobs:
  test:
    runs-on: ubuntu-latest
    
    services:
      postgres:
        image: postgres:15
        env:
          POSTGRES_PASSWORD: postgres
          POSTGRES_DB: echamado_test
        options: >-
          --health-cmd pg_isready
          --health-interval 10s
          --health-timeout 5s
          --health-retries 5
        ports:
          - 5432:5432
      
      redis:
        image: redis:7
        options: >-
          --health-cmd "redis-cli ping"
          --health-interval 10s
          --health-timeout 5s
          --health-retries 5
        ports:
          - 6379:6379
    
    steps:
    - name: Checkout
      uses: actions/checkout@v4
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: '9.0.x'
    
    - name: Restore dependencies
      run: dotnet restore
    
    - name: Build
      run: dotnet build --no-restore
    
    - name: Run Unit Tests
      run: |
        dotnet test Tests/EChamado.Server.UnitTests/ \
                   Tests/EChamado.Client.UnitTests/ \
                   Tests/EChamado.Shared.UnitTests/ \
                   Tests/Echamado.Auth.UnitTests/ \
          --no-build \
          --collect:"XPlat Code Coverage" \
          --results-directory ./TestResults/UnitTests \
          --logger trx
    
    - name: Run Integration Tests  
      run: |
        dotnet test Tests/EChamado.Server.IntegrationTests/ \
          --no-build \
          --collect:"XPlat Code Coverage" \
          --results-directory ./TestResults/IntegrationTests \
          --logger trx
      env:
        ConnectionStrings__DefaultConnection: "Host=localhost;Port=5432;Database=echamado_test;Username=postgres;Password=postgres"
        ConnectionStrings__Redis: "localhost:6379"
    
    - name: Install Playwright Browsers
      run: |
        dotnet build Tests/EChamado.E2E.Tests/
        pwsh Tests/EChamado.E2E.Tests/bin/Debug/net9.0/playwright.ps1 install --with-deps
    
    - name: Start Application for E2E Tests
      run: |
        dotnet run --project Server/EChamado.Server/ &
        sleep 30  # Wait for application to start
    
    - name: Run E2E Tests
      run: |
        dotnet test Tests/EChamado.E2E.Tests/ \
          --no-build \
          --collect:"XPlat Code Coverage" \
          --results-directory ./TestResults/E2E \
          --logger trx
    
    - name: Install ReportGenerator
      run: dotnet tool install -g dotnet-reportgenerator-globaltool
    
    - name: Generate Coverage Report
      run: |
        reportgenerator \
          -reports:"TestResults/**/coverage.cobertura.xml" \
          -targetdir:"TestResults/CoverageReport" \
          -reporttypes:"Html;JsonSummary;Cobertura"
    
    - name: Upload Coverage Reports
      uses: actions/upload-artifact@v4
      with:
        name: coverage-reports
        path: TestResults/CoverageReport/
    
    - name: Upload Test Results
      uses: actions/upload-artifact@v4
      if: always()
      with:
        name: test-results
        path: TestResults/**/*.trx
    
    - name: Coverage Comment
      if: github.event_name == 'pull_request'
      uses: marocchino/sticky-pull-request-comment@v2
      with:
        recreate: true
        path: TestResults/CoverageReport/Summary.txt
```

## Configuração de Coverage Settings

### coverlet.runsettings (já criado)
O arquivo `coverlet.runsettings` na raiz do projeto contém as configurações de cobertura.

## Métricas de Cobertura por Fase

### Fase 1 (Atual - Infraestrutura)
- **Meta**: Configurar todos os projetos de teste ?
- **Status**: Completo

### Fase 2 (Testes Unitários)
- **Meta**: 85% line coverage, 85% branch coverage
- **Projetos**: Domain, Application, Shared, Auth, Client

### Fase 3 (Testes Integração)
- **Meta**: 85% coverage dos endpoints e infrastructure
- **Projetos**: Server, Infrastructure

### Fase 4 (Testes E2E)
- **Meta**: 85% coverage dos fluxos principais
- **Escopo**: Blazor WebAssembly flows

### Comandos Úteis para Desenvolvimento
```powershell
# Verificar cobertura atual rapidamente
dotnet test Tests\EChamado.Server.UnitTests\ --collect:"XPlat Code Coverage" && reportgenerator -reports:"TestResults\**\coverage.cobertura.xml" -targetdir:"TestResults\Quick" -reporttypes:TextSummary && Get-Content TestResults\Quick\Summary.txt | Select-Object -First 20

# Executar testes em watch mode durante desenvolvimento
dotnet watch test Tests\EChamado.Server.UnitTests\