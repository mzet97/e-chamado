# 🏥 Health Checks - EChamado

Documentação completa dos Health Checks implementados no sistema EChamado.

## 📋 Endpoints Disponíveis

### 1. `/health` - Health Check Completo
**Descrição**: Verifica o status de todos os serviços (PostgreSQL, Redis)

**Uso**:
```bash
curl http://localhost:8080/health
```

**Resposta**:
```json
{
  "status": "Healthy",
  "totalDuration": "00:00:00.1234567",
  "entries": {
    "postgresql": {
      "status": "Healthy",
      "duration": "00:00:00.0123456"
    },
    "redis": {
      "status": "Healthy",
      "duration": "00:00:00.0234567"
    }
  }
}
```

**Status Codes**:
- `200 OK` - Sistema saudável ou degradado
- `503 Service Unavailable` - Sistema não saudável

---

### 2. `/health/ready` - Readiness Probe
**Descrição**: Verifica se a aplicação está pronta para receber tráfego (Kubernetes Readiness Probe)

**Uso**:
```bash
curl http://localhost:8080/health/ready
```

**Uso no Kubernetes**:
```yaml
readinessProbe:
  httpGet:
    path: /health/ready
    port: 8080
  initialDelaySeconds: 10
  periodSeconds: 10
  timeoutSeconds: 5
  failureThreshold: 3
```

**Status Codes**:
- `200 OK` - Aplicação pronta
- `503 Service Unavailable` - Aplicação não pronta (dependências não disponíveis)

---

### 3. `/health/live` - Liveness Probe
**Descrição**: Verifica se a aplicação está viva (Kubernetes Liveness Probe)

**Uso**:
```bash
curl http://localhost:8080/health/live
```

**Uso no Kubernetes**:
```yaml
livenessProbe:
  httpGet:
    path: /health/live
    port: 8080
  initialDelaySeconds: 15
  periodSeconds: 20
  timeoutSeconds: 5
  failureThreshold: 3
```

**Status Codes**:
- `200 OK` - Aplicação está viva
- Não retorna resposta se a aplicação travou

---

### 4. `/health-ui` - Health Checks Dashboard
**Descrição**: Interface visual para monitorar o status dos serviços em tempo real

**Acesso**:
```
http://localhost:8080/health-ui
```

**Features**:
- Histórico de 50 verificações
- Atualização automática a cada 30 segundos
- Gráficos de disponibilidade
- Alertas visuais

---

## 🐳 Docker Health Checks

### Docker Compose com Health Checks

```bash
# Subir todos os serviços com health checks
docker-compose -f docker-compose.healthchecks.yml up -d

# Verificar status dos health checks
docker-compose ps

# Ver logs de health checks
docker-compose logs echamado-api
```

### Status dos Containers

```bash
# Ver status detalhado
docker ps --format "table {{.Names}}\t{{.Status}}"
```

**Exemplo de saída**:
```
NAMES                 STATUS
echamado-api          Up 2 minutes (healthy)
echamado-postgres     Up 2 minutes (healthy)
echamado-redis        Up 2 minutes (healthy)
echamado-elasticsearch Up 3 minutes (healthy)
```

---

## 🔍 Serviços Monitorados

### 1. PostgreSQL
- **Tipo**: Database
- **Health Check**: `pg_isready`
- **Intervalo**: 10s
- **Timeout**: 5s
- **Retries**: 5

### 2. Redis
- **Tipo**: Cache
- **Health Check**: `redis-cli ping`
- **Intervalo**: 10s
- **Timeout**: 3s
- **Retries**: 5

---

## 📊 Logging

### Request Logging
Todas as requisições HTTP são logadas com:
- Método HTTP
- Path
- Request ID único
- IP do cliente
- User Agent
- Status Code
- Duração (ms)

**Exemplo de log**:
```
[INFO] HTTP GET /api/orders started - RequestId: abc123, IP: 192.168.1.100
[INFO] HTTP GET /api/orders completed - RequestId: abc123, StatusCode: 200, Duration: 145ms
```

### Performance Logging
Requisições lentas (>3000ms) são logadas automaticamente:

```
[WARN] SLOW REQUEST detected - GET /api/orders took 3500ms (threshold: 3000ms) - StatusCode: 200
```

---

## ⚙️ Configuração

### Alterar Threshold de Requisição Lenta

No `Program.cs`:
```csharp
app.UsePerformanceLogging(slowRequestThresholdMs: 5000); // 5 segundos
```

### Health Checks Personalizados

Para adicionar novos health checks, edite `HealthCheckConfig.cs`:

```csharp
services.AddHealthChecks()
    .AddNpgSql(connectionString, name: "postgresql", tags: new[] { "db" })
    .AddRedis(redisConnection, name: "redis", tags: new[] { "cache" })
    .AddCheck<CustomHealthCheck>("custom", tags: new[] { "custom" });
```

---

## 🚀 Uso em Produção

### 1. Monitoramento com Prometheus
```yaml
# prometheus.yml
scrape_configs:
  - job_name: 'echamado-health'
    metrics_path: '/health'
    scrape_interval: 30s
    static_configs:
      - targets: ['echamado-api:8080']
```

### 2. Alertas
Configure alertas para:
- Health check failures
- Requisições lentas (>3000ms)
- Downtime de dependências

### 3. Load Balancer
Configure o Load Balancer para usar `/health/ready` como health check endpoint.

---

## 🧪 Testes

### Testar Health Checks Localmente

```bash
# Health check geral
curl http://localhost:8080/health | jq

# Readiness probe
curl http://localhost:8080/health/ready | jq

# Liveness probe
curl http://localhost:8080/health/live | jq

# Simular falha do PostgreSQL
docker stop echamado-postgres
curl http://localhost:8080/health
# Deve retornar 503 Service Unavailable

# Restaurar PostgreSQL
docker start echamado-postgres
```

---

## 📈 Métricas

Os health checks fornecem as seguintes métricas:
- **Uptime**: Tempo que cada serviço está saudável
- **Response Time**: Tempo de resposta de cada health check
- **Failure Rate**: Taxa de falhas dos health checks
- **Recovery Time**: Tempo para recuperação após falha

---

## 🔗 Referências

- [ASP.NET Core Health Checks](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks)
- [Kubernetes Probes](https://kubernetes.io/docs/tasks/configure-pod-container/configure-liveness-readiness-startup-probes/)
- [Docker Health Checks](https://docs.docker.com/engine/reference/builder/#healthcheck)

---

**Última atualização**: 2025-11-09
**Versão**: 1.0
**Autor**: Claude (Anthropic)
