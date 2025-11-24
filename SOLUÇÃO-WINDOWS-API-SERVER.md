# 🔧 Solução: EChamado API Server IOException no Windows

## 🎯 **Problema Identificado**

```
Exception thrown: 'System.IO.IOException' in System.Private.CoreLib.dll
The program '[12568] EChamado.Server.exe' has exited with code 1 (0x1).
```

## 🔍 **Causa do System.IO.IOException**

O log mostra que todas as DLLs carregaram corretamente, mas o servidor falha ao inicializar devido a um problema de I/O. As causas mais comuns são:

### **1. Porta já em uso (Mais Comum)**
- Outro processo usando porta 7296
- Processo EChamado.Server.exe pendurado
- Visual Studio debug session conflitos

### **2. Problemas de SSL/HTTPS**
- Certificado de desenvolvimento não gerado
- Conflito de configuração SSL
- Permissões de certificado

### **3. Problemas de Banco de Dados**
- PostgreSQL não acessível
- String de conexão inválida
- Permissões de banco

### **4. Configuração de Diretórios**
- Problemas de permissão de pasta
- Caminhos inválidos
- Arquivos de configuração corrompidos

---

## 🚀 **Soluções Rápidas (Teste nesta ordem)**

### **🔧 Solução 1: Script de Limpeza Windows**

```cmd
# Execute no Command Prompt como Administrador
cd E:\TI\git\e-chamado

# Executar script de correção
fix-windows-api-server.bat
```

### **🔧 Solução 2: Limpeza Manual**

```cmd
# 1. Matar todos os processos EChamado
taskkill /f /im EChamado.Server.exe
taskkill /f /im Echamado.Auth.exe

# 2. Verificar portas
netstat -ano | findstr :7296
netstat -ano | findstr :7132

# 3. Se portas estiverem em uso, matar processos
for /f "tokens=5" %a in ('netstat -ano ^| findstr :7296') do taskkill /f /pid %a

# 4. Limpar e recompilar
cd E:\TI\git\e-chamado\src\EChamado
dotnet clean
dotnet restore
dotnet build

# 5. Executar API Server
cd Server\EChamado.Server
dotnet run --urls http://localhost:7296 --verbosity detailed
```

### **🔧 Solução 3: Forçar Parada e Reiniciar**

```cmd
# Forçar parada de qualquer processo na porta 7296
netstat -ano | findstr :7296
# Note o PID e execute:
taskkill /f /pid [PID_ENCONTRADO]

# Limpar cache do Visual Studio
del /s /q E:\TI\git\e-chamado\.vs
del /s /q E:\TI\git\e-chamado\bin
del /s /q E:\TI\git\e-chamado\obj

# Rebuild completo
dotnet clean
dotnet build
```

### **🔧 Solução 4: Verificar Auth Server Primeiro**

```cmd
# O Auth Server deve estar rodando primeiro na porta 7132
cd E:\TI\git\e-chamado\Echamado.Auth
dotnet run --urls https://localhost:7132

# Em outro terminal, executar API Server
cd E:\TI\git\e-chamado\Server\EChamado.Server
dotnet run --urls https://localhost:7296
```

---

## 🛠️ **Soluções Específicas para IOException**

### **Solução A: Usar HTTP em vez de HTTPS**

Se o problema for SSL, teste com HTTP:

```cmd
cd E:\TI\git\e-chamado\Server\EChamado.Server
dotnet run --urls http://localhost:7296 --no-https
```

### **Solução B: Verificar Banco de Dados**

```cmd
# Verificar se PostgreSQL está rodando (Docker)
docker ps | findstr postgres

# Se não estiver, iniciar infraestrutura
docker-compose up -d postgres redis rabbitmq
```

### **Solução C: Verificar Permissões**

```cmd
# Verificar se tem permissão na pasta do projeto
icacls E:\TI\git\e-chamado /grant %USERNAME%:F /T

# Limpar arquivos temporários
del /q /s %TEMP%\EChamado*
```

### **Solução D: Configuração de Desenvolvimento**

```cmd
# Forçar modo desenvolvimento
set ASPNETCORE_ENVIRONMENT=Development
cd E:\TI\git\e-chamado\Server\EChamado.Server
dotnet run --urls https://localhost:7296 --environment Development
```

---

## 🔍 **Diagnóstico Detalhado**

### **Checklist de Verificação**

1. **Portas livres:**
   ```cmd
   netstat -ano | findstr :7296
   netstat -ano | findstr :7132
   ```

2. **Processos EChamado:**
   ```cmd
   tasklist | findstr EChamado
   ```

3. **Banco de dados:**
   ```cmd
   docker ps | findstr postgres
   ```

4. **Certificados SSL:**
   ```cmd
   dir E:\TI\git\e-chamado\.aspnet\https
   ```

5. **Permissões:**
   ```cmd
   whoami
   icacls E:\TI\git\e-chamado | findstr %USERNAME%
   ```

---

## 🚨 **Se Nada Funcionar - Solução Nuclear**

```cmd
# 1. Parar todos os processos
taskkill /f /im dotnet.exe
taskkill /f /im EChamado*
taskkill /f /im Echamado*

# 2. Limpar tudo
cd E:\TI\git\e-chamado
rmdir /s /q bin
rmdir /s /q obj
rmdir /s /q .vs

# 3. Restaurar packages
dotnet restore

# 4. Rebuild
dotnet build

# 5. Executar com logging máximo
cd Server\EChamado.Server
dotnet run --urls https://localhost:7296 --verbosity diagnostic
```

---

## 📞 **Informações de Depuração**

### **Locais de Log**
- **Visual Studio**: Output Window → Debug
- **Console**: onde executar `dotnet run`
- **Windows Event Viewer**: Application Logs

### **URLs de Teste**
- **Health Check**: http://localhost:7296/health-check
- **API Docs**: http://localhost:7296/api-docs/v1
- **Auth Token**: https://localhost:7132/connect/token

---

## ✅ **Status Esperado Após Correção**

```
info: Microsoft.Hosting.Lifetime[0]
      Now listening on: https://localhost:7296
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

Se vir essa mensagem, o servidor está funcionando corretamente!

---

## 🆘 **Última Opção - Contact Support**

Se nenhuma solução funcionar, colete estas informações:

```cmd
# Informações do sistema
systeminfo | findstr /B /C:"OS Name" /C:"OS Version"

# Informações do .NET
dotnet --info

# Espaço em disco
dir E:\TI\git\e-chamado

# Processes
tasklist > processes.txt
netstat -ano > ports.txt
```

Com essas informações, poderemos diagnosticar problemas mais específicos.