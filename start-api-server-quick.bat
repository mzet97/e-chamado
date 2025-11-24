@echo off
echo 🔥 EChamado API Server - Solucao Rapida IOException
echo ===================================================

echo.
echo ⚡ Testando solucoes rapidas em ordem...
echo.

echo 1️⃣ Verificando se o Auth Server esta rodando na porta 7132...
netstat -ano | findstr :7132 >nul
if %errorlevel% equ 0 (
    echo ✅ Auth Server parece estar rodando
) else (
    echo ❌ Auth Server nao encontrado - iniciando...
    start cmd /k "cd /d E:\TI\git\e-chamado\src\EChamado\Echamado.Auth && dotnet run --urls https://localhost:7132"
    timeout /t 5 >nul
)

echo.
echo 2️⃣ Verificando portas e limpando processos conflitantes...
taskkill /f /im EChamado.Server.exe 2>nul
taskkill /f /im Echamado.Auth.exe 2>nul

for /f "tokens=5" %%a in ('netstat -ano ^| findstr :7296') do (
    echo Matando processo %%a na porta 7296
    taskkill /f /pid %%a 2>nul
)

timeout /t 3 >nul

echo.
echo 3️⃣ Tentativa 1 - Executar API Server com HTTP (evita problemas SSL)...
cd /d E:\TI\git\e-chamado\src\EChamado\Server\EChamado.Server
echo dotnet run --urls http://localhost:7296 --no-https
dotnet run --urls http://localhost:7296 --no-https --verbosity normal
goto :end

echo.
echo 4️⃣ Se falhou, tentando com HTTPS...
echo dotnet run --urls https://localhost:7296
dotnet run --urls https://localhost:7296 --verbosity normal
goto :end

:end
echo.
echo ================================================
echo 📋 SOLUÇÕES ALTERNATIVAS:
echo.
echo Se ainda falhar, tente:
echo 1. dotnet clean && dotnet restore && dotnet build
echo 2. Verificar se PostgreSQL esta rodando (docker ps)
echo 3. Usar script fix-windows-api-server.bat
echo 4. Ver arquivo SOLUÇÃO-WINDOWS-API-SERVER.md
echo.
pause