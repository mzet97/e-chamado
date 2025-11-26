#!/bin/bash

echo "🔍 ANÁLISE DETALHADA DO PROBLEMA DE AUTENTICAÇÃO"
echo "==============================================="
echo ""

# Cores
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

echo -e "${BLUE}📊 STATUS ATUAL DOS SERVIDORES:${NC}"
echo "✅ Auth Server: https://localhost:7133 (funcionando)"
echo "✅ Client Server: https://localhost:7274 (funcionando)"
echo ""

echo -e "${YELLOW}🎯 PROBLEMA IDENTIFICADO:${NC}"
echo "1. Login funciona (usuário autenticado no auth server)"
echo "2. Redirect para /authentication/login-callback funciona"
echo "3. MAS: LoginCallback.razor NÃO está executando"
echo "4. RESULTADO: Token não é salvo no localStorage"
echo ""

echo -e "${RED}❌ EVIDÊNCIA NOS LOGS:${NC}"
echo "- Nenhuma mensagem de debug do LoginCallback aparece"
echo "- Nenhuma mensagem de debug do AuthService.ExchangeCodeForTokenAsync aparece"
echo "- Apenas: '❌ NO TOKEN found in localStorage'"
echo ""

echo -e "${BLUE}🔍 POSSÍVEIS CAUSAS:${NC}"
echo ""
echo "1. ${RED}URL MISMATCH:${NC}"
echo "   Auth server configurado para: https://localhost:7274/authentication/login-callback"
echo "   Mas redirect pode estar usando http://localhost:7274"
echo ""

echo "2. ${RED}BWA-CLIENT CONFIGURATION:${NC}"
echo "   - Redirect URI pode não estar registrado corretamente"
echo "   - Grant type authorization_code pode não estar permitido"
echo ""

echo "3. ${RED}BLARZOR ROUTING:${NC}"
echo "   - /authentication/login-callback pode não estar registrado"
echo "   - Page pode não estar sendo renderizada"
echo ""

echo -e "${YELLOW}🧪 TESTE PARA IDENTIFICAR O PROBLEMA:${NC}"
echo ""

echo "1. Teste se o endpoint existe:"
curl -s -I http://localhost:7274/authentication/login-callback | head -3

echo ""
echo "2. Teste se há diferença entre HTTP e HTTPS:"
echo "   HTTP Redirect: http://localhost:7274/authentication/login-callback"  
echo "   HTTPS Redirect: https://localhost:7274/authentication/login-callback"

echo ""
echo -e "${GREEN}💡 PRÓXIMOS PASSOS:${NC}"
echo "1. Verificar configuração do bwa-client no auth server"
echo "2. Testar redirect URI diretamente no navegador"
echo "3. Verificar logs do browser para erros JavaScript"
echo "4. Testar se o LoginCallback.razor está sendo carregado"

echo ""
echo -e "${BLUE}🔧 COMANDOS PARA DEBUGGING:${NC}"
echo "# Abrir browser dev tools"
echo "# Navegar para http://localhost:7274/authentication/login-callback"
echo "# Verificar se a página carrega e mostra 'Processando login...'"
echo "# Verificar console para mensagens de debug"

echo ""
echo -e "${YELLOW}🎯 SOLUÇÃO PROVÁVEL:${NC}"
echo "O problema está provavelmente na configuração do bwa-client"
echo "Redirect URI pode estar incorreto ou não registrado" 

# Teste de URL direta
echo ""
echo -e "${BLUE}🧪 TESTANDO ENDPOINT DIRETAMENTE:${NC}"
echo "Testando se /authentication/login-callback existe..."

RESPONSE=$(curl -s -o /dev/null -w "%{http_code}" http://localhost:7274/authentication/login-callback)
echo "HTTP Status: $RESPONSE"

if [ "$RESPONSE" = "200" ]; then
    echo -e "${GREEN}✅ Endpoint existe e responde OK${NC}"
elif [ "$RESPONSE" = "404" ]; then
    echo -e "${RED}❌ Endpoint não encontrado (404)${NC}"
    echo "Isso pode explicar por que o LoginCallback não executa"
else
    echo -e "${YELLOW}⚠️  Status inesperado: $RESPONSE${NC}"
fi