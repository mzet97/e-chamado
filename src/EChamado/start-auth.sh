#!/bin/bash
# Inicia o Auth Server em background (com wait pelo Postgres)
cd /mnt/d/TI/git/e-chamado/src/EChamado/Echamado.Auth

# Aguarda Postgres estar pronto
echo "Aguardando Postgres 5433..."
for i in $(seq 1 30); do
  if timeout 2 bash -c '</dev/tcp/127.0.0.1/5433' 2>/dev/null; then
    echo "Postgres acessivel apos ${i}s"
    break
  fi
  sleep 1
done

ASPNETCORE_ENVIRONMENT=Development dotnet run -c Debug --no-build --urls "https://localhost:7133;http://localhost:5137" > /tmp/auth-server.log 2>&1 &
AUTH_PID=$!
echo "Auth Server iniciado com PID: $AUTH_PID"
echo $AUTH_PID > /tmp/auth-server.pid

echo "Aguardando 30s para startup..."
sleep 30
echo "=== LOG DO AUTH SERVER (ultimas linhas) ==="
tail -25 /tmp/auth-server.log
echo "=== TESTE /connect/authorize reachable? ==="
curl -sk -o /dev/null -w "HTTP %{http_code}\n" https://localhost:7133/connect/authorize 2>&1 || echo "curl falhou"
