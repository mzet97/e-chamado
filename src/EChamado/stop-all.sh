#!/bin/bash

# Script para parar todos os serviços EChamado

echo "🛑 Parando todos os serviços EChamado..."
echo ""

# Parar os processos .NET se existirem
if [ -f "logs/EChamado.Server.pid" ]; then
    echo "🔴 Parando EChamado.Server..."
    kill $(cat logs/EChamado.Server.pid) 2>/dev/null
    rm -f logs/EChamado.Server.pid
fi

if [ -f "logs/EChamado.Auth.pid" ]; then
    echo "🔴 Parando EChamado.Auth..."
    kill $(cat logs/EChamado.Auth.pid) 2>/dev/null
    rm -f logs/EChamado.Auth.pid
fi

if [ -f "logs/EChamado.Client.pid" ]; then
    echo "🔴 Parando EChamado.Client..."
    kill $(cat logs/EChamado.Client.pid) 2>/dev/null
    rm -f logs/EChamado.Client.pid
fi

# Parar containers Docker
echo "🐳 Parando containers Docker..."
docker-compose down

echo ""
echo "✅ Todos os serviços foram parados!"
