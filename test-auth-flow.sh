#!/bin/bash

# Script de Teste - Fluxo de Autenticação OpenIddict
# Versão: 1.0
# Data: 2025-11-12

echo "=========================================="
echo "🔐 Teste de Fluxo de Autenticação OpenIddict"
echo "=========================================="
echo ""

# Cores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Verifica se os processos estão rodando
check_process() {
    local port=$1
    local service_name=$2

    if lsof -Pi :$port -sTCP:LISTEN -t >/dev/null 2>&1; then
        echo -e "${GREEN}✅ $service_name está rodando na porta $port${NC}"
        return 0
    else
        echo -e "${RED}❌ $service_name NÃO está rodando na porta $port${NC}"
        return 1
    fi
}

echo "📍 Verificando se os serviços estão rodando..."
echo ""

AUTH_OK=0
API_OK=0
CLIENT_OK=0

check_process 7132 "Auth Server (Echamado.Auth)" && AUTH_OK=1
check_process 7296 "API Server (EChamado.Server)" && API_OK=1
check_process 7274 "Cliente Blazor (EChamado.Client)" && CLIENT_OK=1

echo ""

if [ $AUTH_OK -eq 0 ] || [ $API_OK -eq 0 ] || [ $CLIENT_OK -eq 0 ]; then
    echo -e "${YELLOW}⚠️  Nem todos os serviços estão rodando!${NC}"
    echo ""
    echo "Para iniciar os serviços, abra 3 terminais:"
    echo ""
    echo "Terminal 1 - Auth Server:"
    echo "  cd src/EChamado/Echamado.Auth"
    echo "  dotnet run"
    echo ""
    echo "Terminal 2 - API Server:"
    echo "  cd src/EChamado/Server/EChamado.Server"
    echo "  dotnet run"
    echo ""
    echo "Terminal 3 - Cliente Blazor:"
    echo "  cd src/EChamado/Client/EChamado.Client"
    echo "  dotnet run"
    echo ""
    exit 1
fi

echo -e "${GREEN}✅ Todos os serviços estão rodando!${NC}"
echo ""
echo "=========================================="
echo "🧪 Testes Disponíveis"
echo "=========================================="
echo ""
echo "1. Teste Básico de Health"
echo "2. Teste de Redirecionamento para Login"
echo "3. Teste de Discovery Document (OpenIddict)"
echo "4. Teste de Login (Interativo)"
echo "5. Monitorar Logs em Tempo Real"
echo ""
read -p "Escolha um teste (1-5): " choice

case $choice in
    1)
        echo ""
        echo "🏥 Testando Health Check da API..."
        curl -s https://localhost:7296/health | jq '.' || echo "❌ Erro ao acessar /health"
        ;;
    2)
        echo ""
        echo "🔄 Testando Redirecionamento para /connect/authorize..."
        echo ""
        echo "Acessando: https://localhost:7296/connect/authorize?client_id=bwa-client&response_type=code&redirect_uri=https://localhost:7274/authentication/login-callback&scope=openid"
        echo ""
        curl -L -k -v "https://localhost:7296/connect/authorize?client_id=bwa-client&response_type=code&redirect_uri=https://localhost:7274/authentication/login-callback&scope=openid" 2>&1 | grep -i "location:"
        ;;
    3)
        echo ""
        echo "🔍 Testando Discovery Document..."
        curl -s -k https://localhost:7296/.well-known/openid-configuration | jq '.'
        ;;
    4)
        echo ""
        echo "🔐 Teste de Login Interativo"
        echo ""
        echo "1. Abra o navegador em: https://localhost:7274"
        echo "2. Clique em 'Login'"
        echo "3. Observe os logs nos terminais do Auth Server e API Server"
        echo "4. Use as credenciais:"
        echo "   Email: admin@echamado.com"
        echo "   Senha: Admin@123"
        echo ""
        echo "Logs esperados:"
        echo ""
        echo "📋 Auth Server (Terminal 1):"
        echo "  - 'Login attempt for admin@echamado.com with returnUrl: ...'"
        echo "  - 'Decoded returnUrl: https://localhost:7296/connect/authorize?...'"
        echo "  - 'Redirecting to valid returnUrl: ...'"
        echo ""
        echo "📋 API Server (Terminal 2):"
        echo "  - 'OnRedirectToLogin: Original RedirectUri=...'"
        echo "  - 'Authorization request received. Client: bwa-client...'"
        echo "  - 'User authenticated via External cookie. UserId: ...'"
        echo ""
        read -p "Pressione Enter para abrir o navegador..."
        if command -v xdg-open > /dev/null; then
            xdg-open "https://localhost:7274"
        elif command -v open > /dev/null; then
            open "https://localhost:7274"
        else
            echo "Abra manualmente: https://localhost:7274"
        fi
        ;;
    5)
        echo ""
        echo "📊 Monitorando Logs (Ctrl+C para sair)..."
        echo ""
        echo "Aguardando logs do Auth Server e API Server..."
        echo ""
        # Nota: Este comando assume que os logs estão sendo gravados em arquivos
        # Ajuste conforme necessário
        tail -f /tmp/echamado-*.log 2>/dev/null || echo "❌ Arquivos de log não encontrados em /tmp/echamado-*.log"
        ;;
    *)
        echo ""
        echo "❌ Opção inválida!"
        exit 1
        ;;
esac

echo ""
echo "=========================================="
echo "✅ Teste Concluído"
echo "=========================================="
echo ""
echo "📚 Para mais informações, consulte:"
echo "   - CORRECAO-REDIRECT-OPENIDDICT.md"
echo "   - SSO-SETUP.md"
echo ""
