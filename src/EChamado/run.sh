#!/bin/bash

# Nome da network do Docker
NETWORK_NAME="echamado-network"

# Caminho base para os volumes persistentes
BASE_DIR="/dados"

# Lista de diretórios a serem criados
DIRECTORIES=(
  "$BASE_DIR/elasticsearch"
  "$BASE_DIR/logstash/logs"
  "$BASE_DIR/logstash/data"
  "$BASE_DIR/redis/logs"
  "$BASE_DIR/redis"
  "$BASE_DIR/postgres"
  "$BASE_DIR/pgadmin"
)

# Criar diretórios
echo "Criando diretórios necessários..."
for DIR in "${DIRECTORIES[@]}"; do
  if [ ! -d "$DIR" ]; then
    sudo mkdir -p "$DIR"
    echo "Criado: $DIR"
  else
    echo "Diretório já existe: $DIR"
  fi
done

# Configurar permissões
echo "Configurando permissões para os diretórios..."
sudo chmod -R 777 "$BASE_DIR"
echo "Permissões configuradas em $BASE_DIR."

# Criar network do Docker
echo "Verificando se a network '$NETWORK_NAME' já existe..."
if [ ! "$(docker network ls --filter name=^${NETWORK_NAME}$ --format '{{.Name}}')" ]; then
  docker network create "$NETWORK_NAME"
  echo "Network '$NETWORK_NAME' criada com sucesso."
else
  echo "Network '$NETWORK_NAME' já existe."
fi

# Rodar docker compose
echo "Iniciando os contêineres com docker compose..."
docker compose up -d

echo "Configuração concluída com sucesso!"
