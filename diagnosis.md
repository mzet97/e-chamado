# 🔧 EChamado Server Diagnosis Report

## ✅ Current Status
The servers appear to be running successfully. Here's what we found:

### Auth Server (Port 7132)
- ✅ Process is running
- ✅ Token endpoint is responding
- ✅ Accepts authentication requests

### API Server (Port 7296)  
- ✅ Process is running
- ✅ Health check endpoint responds
- ✅ Authentication system is working

## 🚀 How to Start Servers (Correct Method)

### Option 1: Use the Fixed Startup Script
```bash
cd /mnt/e/TI/git/e-chamado
./start-servers-fixed.sh
```

### Option 2: Manual Start (Alternative)
```bash
# Terminal 1 - Auth Server
cd /mnt/e/TI/git/e-chamado/src/EChamado/Echamado.Auth
dotnet run --urls https://localhost:7132

# Terminal 2 - API Server  
cd /mnt/e/TI/git/e-chamado/src/EChamado/Server/EChamado.Server
dotnet run --urls https://localhost:7296
```

### Option 3: Use Docker Compose (Infrastructure Only)
```bash
cd /mnt/e/TI/git/e-chamado
docker-compose up -d postgres redis rabbitmq
```

## 🐛 If Servers Keep Closing

### Possible Causes:
1. **Port conflicts** - Another process is using the ports
2. **Database connection issues** - PostgreSQL not running
3. **Build errors** - Compilation issues in the code
4. **Memory issues** - Insufficient system resources
5. **SSL certificate issues** - HTTPS configuration problems

### Solutions:
1. **Kill all EChamado processes** and restart
2. **Check PostgreSQL is running** (`docker ps` or `pg_isready`)
3. **Clean and rebuild** (`dotnet clean && dotnet build`)
4. **Check system resources** (`free -h`, `df -h`)
5. **Check SSL certificates** in development mode

## 🔍 Troubleshooting Commands

```bash
# Check if servers are running
./check-servers.sh

# Check port usage
lsof -i :7132
lsof -i :7296

# Check recent logs
tail -f /tmp/authserver.log
tail -f /tmp/apiserver.log

# Kill all EChamado processes
pkill -f "EChamado"
```

## 📞 Testing Endpoints

```bash
# Test Auth Server
curl -k -X POST "https://localhost:7132/connect/token" \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=password&username=admin@admin.com&password=Admin@123&client_id=mobile-client"

# Test API Server
curl -k -X GET "https://localhost:7296/health-check"
```

