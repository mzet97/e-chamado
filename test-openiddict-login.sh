#!/bin/bash

# Script para testar autenticação OpenIddict
# Uso: ./test-openiddict-login.sh

echo "========================================="
echo "Teste de Autenticação OpenIddict"
echo "========================================="
echo ""

# Cores
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Configuração
AUTH_SERVER="https://localhost:7133"
API_SERVER="https://localhost:7296"
USERNAME="admin@admin.com"
PASSWORD="Admin@123"
CLIENT_ID="mobile-client"

echo -e "${YELLOW}1. Obtendo token de acesso...${NC}"
echo ""

# Fazer login
TOKEN_RESPONSE=$(curl -k -s -X POST "$AUTH_SERVER/connect/token" \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=password" \
  -d "username=$USERNAME" \
  -d "password=$PASSWORD" \
  -d "client_id=$CLIENT_ID" \
  -d "scope=openid profile email roles api chamados")

# Verificar se houve erro
if echo "$TOKEN_RESPONSE" | grep -q "error"; then
    echo -e "${RED}❌ Erro ao obter token:${NC}"
    echo "$TOKEN_RESPONSE" | jq '.'
    exit 1
fi

# Extrair access_token
ACCESS_TOKEN=$(echo "$TOKEN_RESPONSE" | jq -r '.access_token')
REFRESH_TOKEN=$(echo "$TOKEN_RESPONSE" | jq -r '.refresh_token')
EXPIRES_IN=$(echo "$TOKEN_RESPONSE" | jq -r '.expires_in')

if [ "$ACCESS_TOKEN" = "null" ] || [ -z "$ACCESS_TOKEN" ]; then
    echo -e "${RED}❌ Token não encontrado na resposta${NC}"
    echo "$TOKEN_RESPONSE"
    exit 1
fi

echo -e "${GREEN}✅ Token obtido com sucesso!${NC}"
echo ""
echo -e "${YELLOW}Access Token (primeiros 100 caracteres):${NC}"
echo "${ACCESS_TOKEN:0:100}..."
echo ""
echo -e "${YELLOW}Expira em:${NC} $EXPIRES_IN segundos"
echo ""

# Decodificar e mostrar payload do token
echo -e "${YELLOW}2. Payload do token:${NC}"
echo "$ACCESS_TOKEN" | cut -d'.' -f2 | base64 -d 2>/dev/null | jq '.' || echo "Não foi possível decodificar"
echo ""

# Testar chamada à API
echo -e "${YELLOW}3. Testando chamada à API (GET /v1/categories)...${NC}"
echo ""

API_RESPONSE=$(curl -k -s -X GET "$API_SERVER/v1/categories" \
  -H "Authorization: Bearer $ACCESS_TOKEN" \
  -H "Accept: application/json")

# Verificar se a API retornou sucesso
if echo "$API_RESPONSE" | grep -q "success"; then
    echo -e "${GREEN}✅ API respondeu com sucesso!${NC}"
    echo ""
    echo -e "${YELLOW}Resposta:${NC}"
    echo "$API_RESPONSE" | jq '.'
else
    echo -e "${RED}❌ Erro na chamada à API:${NC}"
    echo "$API_RESPONSE"
    exit 1
fi

echo ""
echo -e "${GREEN}=========================================${NC}"
echo -e "${GREEN}✅ Teste concluído com sucesso!${NC}"
echo -e "${GREEN}=========================================${NC}"
echo ""

# Salvar tokens em arquivo para uso posterior
cat > .tokens.json <<EOF
{
  "access_token": "$ACCESS_TOKEN",
  "refresh_token": "$REFRESH_TOKEN",
  "expires_in": $EXPIRES_IN,
  "obtained_at": "$(date -Iseconds)"
}
EOF

echo -e "${YELLOW}💾 Tokens salvos em .tokens.json${NC}"
echo ""

# Exemplo de uso do refresh token
if [ -n "$REFRESH_TOKEN" ] && [ "$REFRESH_TOKEN" != "null" ]; then
    echo -e "${YELLOW}4. Testando Refresh Token...${NC}"
    echo ""

    REFRESH_RESPONSE=$(curl -k -s -X POST "$AUTH_SERVER/connect/token" \
      -H "Content-Type: application/x-www-form-urlencoded" \
      -d "grant_type=refresh_token" \
      -d "refresh_token=$REFRESH_TOKEN" \
      -d "client_id=$CLIENT_ID")

    if echo "$REFRESH_RESPONSE" | grep -q "access_token"; then
        echo -e "${GREEN}✅ Refresh token funcionou!${NC}"
        NEW_ACCESS_TOKEN=$(echo "$REFRESH_RESPONSE" | jq -r '.access_token')
        echo "Novo access token (primeiros 100 caracteres): ${NEW_ACCESS_TOKEN:0:100}..."
    else
        echo -e "${RED}❌ Erro ao usar refresh token:${NC}"
        echo "$REFRESH_RESPONSE" | jq '.'
    fi
fi

echo ""
echo -e "${YELLOW}📝 Para usar o token em outros requests:${NC}"
echo ""
echo "  curl -k -X GET $API_SERVER/v1/categories \\"
echo "    -H \"Authorization: Bearer $ACCESS_TOKEN\""
echo ""
