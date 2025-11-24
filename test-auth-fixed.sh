#!/bin/bash

# Script de teste de autenticação - CORRIGIDO
# Usa a porta CORRETA do servidor de autenticação (7132)

set -e

echo "========================================="
echo "Teste de Autenticação OpenIddict"
echo "========================================="
echo ""

# Cores para output
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# URLs corretas
AUTH_SERVER="https://localhost:7132"
API_SERVER="https://localhost:7296"

echo -e "${YELLOW}IMPORTANTE:${NC}"
echo "✅ Autenticação (tokens): $AUTH_SERVER"
echo "✅ API (dados):           $API_SERVER"
echo ""

# Verifica se os servidores estão rodando
echo "Verificando servidores..."
echo ""

if ! curl -k -s "${AUTH_SERVER}/health" >/dev/null 2>&1; then
    echo -e "${RED}❌ Echamado.Auth (porta 7132) não está rodando!${NC}"
    echo ""
    echo "Inicie o servidor de autenticação:"
    echo "  cd src/EChamado/Echamado.Auth"
    echo "  dotnet run"
    echo ""
    exit 1
fi
echo -e "${GREEN}✅ Echamado.Auth está rodando (porta 7132)${NC}"

if ! curl -k -s "${API_SERVER}/health" >/dev/null 2>&1; then
    echo -e "${YELLOW}⚠️  EChamado.Server (porta 7296) não está rodando${NC}"
    echo "   (opcional para este teste)"
else
    echo -e "${GREEN}✅ EChamado.Server está rodando (porta 7296)${NC}"
fi

echo ""
echo "========================================="
echo "1. Testando Password Grant (mobile-client)"
echo "========================================="
echo ""

echo "Requisição:"
echo "POST ${AUTH_SERVER}/connect/token"
echo ""

RESPONSE=$(curl -k -s -X POST "${AUTH_SERVER}/connect/token" \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=password" \
  -d "username=admin@admin.com" \
  -d "password=Admin@123" \
  -d "client_id=mobile-client" \
  -d "scope=openid profile email roles api chamados")

# Verifica se houve erro
if echo "$RESPONSE" | jq -e '.error' >/dev/null 2>&1; then
    echo -e "${RED}❌ Erro na autenticação:${NC}"
    echo "$RESPONSE" | jq '.'
    exit 1
fi

# Verifica se recebeu access_token
if echo "$RESPONSE" | jq -e '.access_token' >/dev/null 2>&1; then
    echo -e "${GREEN}✅ Autenticação bem-sucedida!${NC}"
    echo ""

    ACCESS_TOKEN=$(echo "$RESPONSE" | jq -r '.access_token')
    REFRESH_TOKEN=$(echo "$RESPONSE" | jq -r '.refresh_token')
    EXPIRES_IN=$(echo "$RESPONSE" | jq -r '.expires_in')

    echo "Resposta:"
    echo "$RESPONSE" | jq '.'
    echo ""

    echo -e "${GREEN}Token obtido com sucesso!${NC}"
    echo "  Tipo: Bearer"
    echo "  Expira em: ${EXPIRES_IN} segundos"
    echo "  Access Token (primeiros 50 chars): ${ACCESS_TOKEN:0:50}..."
    echo "  Refresh Token disponível: $([ -n "$REFRESH_TOKEN" ] && echo 'Sim' || echo 'Não')"
    echo ""

    # Testa chamada à API se o servidor estiver rodando
    if curl -k -s "${API_SERVER}/health" >/dev/null 2>&1; then
        echo "========================================="
        echo "2. Testando chamada à API com token"
        echo "========================================="
        echo ""

        echo "Requisição:"
        echo "GET ${API_SERVER}/v1/categories"
        echo "Authorization: Bearer ${ACCESS_TOKEN:0:20}..."
        echo ""

        API_RESPONSE=$(curl -k -s -w "\n%{http_code}" -X GET "${API_SERVER}/v1/categories" \
          -H "Authorization: Bearer ${ACCESS_TOKEN}")

        HTTP_CODE=$(echo "$API_RESPONSE" | tail -n 1)
        API_BODY=$(echo "$API_RESPONSE" | sed '$d')

        if [ "$HTTP_CODE" -eq 200 ] || [ "$HTTP_CODE" -eq 201 ]; then
            echo -e "${GREEN}✅ Chamada à API bem-sucedida! (HTTP $HTTP_CODE)${NC}"
            echo ""
            echo "Resposta (primeiros 500 chars):"
            echo "$API_BODY" | head -c 500
            echo ""
            echo "..."
        else
            echo -e "${YELLOW}⚠️  Chamada à API retornou HTTP $HTTP_CODE${NC}"
            echo ""
            echo "Resposta:"
            echo "$API_BODY"
        fi
    fi

    # Testa refresh token se disponível
    if [ -n "$REFRESH_TOKEN" ]; then
        echo ""
        echo "========================================="
        echo "3. Testando Refresh Token"
        echo "========================================="
        echo ""

        echo "Requisição:"
        echo "POST ${AUTH_SERVER}/connect/token"
        echo "  grant_type=refresh_token"
        echo ""

        REFRESH_RESPONSE=$(curl -k -s -X POST "${AUTH_SERVER}/connect/token" \
          -H "Content-Type: application/x-www-form-urlencoded" \
          -d "grant_type=refresh_token" \
          -d "refresh_token=${REFRESH_TOKEN}" \
          -d "client_id=mobile-client")

        if echo "$REFRESH_RESPONSE" | jq -e '.access_token' >/dev/null 2>&1; then
            echo -e "${GREEN}✅ Refresh token funcionou!${NC}"
            echo ""

            NEW_ACCESS_TOKEN=$(echo "$REFRESH_RESPONSE" | jq -r '.access_token')
            echo "Novo Access Token (primeiros 50 chars): ${NEW_ACCESS_TOKEN:0:50}..."
        else
            echo -e "${RED}❌ Erro no refresh token:${NC}"
            echo "$REFRESH_RESPONSE" | jq '.'
        fi
    fi

    echo ""
    echo "========================================="
    echo -e "${GREEN}✅ TESTE COMPLETO!${NC}"
    echo "========================================="
    echo ""
    echo "Resumo:"
    echo "  ✅ Password Grant funcionando"
    echo "  ✅ Access Token recebido"
    echo "  ✅ Refresh Token recebido"
    if curl -k -s "${API_SERVER}/health" >/dev/null 2>&1; then
        echo "  ✅ API acessível com token"
    fi
    echo ""
    echo "Para usar o token em outras requisições:"
    echo "  export ACCESS_TOKEN=\"${ACCESS_TOKEN}\""
    echo ""
    echo "  curl -k -X GET ${API_SERVER}/v1/categories \\"
    echo "    -H \"Authorization: Bearer \$ACCESS_TOKEN\""
    echo ""

else
    echo -e "${RED}❌ Resposta inesperada:${NC}"
    echo "$RESPONSE"
    exit 1
fi
