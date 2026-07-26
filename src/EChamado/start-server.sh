#!/bin/bash
# Inicia o API Server em background (com wait pelo Postgres)
cd /mnt/d/TI/git/e-chamado/src/EChamado/Server/EChamado.Server

# Aguarda Postgres estar pronto
echo "Aguardando Postgres 5433..."
for i in $(seq 1 30); do
  if timeout 2 bash -c '</dev/tcp/127.0.0.1/5433' 2>/dev/null; then
    echo "Postgres acessivel apos ${i}s"
    break
  fi
  sleep 1
done

ASPNETCORE_ENVIRONMENT=Development \
SEED_ADMIN_PASSWORD="Admin@123456" \
SEED_TEST_PASSWORD="User@1234567" \
dotnet run -c Debug --no-build --urls "https://localhost:7296;http://localhost:5071" > /mnt/d/TI/git/e-chamado/src/EChamado/api-server.log 2>&1 &
SERVER_PID=$!
echo "API Server iniciado com PID: $SERVER_PID"
echo $SERVER_PID > /tmp/api-server.pid

echo "Aguardando 35s para startup + seed..."
sleep 35
echo "=== LOG DO API SERVER (ultimas linhas) ==="
tail -30 /tmp/api-server.log
echo "=== TESTE /health ==="
curl -sk -o /dev/null -w "HTTP %{http_code}\n" https://localhost:7296/health 2>&1 || echo "curl falhou"
