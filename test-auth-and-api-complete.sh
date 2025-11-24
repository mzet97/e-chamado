#!/bin/bash

echo "========================================="
echo "🧪 TESTE COMPLETO: AUTH + API"
echo "========================================="
echo ""

GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

AUTH_URL="https://localhost:7133/connect/token"
API_URL="https://localhost:7296/v1/categories"

echo -e "${BLUE}📋 PASSO 1: Obtendo token de acesso${NC}"
echo "========================================="
echo "POST $AUTH_URL"
echo ""

RESPONSE=$(curl -k -s -X POST "$AUTH_URL" \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=password" \
  -d "username=admin@admin.com" \
  -d "password=Admin@123" \
  -d "client_id=mobile-client" \
  -d "scope=openid profile email roles api chamados")

# Verifica se houve erro
if echo "$RESPONSE" | jq -e '.error' >/dev/null 2>&1; then
    echo -e "${RED}❌ ERRO na autenticação:${NC}"
    echo "$RESPONSE" | jq '.'
    echo ""
    echo "Possíveis causas:"
    echo "1. Auth Server não está rodando em https://localhost:7133"
    echo "2. Banco de dados não foi limpo após as correções"
    echo "3. Usuário admin@admin.com não existe"
    echo ""
    echo "Solução:"
    echo "1. Reinicie o Auth Server"
    echo "2. Limpe o banco: TRUNCATE TABLE \"OpenIddictScopes\" CASCADE;"
    exit 1
fi

# Verifica se recebeu access_token
if ! echo "$RESPONSE" | jq -e '.access_token' >/dev/null 2>&1; then
    echo -e "${RED}❌ Resposta inesperada:${NC}"
    echo "$RESPONSE"
    exit 1
fi

ACCESS_TOKEN=$(echo "$RESPONSE" | jq -r '.access_token')
EXPIRES_IN=$(echo "$RESPONSE" | jq -r '.expires_in')

echo -e "${GREEN}✅ Token obtido com sucesso!${NC}"
echo "  Expira em: ${EXPIRES_IN} segundos"
echo "  Token (primeiros 80 chars): ${ACCESS_TOKEN:0:80}..."
echo ""

echo -e "${BLUE}📋 PASSO 2: Testando chamada à API${NC}"
echo "========================================="
echo "GET $API_URL?PageIndex=1&PageSize=1"
echo "Authorization: Bearer ${ACCESS_TOKEN:0:50}..."
echo ""

API_RESPONSE=$(curl -k -s -w "\n%{http_code}" -X GET "${API_URL}?PageIndex=1&PageSize=1" \
  -H "Authorization: Bearer ${ACCESS_TOKEN}")

HTTP_CODE=$(echo "$API_RESPONSE" | tail -n 1)
API_BODY=$(echo "$API_RESPONSE" | sed '$d')

if [ "$HTTP_CODE" -eq 200 ] || [ "$HTTP_CODE" -eq 201 ]; then
    echo -e "${GREEN}✅ Chamada à API bem-sucedida! (HTTP $HTTP_CODE)${NC}"
    echo ""
    echo "Resposta:"
    echo "$API_BODY" | jq '.' 2>/dev/null || echo "$API_BODY"
    echo ""
    echo "========================================="
    echo -e "${GREEN}🎉 SUCESSO COMPLETO!${NC}"
    echo "========================================="
    echo ""
    echo "✅ Autenticação funcionando"
    echo "✅ Token válido"
    echo "✅ API aceitando o token"
    echo "✅ Autorização funcionando"
    echo ""
elif [ "$HTTP_CODE" -eq 401 ]; then
    echo -e "${RED}❌ Não autorizado (HTTP 401)${NC}"
    echo ""
    echo "Possíveis causas:"
    echo "1. API Server não está configurado para validar tokens do Auth Server"
    echo "2. Token inválido ou expirado"
    echo "3. Problema na configuração do OpenIddict Validation"
    echo ""
    echo "Resposta da API:"
    echo "$API_BODY"
    exit 1
elif [ "$HTTP_CODE" -eq 500 ]; then
    echo -e "${RED}❌ Erro interno do servidor (HTTP 500)${NC}"
    echo ""
    echo "Causa provável:"
    echo "- Middleware de autorização não está configurado corretamente"
    echo "- UseAuthentication() ou UseAuthorization() faltando no Program.cs"
    echo ""
    echo "Resposta da API:"
    echo "$API_BODY"
    exit 1
else
    echo -e "${YELLOW}⚠️  Chamada à API retornou HTTP $HTTP_CODE${NC}"
    echo ""
    echo "Resposta:"
    echo "$API_BODY"
    exit 1
fi

echo "Para usar o token manualmente:"
echo "  export ACCESS_TOKEN=\"${ACCESS_TOKEN}\""
echo ""
echo "  curl -k -X GET \"$API_URL?PageIndex=1&PageSize=10\" \\"
echo "    -H \"Authorization: Bearer \$ACCESS_TOKEN\""
echo ""
