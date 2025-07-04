# Script PowerShell para executar todos os projetos EChamado
# Execute este script do diretório raiz do projeto

Write-Host "🚀 Iniciando EChamado..." -ForegroundColor Green
Write-Host ""

# Verificar se o Docker está rodando
try {
    docker info | Out-Null
} catch {
    Write-Host "❌ Docker não está rodando. Por favor, inicie o Docker primeiro." -ForegroundColor Red
    exit 1
}

# Subir os serviços de infraestrutura
Write-Host "🐳 Subindo serviços de infraestrutura..." -ForegroundColor Cyan
docker-compose up -d postgres redis elasticsearch kibana logstash rabbitmq pgadmin

# Aguardar alguns segundos para os serviços subirem
Write-Host "⏳ Aguardando serviços ficarem prontos..." -ForegroundColor Yellow
Start-Sleep -Seconds 15

# Função para executar um projeto em background
function Start-Project {
    param(
        [string]$ProjectPath,
        [string]$ProjectName,
        [string]$Port
    )
    
    Write-Host "🏃 Executando $ProjectName em $Port..." -ForegroundColor Green
    
    # Criar diretório de logs se não existir
    if (-not (Test-Path "logs")) {
        New-Item -ItemType Directory -Path "logs" | Out-Null
    }
    
    # Executar o projeto em background
    $process = Start-Process -FilePath "dotnet" -ArgumentList "run", "--urls", "https://localhost:$Port" -WorkingDirectory $ProjectPath -WindowStyle Hidden -PassThru
    $process.Id | Out-File -FilePath "logs/$ProjectName.pid" -Encoding utf8
    
    return $process
}

# Executar os projetos na ordem correta
Write-Host ""
Write-Host "📋 Executando projetos..." -ForegroundColor Cyan

# 1. EChamado.Server (OpenIddict Server)
$serverProcess = Start-Project -ProjectPath "Server/EChamado.Server" -ProjectName "EChamado.Server" -Port "7296"

# Aguardar o servidor subir
Write-Host "⏳ Aguardando EChamado.Server inicializar..." -ForegroundColor Yellow
Start-Sleep -Seconds 10

# 2. EChamado.Auth (UI de Autenticação)
$authProcess = Start-Project -ProjectPath "Echamado.Auth" -ProjectName "EChamado.Auth" -Port "7132"

# Aguardar o Auth subir
Write-Host "⏳ Aguardando EChamado.Auth inicializar..." -ForegroundColor Yellow
Start-Sleep -Seconds 8

# 3. EChamado.Client (Blazor WebAssembly)
$clientProcess = Start-Project -ProjectPath "Client/EChamado.Client" -ProjectName "EChamado.Client" -Port "7274"

Write-Host ""
Write-Host "✅ Todos os projetos foram iniciados!" -ForegroundColor Green
Write-Host ""
Write-Host "🌐 URLs disponíveis:" -ForegroundColor Cyan
Write-Host "   • Client (Blazor WASM): https://localhost:7274" -ForegroundColor White
Write-Host "   • Auth (UI Login):      https://localhost:7132" -ForegroundColor White
Write-Host "   • Server (API):         https://localhost:7296" -ForegroundColor White
Write-Host "   • Swagger:              https://localhost:7296/swagger" -ForegroundColor White
Write-Host ""
Write-Host "📊 Serviços de infraestrutura:" -ForegroundColor Cyan
Write-Host "   • PgAdmin:              http://localhost:15432" -ForegroundColor White
Write-Host "   • Kibana:               http://localhost:5601" -ForegroundColor White
Write-Host "   • RabbitMQ:             http://localhost:15672" -ForegroundColor White
Write-Host ""
Write-Host "📜 Logs disponíveis em: ./logs/" -ForegroundColor Cyan
Write-Host ""
Write-Host "🛑 Para parar todos os serviços, execute: .\stop-all.ps1" -ForegroundColor Red
