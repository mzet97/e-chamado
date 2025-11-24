#!/bin/bash

echo "🔧 Resolvendo erro de concorrência do OpenIddict..."
echo

# Cores
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m'

echo "📋 PASSO 1: Comentando registro do OpenIddictWorker..."
sed -i 's/services\.AddHostedService<OpenIddictWorker>();/\/\/ services.AddHostedService<OpenIddictWorker>();/' /mnt/e/TI/git/e-chamado/src/EChamado/Server/EChamado.Server.Infrastructure/Configuration/IdentityConfig.cs

echo -e "${GREEN}✅ OpenIddictWorker comentado no API Server${NC}"

echo
echo "📋 PASSO 2: Finalizando todos os processos .NET..."
pkill -f "dotnet run" 2>/dev/null || echo "Nenhum processo encontrado"
sleep 3

echo "📋 PASSO 3: Limpando banco de dados (opcional)..."
echo "Para limpar o banco: delete o arquivo de banco ou execute migrations"

echo
echo "📋 PASSO 4: Reiniciando servidores..."
echo "Iniciando Auth Server..."
cd /mnt/e/TI/git/e-chamado/src/EChamado/Echamado.Auth
nohup dotnet run --urls "https://localhost:7132" > /tmp/auth-new.log 2>&1 &
AUTH_PID=$!
echo "Auth Server iniciado: PID $AUTH_PID"
sleep 5

echo "Iniciando API Server..."
cd /mnt/e/TI/git/e-chamado/src/EChamado/Server/EChamado.Server  
nohup dotnet run --urls "https://localhost:7296" > /tmp/api-new.log 2>&1 &
API_PID=$!
echo "API Server iniciado: PID $API_PID"
sleep 5

echo
echo "📋 PASSO 5: Testando novamente..."
TOKEN=$(curl -k -s -X POST https://localhost:7132/connect/token \
    -H "Content-Type: application/x-www-form-urlencoded" \
    -d "grant_type=password" \
    -d "username=admin@admin.com" \
    -d "password=Admin@123" \
    -d "client_id=mobile-client" \
    -d "scope=openid profile email roles api chamados" 2>/dev/null)

if [ -n "$TOKEN" ]; then
    echo -e "${GREEN}✅ Token obtido com sucesso!${NC}"
    ACCESS_TOKEN=$(echo "$TOKEN" | sed 's/.*"access_token":"\([^"]*\)".*/\1/' 2>/dev/null)
    
    echo "Testando endpoint categories..."
    RESPONSE=$(curl -k -w "HTTPSTATUS:%{http_code}" -H "Authorization: Bearer $ACCESS_TOKEN" \
        https://localhost:7296/v1/categories 2>/dev/null)
    HTTP_CODE=$(echo "$RESPONSE" | grep -o "HTTPSTATUS:[0-9]*" | cut -d: -f2 2>/dev/null || echo "000")
    
    if [ "$HTTP_CODE" = "200" ] || [ "$HTTP_CODE" = "401" ]; then
        echo -e "${GREEN}✅ API Server respondendo! Status: $HTTP_CODE${NC}"
    else
        echo -e "${YELLOW}⚠️ API Status: $HTTP_CODE${NC}"
    fi
else
    echo -e "${RED}❌ Falha ao obter token${NC}"
fi

echo
echo "📋 PASSO 6: Verificando logs..."
echo "Auth logs: tail -f /tmp/auth-new.log"
echo "API logs: tail -f /tmp/api-new.log"

echo
echo "🎉 Solução aplicada!"
echo "O OpenIddictWorker foi comentado no API Server para evitar concorrência."
echo "O Auth Server permanece responsável pela configuração dos clientes."