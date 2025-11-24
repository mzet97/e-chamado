#!/bin/bash

echo "🔧 Teste final dos endpoints de categorias..."
echo

# Cores
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m'

echo "📋 PASSO 1: Teste do endpoint sem autenticação..."
curl -k -w "HTTPSTATUS:%{http_code}" https://localhost:7296/v1/categories/test-simple 2>/dev/null | tail -1

echo
echo "📋 PASSO 2: Teste com token válido..."
TOKEN=$(curl -k -s -X POST https://localhost:7132/connect/token \
    -H "Content-Type: application/x-www-form-urlencoded" \
    -d "grant_type=password" \
    -d "username=admin@admin.com" \
    -d "password=Admin@123" \
    -d "client_id=mobile-client" \
    -d "scope=openid profile email roles api chamados" | sed 's/.*"access_token":"\([^"]*\)".*/\1/' 2>/dev/null)

echo "Token: ${TOKEN:0:30}..."
curl -k -w "HTTPSTATUS:%{http_code}" -H "Authorization: Bearer $TOKEN" https://localhost:7296/v1/categories/test-simple 2>/dev/null | tail -1

echo
echo "📋 PASSO 3: Teste endpoint com autenticação..."
echo "Tentando endpoint /v1/categories com token..."
timeout 10s curl -k -w "HTTPSTATUS:%{http_code}" -H "Authorization: Bearer $TOKEN" https://localhost:7296/v1/categories 2>/dev/null || echo "HTTPSTATUS:TIMEOUT"

echo
echo "📋 PASSO 4: Teste endpoint por ID..."
echo "Testando endpoint /v1/category/12345678-1234-1234-1234-123456789012..."
timeout 10s curl -k -w "HTTPSTATUS:%{http_code}" -H "Authorization: Bearer $TOKEN" https://localhost:7296/v1/category/12345678-1234-1234-1234-123456789012 2>/dev/null || echo "HTTPSTATUS:TIMEOUT"

echo
echo "🎉 Teste concluído!"