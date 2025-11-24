#!/bin/bash
set -e

echo "🔧 Script para resolver problemas de execução dos projetos..."
echo

# Cores para output
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo "📋 PASSO 1: Finalizando todos os processos .NET..."
pkill -f "dotnet run" || echo "Nenhum processo dotnet run encontrado"
sleep 2

echo "📋 PASSO 2: Verificando se as portas estão livres..."
echo "Verificando porta 7132 (Auth Server)..."
lsof -i :7132 2>/dev/null && echo -e "${RED}❌ Porta 7132 ainda em uso${NC}" || echo -e "${GREEN}✅ Porta 7132 livre${NC}"

echo "Verificando porta 7296 (API Server)..."
lsof -i :7296 2>/dev/null && echo -e "${RED}❌ Porta 7296 ainda em uso${NC}" || echo -e "${GREEN}✅ Porta 7296 livre${NC}"

echo
echo "📋 PASSO 3: Executando limpeza geral..."
# Remove logs temporários
rm -f /tmp/auth.log /tmp/api.log 2>/dev/null || true

echo
echo "📋 PASSO 4: Iniciando servidores..."
echo "Iniciando Auth Server na porta 7132..."

# Inicia Auth Server em background
cd /mnt/e/TI/git/e-chamado/src/EChamado/Echamado.Auth
nohup dotnet run --urls "https://localhost:7132" > /tmp/auth.log 2>&1 &
AUTH_PID=$!
echo "Auth Server iniciado com PID: $AUTH_PID"

# Aguarda inicialização
sleep 5

# Verifica se Auth Server está funcionando
if curl -k -s -o /dev/null https://localhost:7132/connect/token; then
    echo -e "${GREEN}✅ Auth Server funcionando!${NC}"
else
    echo -e "${RED}❌ Auth Server não está respondendo${NC}"
    echo "Verificar logs: tail -f /tmp/auth.log"
fi

echo
echo "Iniciando API Server na porta 7296..."
cd /mnt/e/TI/git/e-chamado/src/EChamado/Server/EChamado.Server
nohup dotnet run --urls "https://localhost:7296" > /tmp/api.log 2>&1 &
API_PID=$!
echo "API Server iniciado com PID: $API_PID"

# Aguarda inicialização
sleep 5

# Verifica se API Server está funcionando
if curl -k -s -o /dev/null https://localhost:7296/health; then
    echo -e "${GREEN}✅ API Server funcionando!${NC}"
else
    echo -e "${RED}❌ API Server não está respondendo${NC}"
    echo "Verificar logs: tail -f /tmp/api.log"
fi

echo
echo "📋 PASSO 5: Teste de conectividade..."
echo "Testando token endpoint..."
TOKEN_RESPONSE=$(curl -k -s -X POST https://localhost:7132/connect/token \
    -H "Content-Type: application/x-www-form-urlencoded" \
    -d "grant_type=password" \
    -d "username=admin@admin.com" \
    -d "password=Admin@123" \
    -d "client_id=mobile-client" \
    -d "scope=openid profile email roles api chamados" 2>/dev/null)

if [ -n "$TOKEN_RESPONSE" ]; then
    echo -e "${GREEN}✅ Token obtido com sucesso!${NC}"
    echo "Testando API com token..."
    
    ACCESS_TOKEN=$(echo "$TOKEN_RESPONSE" | sed 's/.*"access_token":"\([^"]*\)".*/\1/' 2>/dev/null || echo "")
    if [ -n "$ACCESS_TOKEN" ]; then
        API_RESPONSE=$(curl -k -w "HTTPSTATUS:%{http_code}" -s https://localhost:7296/v1/categories \
            -H "Authorization: Bearer $ACCESS_TOKEN" \
            -H "Content-Type: application/json" 2>/dev/null)
        
        HTTP_CODE=$(echo "$API_RESPONSE" | grep -o "HTTPSTATUS:[0-9]*" | cut -d: -f2 2>/dev/null || echo "000")
        
        if [ "$HTTP_CODE" = "200" ] || [ "$HTTP_CODE" = "401" ]; then
            echo -e "${GREEN}✅ API Server respondendo!${NC}"
            echo "Status: $HTTP_CODE"
        else
            echo -e "${YELLOW}⚠️  API Server com problemas${NC}"
            echo "Status: $HTTP_CODE"
        fi
    fi
else
    echo -e "${RED}❌ Falha ao obter token${NC}"
fi

echo
echo "📋 RESUMO:"
echo "   Auth Server (porta 7132): PID $AUTH_PID"
echo "   API Server (porta 7296): PID $API_PID"
echo "   Logs disponíveis em:"
echo "     - /tmp/auth.log"
echo "     - /tmp/api.log"
echo
echo "🔧 Para parar os servidores:"
echo "   kill $AUTH_PID $API_PID"
echo
echo "📝 Para acompanhar logs:"
echo "   tail -f /tmp/auth.log"
echo "   tail -f /tmp/api.log"
echo
echo -e "${GREEN}🎉 Script concluído!${NC}"