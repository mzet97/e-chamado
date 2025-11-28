# Script para limpar e reconstruir o EChamado.Server no Windows
# Execute este script no PowerShell como Administrador

Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "Limpando projeto EChamado.Server" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan

# Parar processos dotnet que possam estar usando os arquivos
Write-Host "`nParando processos dotnet..." -ForegroundColor Yellow
Get-Process -Name "dotnet" -ErrorAction SilentlyContinue | Stop-Process -Force -ErrorAction SilentlyContinue
Start-Sleep -Seconds 2

# Navegue para o diretório do projeto
$projectPath = "E:\TI\git\e-chamado\src\EChamado\Server\EChamado.Server"
Set-Location $projectPath

Write-Host "`nDiretório atual: $projectPath" -ForegroundColor Green

# 1. Limpar com dotnet clean
Write-Host "`n[1/6] Executando dotnet clean..." -ForegroundColor Yellow
dotnet clean --verbosity quiet

# 2. Remover pastas bin e obj RECURSIVAMENTE (incluindo subpastas)
Write-Host "`n[2/6] Removendo pastas bin e obj..." -ForegroundColor Yellow
Get-ChildItem -Path . -Include bin,obj -Recurse -Directory -Force | Remove-Item -Recurse -Force -ErrorAction SilentlyContinue

# 3. Remover arquivos .staticwebassets específicos
Write-Host "`n[3/6] Removendo arquivos staticwebassets..." -ForegroundColor Yellow
Get-ChildItem -Path . -Recurse -Filter "*.staticwebassets.*" | Remove-Item -Force -ErrorAction SilentlyContinue

# 4. Limpar cache do NuGet local (opcional mas recomendado)
Write-Host "`n[4/6] Limpando cache NuGet local..." -ForegroundColor Yellow
dotnet nuget locals all --clear

# 5. Restaurar pacotes
Write-Host "`n[5/6] Restaurando pacotes NuGet..." -ForegroundColor Yellow
dotnet restore --force

# 6. Reconstruir o projeto
Write-Host "`n[6/6] Reconstruindo o projeto..." -ForegroundColor Yellow
dotnet build --no-restore

Write-Host "`n=========================================" -ForegroundColor Green
Write-Host "Limpeza e rebuild concluídos!" -ForegroundColor Green
Write-Host "=========================================" -ForegroundColor Green

Write-Host "`nPara executar o projeto, use:" -ForegroundColor Cyan
Write-Host "  dotnet run --launch-profile https" -ForegroundColor White

Write-Host "`nOu execute este comando agora? (S/N): " -ForegroundColor Yellow -NoNewline
$response = Read-Host

if ($response -eq 'S' -or $response -eq 's') {
    Write-Host "`nIniciando o servidor API..." -ForegroundColor Green
    dotnet run --launch-profile https
} else {
    Write-Host "`nScript finalizado. Execute manualmente quando estiver pronto." -ForegroundColor Cyan
}
