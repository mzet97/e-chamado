@echo off
echo 🔧 EChamado API Server Windows Fix
echo ===================================

echo 🧹 Step 1: Killing existing EChamado processes...
taskkill /f /im EChamado.Server.exe 2>nul
taskkill /f /im Echamado.Auth.exe 2>nul
timeout /t 2 >nul

echo 🔍 Step 2: Checking port availability...
echo Checking port 7296 (API Server)...
netstat -ano | findstr :7296
if %errorlevel% equ 0 (
    echo ❌ Port 7296 is in use - killing process...
    for /f "tokens=5" %%a in ('netstat -ano ^| findstr :7296') do taskkill /f /pid %%a
) else (
    echo ✅ Port 7296 is available
)

echo Checking port 7132 (Auth Server)...
netstat -ano | findstr :7132
if %errorlevel% equ 0 (
    echo ⚠️  Port 7132 is in use - Auth Server might be running
) else (
    echo ✅ Port 7132 is available
)

echo 🏗️ Step 3: Cleaning build artifacts...
cd /d "E:\TI\git\e-chamado\src\EChamado"
echo Cleaning solution...
dotnet clean --verbosity quiet

echo 🛠️ Step 4: Checking SSL certificates...
echo SSL certificates should be auto-generated in development mode
echo If issues persist, try using HTTP instead of HTTPS

echo 🚀 Step 5: Starting API Server with verbose output...
cd /d "E:\TI\git\e-chamado\src\EChamado\Server\EChamado.Server"
echo Starting API Server...
dotnet run --urls http://localhost:7296 --verbosity detailed --no-build

pause