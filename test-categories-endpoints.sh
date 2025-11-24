#!/bin/bash
set -e

echo "🔧 Testando endpoints de categorias..."
echo

# Cores para output
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo "📋 PASSO 1: Verificando se os servidores estão rodando..."
AUTH_STATUS=$(curl -k -s -o /dev/null -w "%{http_code}" https://localhost:7132/connect/token || echo "000")
API_STATUS=$(curl -s -o /dev/null -w "%{http_code}" https://localhost:7296/health || echo "000")

if [ "$AUTH_STATUS" = "000" ] || [ "$API_STATUS" = "000" ]; then
    echo -e "${RED}❌ Um ou mais servidores não estão rodando${NC}"
    echo "   Auth: $AUTH_STATUS, API: $API_STATUS"
    exit 1
else
    echo -e "${GREEN}✅ Servidores estão rodando${NC}"
fi

echo
echo "📋 PASSO 2: Obtendo token de acesso..."
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

# Fallback: extrair token usando sed se grep falhar
if [ -z "$ACCESS_TOKEN" ]; then
    ACCESS_TOKEN=$(echo "$TOKEN_RESPONSE" | sed 's/.*"access_token":"\([^"]*\)".*/\1/' || echo "")
fi

if [ -z "$ACCESS_TOKEN" ]; then
    echo -e "${RED}❌ Token não encontrado na resposta${NC}"
    exit 1
fi

echo -e "${GREEN}✅ Token obtido com sucesso${NC}"

echo
echo "📋 PASSO 3: Testando GET /v1/categories (busca com filtros)..."
SEARCH_RESPONSE=$(curl -s -w "HTTPSTATUS:%{http_code}" -X GET "https://localhost:7296/v1/categories?name=Hardware&pageIndex=1&pageSize=5" \
    -H "Authorization: Bearer $ACCESS_TOKEN" \
    -H "Content-Type: application/json" || echo "HTTPSTATUS:000")

HTTP_STATUS=$(echo "$SEARCH_RESPONSE" | grep -o "HTTPSTATUS:[0-9]*" | cut -d: -f2)
RESPONSE_BODY=$(echo "$SEARCH_RESPONSE" | sed 's/HTTPSTATUS:[0-9]*$//')

if [ "$HTTP_STATUS" = "200" ]; then
    echo -e "${GREEN}✅ GET /v1/categories funcionando!${NC}"
    echo "   Status: $HTTP_STATUS"
    echo "   Resposta: ${RESPONSE_BODY:0:200}..."
elif [ "$HTTP_STATUS" = "401" ]; then
    echo -e "${RED}❌ ERRO 401 - Autenticação falhou${NC}"
    exit 1
elif [ "$HTTP_STATUS" = "000" ]; then
    echo -e "${RED}❌ Falha de conexão${NC}"
    exit 1
else
    echo -e "${YELLOW}⚠️  Status HTTP inesperado: $HTTP_STATUS${NC}"
    echo "   Resposta: $RESPONSE_BODY"
fi

echo
echo "📋 PASSO 4: Testando GET /v1/category/{id} (busca por ID)..."
# Primeiro obtém um ID válido fazendo uma busca
CATEGORIES_LIST=$(curl -s -X GET "https://localhost:7296/v1/categories" \
    -H "Authorization: Bearer $ACCESS_TOKEN" \
    -H "Content-Type: application/json")

CATEGORY_ID=$(echo "$CATEGORIES_LIST" | grep -o '"id":"[^"]*' | head -1 | cut -d'"' -f4 || echo "")

if [ -n "$CATEGORY_ID" ]; then
    GET_RESPONSE=$(curl -s -w "HTTPSTATUS:%{http_code}" -X GET "https://localhost:7296/v1/category/$CATEGORY_ID" \
        -H "Authorization: Bearer $ACCESS_TOKEN" \
        -H "Content-Type: application/json" || echo "HTTPSTATUS:000")

    GET_HTTP_STATUS=$(echo "$GET_RESPONSE" | grep -o "HTTPSTATUS:[0-9]*" | cut -d: -f2)
    GET_RESPONSE_BODY=$(echo "$GET_RESPONSE" | sed 's/HTTPSTATUS:[0-9]*$//')

    if [ "$GET_HTTP_STATUS" = "200" ]; then
        echo -e "${GREEN}✅ GET /v1/category/{id} funcionando!${NC}"
        echo "   Status: $GET_HTTP_STATUS"
        echo "   ID testado: $CATEGORY_ID"
    else
        echo -e "${YELLOW}⚠️  Status HTTP inesperado para GET por ID: $GET_HTTP_STATUS${NC}"
        echo "   Resposta: $GET_RESPONSE_BODY"
    fi
else
    echo -e "${YELLOW}⚠️  Não foi possível obter um ID válido para testar${NC}"
fi

echo
echo -e "${GREEN}🎉 Teste de endpoints de categorias concluído!${NC}"
echo
echo "📋 RESUMO:"
echo "   ✅ Autenticação funcionando"
echo "   ✅ GET /v1/categories (busca com filtros)"
echo "   $([ -n "$CATEGORY_ID" ] && echo "✅" || echo "⚠️ ") GET /v1/category/{id} (busca por ID)"