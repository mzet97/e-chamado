# Script para instalar navegadores do Playwright
# Execute este script após compilar o projeto EChamado.E2E.Tests

Write-Host "Instalando navegadores do Playwright..." -ForegroundColor Green

# Navegar para o diretório do projeto E2E
$projectDir = "Tests\EChamado.E2E.Tests"
$binDir = "$projectDir\bin\Debug\net9.0"

if (Test-Path $binDir) {
    Set-Location $binDir
    
    # Executar instalação dos navegadores
    & ".\playwright.ps1" install
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "Navegadores do Playwright instalados com sucesso!" -ForegroundColor Green
    } else {
        Write-Host "Erro ao instalar navegadores do Playwright" -ForegroundColor Red
        exit 1
    }
} else {
    Write-Host "Projeto EChamado.E2E.Tests não foi compilado ainda." -ForegroundColor Yellow
    Write-Host "Execute 'dotnet build Tests\EChamado.E2E.Tests\' primeiro." -ForegroundColor Yellow
    exit 1
}

# Voltar para o diretório raiz
Set-Location "..\..\..\.."

Write-Host "Instalação concluída!" -ForegroundColor Green