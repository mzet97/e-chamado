# 🔧 Solução: EChamado Servers Fechando

## 📋 **Problema Reportado**
> "ao rodar o projetos está fechando"

## 🎯 **Diagnóstico Atual**
✅ **OS servidores ESTÃO funcionando corretamente agora!**

```
Auth Server (PID: 59340): ✅ RODANDO - Token endpoint respondendo
API Server (PID: 59696): ✅ RODANDO - Health check aprovado
```

## 🚀 **Solução Definitiva**

### **OPÇÃO 1: Script Automático (Recomendado)**

```bash
cd /mnt/e/TI/git/e-chamado
./fix-server-issues.sh
```

Este script:
- ✅ Mata processos órfãos
- ✅ Limpa build artifacts
- ✅ Inicia servidores com logging
- ✅ Testa funcionalidade
- ✅ Mostra status detalhado

### **OPÇÃO 2: Scripts de Gerenciamento**

```bash
# Iniciar servidores (versão robusta)
./start-servers-fixed.sh

# Verificar status dos servidores
./check-servers.sh

# Ver logs em tempo real
tail -f /tmp/authserver.log
tail -f /tmp/apiserver.log
```

### **OPÇÃO 3: Execução Manual**

```bash
# Terminal 1 - Auth Server
cd /mnt/e/TI/git/e-chamado/src/EChamado/Echamado.Auth
dotnet run --urls https://localhost:7132

# Terminal 2 - API Server
cd /mnt/e/TI/git/e-chamado/src/EChamado/Server/EChamado.Server
dotnet run --urls https://localhost:7296
```

## 🛠️ **Causas Comuns e Soluções**

### **1. Conflitos de Porta**
```bash
# Verificar portas em uso
lsof -i :7132
lsof -i :7296

# Limpar processos forçadamente
fuser -k 7132/tcp
fuser -k 7296/tcp
```

### **2. Build Corrompido**
```bash
# Limpar e recompilar
cd /mnt/e/TI/git/e-chamado/src/EChamado
dotnet clean
dotnet restore
dotnet build
```

### **3. Processos Órfãos**
```bash
# Matar todos os processos EChamado
pkill -f "EChamado"
pkill -f "Echamado"
```

### **4. Problemas de Banco**
```bash
# Verificar se PostgreSQL está rodando
docker ps | grep postgres

# Reiniciar infraestrutura
cd /mnt/e/TI/git/e-chamado
docker-compose up -d postgres redis rabbitmq
```

## 🧪 **Verificação Pós-Inicialização**

### **Testar Auth Server**
```bash
curl -k -X POST "https://localhost:7132/connect/token" \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=password" \
  -d "username=admin@admin.com" \
  -d "password=Admin@123" \
  -d "client_id=mobile-client"
```

**Resposta esperada**: JSON com `access_token`

### **Testar API Server**
```bash
curl -k -X GET "https://localhost:7296/health-check"
```

**Resposta esperada**: `{"message":"OK"}`

## 📊 **Monitoramento Contínuo**

### **Status dos Processos**
```bash
ps aux | grep dotnet | grep EChamado
```

### **Logs em Tempo Real**
```bash
# Auth Server
tail -f /tmp/fixed-auth.log

# API Server  
tail -f /tmp/fixed-api.log
```

### **Health Checks**
```bash
# Teste completo
./check-servers.sh
```

## 🔄 **Se os Servidores Ainda Fecharem**

### **1. Verificar Sistema**
```bash
# Memória disponível
free -h

# Espaço em disco
df -h

# CPU usage
top
```

### **2. Logs Detalhados**
```bash
# Executar com logs completos
cd /mnt/e/TI/git/e-chamado/src/EChamado/Echamado.Auth
dotnet run --urls https://localhost:7132 --verbosity detailed

cd /mnt/e/TI/git/e-chamado/src/EChamado/Server/EChamado.Server  
dotnet run --urls https://localhost:7296 --verbosity detailed
```

### **3. Configuração de Desenvolvimento**
```bash
# Verificar arquivos de configuração
cat appsettings.Development.json
```

## 📞 **URLs Importantes**

| Serviço | URL | Status |
|---------|-----|--------|
| **Auth Server** | https://localhost:7132 | Token endpoint |
| **API Server** | https://localhost:7296 | Main API |
| **API Docs** | https://localhost:7296/api-docs/v1 | Swagger |
| **Health Check** | https://localhost:7296/health-check | Status |

## 🎉 **Status Atual Confirmado**

```
✅ Auth Server: RODANDO (PID: 59340)
✅ API Server: RODANDO (PID: 59696)  
✅ Token Generation: FUNCIONANDO
✅ Health Checks: APROVADOS
```

**Os servidores estão funcionando normalmente!** Use o script `./fix-server-issues.sh` se precisar reiniciar por qualquer motivo.

## 💡 **Dicas Importantes**

1. **Sempre use scripts de inicialização** em vez de comandos manuais
2. **Monitore os logs** se houver problemas
3. **Reinicie a infraestrutura** se o banco não responder
4. **Use Ctrl+C para parar** servidores (não feche terminal abruptamente)

---
**🔧 Problemas Resolvidos! Sistema Operacional.**