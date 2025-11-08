# 🔧 Exemplos e Configurações - EChamado

Esta pasta contém exemplos práticos, configurações e snippets de código para facilitar o desenvolvimento e implantação do sistema EChamado.

## 📁 Estrutura

### 🔐 Autenticação
- [**openiddict-config.json**](openiddict-config.json) - Configurações do OpenIddict
- [**client-registration.cs**](client-registration.cs) - Registro de clientes OAuth2
- [**token-validation.cs**](token-validation.cs) - Validação de tokens JWT

### 📨 Mensageria
- [**rabbitmq-queues.json**](rabbitmq-queues.json) - Definições de filas RabbitMQ
- [**message-handlers.cs**](message-handlers.cs) - Handlers de mensagens
- [**retry-policies.cs**](retry-policies.cs) - Políticas de retry

### 🐳 Docker e Infraestrutura
- [**docker-compose.production.yml**](docker-compose.production.yml) - Configuração para produção
- [**kubernetes/**](kubernetes/) - Manifests Kubernetes
- [**nginx.conf**](nginx.conf) - Configuração do proxy reverso

### 🔧 Configurações
- [**appsettings.examples/**](appsettings.examples/) - Exemplos de configuração por ambiente
- [**logging.json**](logging.json) - Configuração de logging estruturado
- [**monitoring.yml**](monitoring.yml) - Configuração de monitoramento

### 📊 Observabilidade
- [**prometheus.yml**](prometheus.yml) - Métricas Prometheus
- [**grafana-dashboards/**](grafana-dashboards/) - Dashboards Grafana
- [**elastic-templates/**](elastic-templates/) - Templates Elasticsearch

## 🚀 Como Usar

1. **Copie** os arquivos de exemplo para seu ambiente
2. **Adapte** as configurações conforme necessário
3. **Valide** as configurações antes de aplicar
4. **Teste** em ambiente de desenvolvimento primeiro

## ⚠️ Importante

- Nunca commite credenciais reais
- Use variáveis de ambiente para dados sensíveis
- Valide configurações antes de aplicar em produção
- Mantenha backups das configurações funcionais