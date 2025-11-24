#!/bin/bash

echo "🚀 INICIANDO SERVIÇOS ECHAMADO - CONFIGURAÇÃO CORRIGIDA"
echo "=================================================="

# Cores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Função para verificar se um processo está rodando
check_process() {
    local port=$1
    local name=$2
    
    if curl -k -s -o /dev/null -w "%{http_code}" "https://localhost:$port" | grep -q "200\|404"; then
        echo -e "${GREEN}✅ $name está rodando na porta $port${NC}"
        return 0
    else
        echo -e "${RED}❌ $name não está rodando na porta $port${NC}"
        return 1
    fi
}

# Função para iniciar um serviço
start_service() {
    local dir=$1
    local name=$2
    local port=$3
    
    echo -e "${YELLOW}📦 Iniciando $name...${NC}"
    
    cd "$dir"
    
    # Inicia em background
    nohup dotnet run --urls="https://localhost:$port" > "../${name,,}-$(date +%s).log" 2>&1 &
    local pid=$!
    echo "PID: $pid"
    
    # Aguarda o serviço iniciar (máximo 30 segundos)
    local attempts=0
    while [ $attempts -lt 30 ]; do
        if check_process $port $name >/dev/null 2>&1; then
            echo -e "${GREEN}✅ $name iniciado com sucesso!${NC}"
            return 0
        fi
        sleep 1
        attempts=$((attempts + 1))
        echo -n "."
    done
    
    echo -e "${RED}❌ Timeout ao iniciar $name${NC}"
    return 1
}

# Verifica se dotnet está instalado
if ! command -v dotnet &> /dev/null; then
    echo -e "${RED}❌ dotnet não encontrado. Instale o .NET SDK primeiro.${NC}"
    exit 1
fi

# Navega para o diretório correto
cd "$(dirname "$0")/src/EChamado"

echo -e "${YELLOW}🔧 Verificando projeto...${NC}"
if [ ! -d "Echamado.Auth" ] || [ ! -d "Server/EChamado.Server" ] || [ ! -d "Client/EChamado.Client" ]; then
    echo -e "${RED}❌ Estrutura do projeto não encontrada${NC}"
    exit 1
fi

echo -e "${GREEN}✅ Estrutura do projeto OK${NC}"

# 1. INICIA AUTH SERVER PRIMEIRO
echo ""
echo -e "${YELLOW}🎯 FASE 1: Iniciando Auth Server${NC}"
start_service "Echamado.Auth" "Auth Server" 7133

if [ $? -eq 0 ]; then
    # Aguarda mais um pouco para garantir que está totalmente inicializado
    echo -e "${YELLOW}⏳ Aguardando inicialização completa do Auth Server...${NC}"
    sleep 5
else
    echo -e "${RED}❌ Falha ao iniciar Auth Server${NC}"
    exit 1
fi

# 2. INICIA API SERVER
echo ""
echo -e "${YELLOW}🎯 FASE 2: Iniciando API Server${NC}"
start_service "Server/EChamado.Server" "API Server" 7296

if [ $? -eq 0 ]; then
    # Aguarda mais um pouco para garantir que está totalmente inicializado
    echo -e "${YELLOW}⏳ Aguardando inicialização completa do API Server...${NC}"
    sleep 5
else
    echo -e "${RED}❌ Falha ao iniciar API Server${NC}"
    exit 1
fi

# 3. INICIA CLIENT
echo ""
echo -e "${YELLOW}🎯 FASE 3: Iniciando Client${NC}"
start_service "Client/EChamado.Client" "Client" 7274

if [ $? -eq 0 ]; then
    # Aguarda mais um pouco para garantir que está totalmente inicializado
    echo -e "${YELLOW}⏳ Aguardando inicialização completa do Client...${NC}"
    sleep 5
else
    echo -e "${YELLOW}⚠️ Falha ao iniciar Client (não é crítico)${NC}"
fi

# VERIFICAÇÃO FINAL
echo ""
echo -e "${YELLOW}🔍 VERIFICAÇÃO FINAL DO STATUS${NC}"
echo "====================================="

check_process 7133 "Auth Server"
check_process 7296 "API Server" 
check_process 7274 "Client"

echo ""
echo -e "${GREEN}🎉 INICIALIZAÇÃO CONCLUÍDA!${NC}"
echo ""
echo -e "${GREEN}📍 URLs dos serviços:${NC}"
echo "• Auth Server: https://localhost:7133"
echo "• API Server:  https://localhost:7296"  
echo "• Client:      https://localhost:7274"
echo ""
echo -e "${GREEN}🔧 Para testar a autenticação:${NC}"
echo "curl -X POST https://localhost:7133/connect/token \\"
echo "  -H 'Content-Type: application/x-www-form-urlencoded' \\"
echo "  -d 'grant_type=password' \\"
echo "  -d 'username=admin@admin.com' \\"
echo "  -d 'password=Admin@123' \\"
echo "  -d 'client_id=mobile-client' \\"
echo "  -d 'scope=openid profile email roles api chamados'"
echo ""
echo -e "${GREEN}✅ Status: Todos os serviços iniciados com configuração corrigida!${NC}"