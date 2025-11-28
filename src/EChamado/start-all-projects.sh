#!/bin/bash

# Script para iniciar todos os projetos EChamado
echo "Iniciando projetos EChamado..."

# Matar processos existentes
echo "Finalizando processos existentes..."
pkill -f "EChamado.Server"
pkill -f "Echamado.Auth"  
pkill -f "EChamado.Client"

# Aguardar um pouco
sleep 2

# Iniciar Auth Server (porta 7132)
echo "Iniciando Echamado.Auth na porta 7132..."
cd Echamado.Auth
dotnet run --urls="https://localhost:7132;http://localhost:5136" &
cd ..

# Aguardar Auth inicializar
sleep 5

# Iniciar API Server (porta 7296)
echo "Iniciando EChamado.Server na porta 7296..."
cd Server/EChamado.Server
dotnet run --urls="https://localhost:7296;http://localhost:5295" &
cd ../..

# Aguardar API inicializar
sleep 5

# Iniciar Client (porta 7274)
echo "Iniciando EChamado.Client na porta 7274..."
cd Client/EChamado.Client
dotnet run --urls="https://localhost:7274;http://localhost:5199" &
cd ../..

echo "Todos os projetos foram iniciados!"
echo "Auth: https://localhost:7132"
echo "API: https://localhost:7296" 
echo "Client: https://localhost:7274"