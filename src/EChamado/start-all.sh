#!/bin/bash

# Script para executar todos os projetos EChamado
# Execute este script do diretório raiz do projeto

echo "🚀 Iniciando EChamado..."
echo ""

# Verificar se o Docker está rodando
if ! docker info > /dev/null 2>&1; then
    echo "❌ Docker não está rodando. Por favor, inicie o Docker primeiro."
    exit 1
fi

# Subir os serviços de infraestrutura
echo "🐳 Subindo serviços de infraestrutura..."
docker-compose up -d postgres redis elasticsearch kibana logstash rabbitmq pgadmin

# Aguardar alguns segundos para os serviços subirem
echo "⏳ Aguardando serviços ficarem prontos..."
sleep 15

# Função para executar um projeto em background
run_project() {
    local project_path="$1"
    local project_name="$2"
    local port="$3"
    
    echo "🏃 Executando $project_name em $port..."
    cd "$project_path"
    dotnet run --urls "https://localhost:$port" > "../logs/$project_name.log" 2>&1 &
    echo $! > "../logs/$project_name.pid"
    cd - > /dev/null
}

# Criar diretório de logs
mkdir -p logs

# Executar os projetos na ordem correta
echo ""
echo "📋 Executando projetos..."

# 1. EChamado.Server (OpenIddict Server)
run_project "Server/EChamado.Server" "EChamado.Server" "7296"

# Aguardar o servidor subir
echo "⏳ Aguardando EChamado.Server inicializar..."
sleep 10

# 2. EChamado.Auth (UI de Autenticação)
run_project "Echamado.Auth" "EChamado.Auth" "7132"

# Aguardar o Auth subir
echo "⏳ Aguardando EChamado.Auth inicializar..."
sleep 8

# 3. EChamado.Client (Blazor WebAssembly)
run_project "Client/EChamado.Client" "EChamado.Client" "7274"

echo ""
echo "✅ Todos os projetos foram iniciados!"
echo ""
echo "🌐 URLs disponíveis:"
echo "   • Client (Blazor WASM): https://localhost:7274"
echo "   • Auth (UI Login):      https://localhost:7132"
echo "   • Server (API):         https://localhost:7296"
echo "   • Swagger:              https://localhost:7296/swagger"
echo ""
echo "📊 Serviços de infraestrutura:"
echo "   • PgAdmin:              http://localhost:15432"
echo "   • Kibana:               http://localhost:5601"
echo "   • RabbitMQ:             http://localhost:15672"
echo ""
echo "📜 Logs disponíveis em: ./logs/"
echo ""
echo "🛑 Para parar todos os serviços, execute: ./stop-all.sh"
