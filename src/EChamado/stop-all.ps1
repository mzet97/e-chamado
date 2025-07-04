# Script PowerShell para parar todos os serviços EChamado

Write-Host "🛑 Parando todos os serviços EChamado..." -ForegroundColor Red
Write-Host ""

# Função para parar processo por PID
function Stop-ProcessByPidFile {
    param([string]$PidFile, [string]$ProcessName)
    
    if (Test-Path $PidFile) {
        try {
            $pid = Get-Content $PidFile -ErrorAction SilentlyContinue
            if ($pid) {
                Write-Host "🔴 Parando $ProcessName (PID: $pid)..." -ForegroundColor Yellow
                Stop-Process -Id $pid -Force -ErrorAction SilentlyContinue
                Remove-Item $PidFile -ErrorAction SilentlyContinue
            }
        } catch {
            Write-Host "⚠️ Erro ao parar $ProcessName" -ForegroundColor Yellow
        }
    }
}

# Parar os processos .NET se existirem
Stop-ProcessByPidFile -PidFile "logs/EChamado.Server.pid" -ProcessName "EChamado.Server"
Stop-ProcessByPidFile -PidFile "logs/EChamado.Auth.pid" -ProcessName "EChamado.Auth"
Stop-ProcessByPidFile -PidFile "logs/EChamado.Client.pid" -ProcessName "EChamado.Client"

# Parar todos os processos dotnet que possam estar rodando
Write-Host "🔴 Parando todos os processos dotnet..." -ForegroundColor Yellow
Get-Process -Name "dotnet" -ErrorAction SilentlyContinue | Stop-Process -Force -ErrorAction SilentlyContinue

# Parar containers Docker
Write-Host "🐳 Parando containers Docker..." -ForegroundColor Cyan
docker-compose down

Write-Host ""
Write-Host "✅ Todos os serviços foram parados!" -ForegroundColor Green
