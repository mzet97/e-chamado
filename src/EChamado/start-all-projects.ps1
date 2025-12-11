# Script para executar todos os projetos EChamado
Write-Host "=== Iniciando EChamado - Todos os Projetos ===" -ForegroundColor Green

# Verificar se as portas estão livres
$ports = @(5136, 7132, 5071, 7296, 5199, 7274)
foreach ($port in $ports) {
    $connection = Get-NetTCPConnection -LocalPort $port -ErrorAction SilentlyContinue
    if ($connection) {
        Write-Warning "Porta $port está em uso. PID: $($connection.OwningProcess)"
        $process = Get-Process -Id $connection.OwningProcess -ErrorAction SilentlyContinue
        if ($process) {
            Write-Host "Processo: $($process.ProcessName)" -ForegroundColor Yellow
        }
    }
}

Write-Host "`n=== Compilando projetos ===" -ForegroundColor Blue
dotnet build --configuration Debug

if ($LASTEXITCODE -ne 0) {
    Write-Error "Falha na compilação"
    exit 1
}

Write-Host "`n=== Iniciando projetos ===" -ForegroundColor Blue

# Função para iniciar projeto em nova janela
function Start-Project {
    param(
        [string]$ProjectPath,
        [string]$ProjectName,
        [string]$LaunchProfile = "https"
    )
    
    Write-Host "Iniciando $ProjectName..." -ForegroundColor Cyan
    
    $startInfo = New-Object System.Diagnostics.ProcessStartInfo
    $startInfo.FileName = "dotnet"
    $startInfo.Arguments = "run --project `"$ProjectPath`" --launch-profile $LaunchProfile"
    $startInfo.WorkingDirectory = Split-Path $ProjectPath
    $startInfo.CreateNoWindow = $false
    $startInfo.UseShellExecute = $true
    
    $process = [System.Diagnostics.Process]::Start($startInfo)
    return $process
}

# Iniciar Auth Server
$authProcess = Start-Project -ProjectPath "Echamado.Auth\Echamado.Auth.csproj" -ProjectName "Auth Server"
Start-Sleep -Seconds 3

# Iniciar API Server  
$serverProcess = Start-Project -ProjectPath "Server\EChamado.Server\EChamado.Server.csproj" -ProjectName "API Server"
Start-Sleep -Seconds 3

# Iniciar Client (sem debug)
$clientProcess = Start-Project -ProjectPath "Client\EChamado.Client\EChamado.Client.csproj" -ProjectName "Blazor Client" -LaunchProfile "https-no-debug"

Write-Host "`n=== Todos os projetos iniciados ===" -ForegroundColor Green
Write-Host "Auth Server: https://localhost:7132" -ForegroundColor Yellow
Write-Host "API Server:  https://localhost:7296" -ForegroundColor Yellow  
Write-Host "Client:      https://localhost:7274" -ForegroundColor Yellow

Write-Host "`nPressione qualquer tecla para finalizar todos os processos..." -ForegroundColor Cyan
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")

# Finalizar processos
Write-Host "`n=== Finalizando processos ===" -ForegroundColor Red
if ($authProcess -and !$authProcess.HasExited) { $authProcess.Kill() }
if ($serverProcess -and !$serverProcess.HasExited) { $serverProcess.Kill() }
if ($clientProcess -and !$clientProcess.HasExited) { $clientProcess.Kill() }

Write-Host "Todos os processos finalizados." -ForegroundColor Green