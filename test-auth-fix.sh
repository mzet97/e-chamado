#!/bin/bash
set -e

echo "🔧 Testando correção do erro 401 do token OpenIddict..."
echo

# Cores para output
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo "📋 PASSO 1: Verificando se o Auth Server está rodando..."
AUTH_STATUS=$(curl -k -s -o /dev/null -w "%{http_code}" https://localhost:7132/connect/token || echo "000")
if [ "$AUTH_STATUS" = "000" ]; then
    echo -e "${RED}❌ Auth Server (porta 7132) não está rodando${NC}"
    echo "   Inicie com: dotnet run --project src/EChamado/Echamado.Auth"
    exit 1
else
    echo -e "${GREEN}✅ Auth Server está rodando${NC}"
fi

echo
echo "📋 PASSO 2: Verificando se o API Server está rodando..."
API_STATUS=$(curl -s -o /dev/null -w "%{http_code}" https://localhost:7296/health || echo "000")
if [ "$API_STATUS" = "000" ]; then
    echo -e "${RED}❌ API Server (porta 7296) não está rodando${NC}"
    echo "   Inicie com: dotnet run --project src/EChamado/Server/EChamado.Server"
    exit 1
else
    echo -e "${GREEN}✅ API Server está rodando${NC}"
fi

echo
echo "📋 PASSO 3: Obtendo token de acesso..."
TOKEN_RESPONSE=$(curl -k -s -X POST https://localhost:7132/connect/token \
    -H "Content-Type: application/x-www-form-urlencoded" \
    -d "grant_type=password" \
    -d "username=admin@admin.com" \
    -d "password=Admin@123" \
    -d "client_id=mobile-client" \
    -d "scope=openid profile email roles api chamados" || echo "")

if [ -z "$TOKEN_RESPONSE" ]; then
    echo -e "${RED}❌ Falha ao obter token${NC}"
    exit 1
fi

ACCESS_TOKEN=$(echo "$TOKEN_RESPONSE" | grep -o '"access_token":"[^"]*' | cut -d'"' -f4 || echo "")
REFRESH_TOKEN=$(echo "$TOKEN_RESPONSE" | grep -o '"refresh_token":"[^"]*' | cut -d'"' -f4 || echo "")

# Fallback: extrair token usando sed se grep falhar
if [ -z "$ACCESS_TOKEN" ]; then
    ACCESS_TOKEN=$(echo "$TOKEN_RESPONSE" | sed 's/.*"access_token":"\([^"]*\)".*/\1/' || echo "")
fi

if [ -z "$ACCESS_TOKEN" ]; then
    echo -e "${RED}❌ Token não encontrado na resposta${NC}"
    echo "Resposta: $TOKEN_RESPONSE"
    exit 1
fi

echo -e "${GREEN}✅ Token obtido com sucesso${NC}"
echo "   Access Token: ${ACCESS_TOKEN:0:50}..."
echo "   Refresh Token: ${REFRESH_TOKEN:0:30}..."

echo
echo "📋 PASSO 4: Testando endpoint protegido no API Server..."
API_RESPONSE=$(curl -s -w "HTTPSTATUS:%{http_code}" -X POST https://localhost:7296/v1/category \
    -H "Content-Type: application/json" \
    -H "Authorization: Bearer $ACCESS_TOKEN" \
    -d '{"title": "Teste Categoria", "description": "Categoria de teste"}' || echo "HTTPSTATUS:000")

HTTP_STATUS=$(echo "$API_RESPONSE" | grep -o "HTTPSTATUS:[0-9]*" | cut -d: -f2)
RESPONSE_BODY=$(echo "$API_RESPONSE" | sed 's/HTTPSTATUS:[0-9]*$//')

if [ "$HTTP_STATUS" = "000" ]; then
    echo -e "${RED}❌ Falha ao conectar com API Server${NC}"
    exit 1
elif [ "$HTTP_STATUS" = "200" ]; then
    echo -e "${GREEN}✅ SUCESSO! Token foi aceito pelo API Server${NC}"
    echo "   Status: $HTTP_STATUS"
    echo "   Resposta: $RESPONSE_BODY"
elif [ "$HTTP_STATUS" = "401" ]; then
    echo -e "${RED}❌ ERRO 401 - Token ainda sendo rejeitado${NC}"
    echo "   Resposta: $RESPONSE_BODY"
    echo
    echo "Possíveis causas:"
    echo "1. Auth Server ainda não foi reiniciado com a correção"
    echo "2. Token emitido é opaco (não-JWT)"
    echo "3. Problema de configuração de issuer/audience"
    exit 1
else
    echo -e "${YELLOW}⚠️  Status HTTP inesperado: $HTTP_STATUS${NC}"
    echo "   Resposta: $RESPONSE_BODY"
fi

echo
echo "📋 PASSO 5: Verificando se o token é um JWT válido..."
# Decodificar base64 do payload do JWT (sem verificação de assinatura)
PAYLOAD=$(echo "$ACCESS_TOKEN" | cut -d'.' -f2)
# Adicionar padding se necessário
PADDING=$(echo "$PAYLOAD" | grep -o '=' || echo "")
PAYLOAD=$(echo "$PAYLOAD"$(printf '=%.0s' {1..$((4 - ${#PAYLOAD} % 4))} 2>/dev/null | tr ' ' '=') || echo "$PAYLOAD")

if command -v base64 >/dev/null 2>&1; then
    JWT_PAYLOAD=$(echo "$PAYLOAD" | base64 -d 2>/dev/null || echo "{}")
    echo "JWT Payload: $JWT_PAYLOAD"
fi

echo
echo -e "${GREEN}🎉 Teste concluído!${NC}"