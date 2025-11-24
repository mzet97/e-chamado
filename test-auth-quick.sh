#!/bin/bash

echo "========================================="
echo "🧪 TESTE RÁPIDO DE AUTENTICAÇÃO"
echo "========================================="
echo ""

# Usa a porta que o usuário especificou (7133)
AUTH_URL="https://localhost:7133/connect/token"

echo "Testando autenticação em: $AUTH_URL"
echo ""
echo "Parâmetros:"
echo "  - grant_type: password"
echo "  - username: admin@admin.com"
echo "  - password: Admin@123"
echo "  - client_id: mobile-client"
echo "  - scope: openid profile email roles api chamados"
echo ""
echo "Resposta:"
echo "========================================="

curl -k -X POST "$AUTH_URL" \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=password" \
  -d "username=admin@admin.com" \
  -d "password=Admin@123" \
  -d "client_id=mobile-client" \
  -d "scope=openid profile email roles api chamados" \
  -w "\n\n" 2>&1 | jq . 2>/dev/null || curl -k -X POST "$AUTH_URL" \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=password" \
  -d "username=admin@admin.com" \
  -d "password=Admin@123" \
  -d "client_id=mobile-client" \
  -d "scope=openid profile email roles api chamados" \
  -w "\n\n" 2>&1

echo ""
echo "========================================="
echo "Se você viu um access_token acima, a autenticação funcionou! ✅"
echo "Se você viu 'invalid_scope', o servidor precisa ser reiniciado."
echo ""
