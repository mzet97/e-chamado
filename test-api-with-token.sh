#!/bin/bash

echo "=========================================="
echo "Teste Completo: Autenticação + API"
echo "=========================================="

# Cores
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo ""
echo "${YELLOW}[1/4] Obtendo token do Auth Server (porta 7132)...${NC}"
echo "----------------------------------------------"

TOKEN_RESPONSE=$(curl -k -s -X POST https://localhost:7133/connect/token \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=password" \
  -d "username=admin@admin.com" \
  -d "password=Admin@123" \
  -d "client_id=mobile-client" \
  -d "scope=openid profile email roles api chamados")

# Verificar se obteve token
if echo "$TOKEN_RESPONSE" | grep -q "access_token"; then
    echo "${GREEN}✅ Token obtido com sucesso${NC}"
    ACCESS_TOKEN=$(echo "$TOKEN_RESPONSE" | grep -o '"access_token":"[^"]*' | cut -d'"' -f4)

    echo ""
    echo "${YELLOW}[2/4] Verificando token (primeiros 50 caracteres)...${NC}"
    echo "----------------------------------------------"
    echo "Token: ${ACCESS_TOKEN:0:50}..."

    # Decode JWT header (primeiro parte antes do primeiro ponto)
    HEADER=$(echo "$ACCESS_TOKEN" | cut -d'.' -f1)
    # Base64 decode (pode não funcionar perfeitamente mas dá uma ideia)
    echo ""
    echo "Token Header (base64 decoded - pode ter caracteres estranhos):"
    echo "$HEADER" | base64 -d 2>/dev/null | head -c 200
    echo ""

    echo ""
    echo "${YELLOW}[3/4] Testando API sem autenticação (deve retornar 401)...${NC}"
    echo "----------------------------------------------"
    HTTP_CODE=$(curl -k -s -o /dev/null -w "%{http_code}" -X POST https://localhost:7296/v1/category \
      -H "Content-Type: application/json" \
      -d '{"name": "Teste Sem Auth", "description": "Teste"}')

    if [ "$HTTP_CODE" = "401" ]; then
        echo "${GREEN}✅ Retornou 401 Unauthorized (correto)${NC}"
    else
        echo "${RED}❌ Retornou HTTP $HTTP_CODE (esperado: 401)${NC}"
    fi

    echo ""
    echo "${YELLOW}[4/4] Testando API COM token Bearer...${NC}"
    echo "----------------------------------------------"

    API_RESPONSE=$(curl -k -s -w "\nHTTP_CODE:%{http_code}" -X POST https://localhost:7296/v1/category \
      -H "Content-Type: application/json" \
      -H "Authorization: Bearer $ACCESS_TOKEN" \
      -d '{"name": "Teste Com Token", "description": "Teste de autenticação"}')

    # Extrair HTTP code
    HTTP_CODE=$(echo "$API_RESPONSE" | grep "HTTP_CODE:" | cut -d: -f2)
    BODY=$(echo "$API_RESPONSE" | sed '/HTTP_CODE:/d')

    echo "HTTP Status: $HTTP_CODE"
    echo "Response Body:"
    echo "$BODY" | head -20

    if [ "$HTTP_CODE" = "200" ] || [ "$HTTP_CODE" = "201" ]; then
        echo ""
        echo "${GREEN}=========================================="
        echo "✅ SUCESSO! API funcionando com token"
        echo "==========================================${NC}"
    elif [ "$HTTP_CODE" = "401" ]; then
        echo ""
        echo "${RED}=========================================="
        echo "❌ ERRO 401: Token rejeitado"
        echo "==========================================${NC}"
        echo ""
        echo "Possíveis causas:"
        echo "1. EChamado.Server não foi reconstruído após mudança no IdentityConfig.cs"
        echo "2. Issuer não corresponde (deve ser: https://localhost:7133)"
        echo "3. Chaves de assinatura não correspondem"
        echo "4. Token expirou"
        echo ""
        echo "Solução:"
        echo "  cd /mnt/e/TI/git/e-chamado/src/EChamado/Server/EChamado.Server"
        echo "  dotnet clean && dotnet build && dotnet run"
    else
        echo ""
        echo "${RED}=========================================="
        echo "❌ ERRO HTTP $HTTP_CODE"
        echo "==========================================${NC}"
    fi

else
    echo "${RED}❌ Erro ao obter token${NC}"
    echo "Resposta:"
    echo "$TOKEN_RESPONSE"
    echo ""
    echo "Verifique se o Echamado.Auth está rodando em https://localhost:7133"
fi

echo ""
