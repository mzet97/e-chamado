# 🚀 EChamado - Configuração e Execução

## 📋 Pré-requisitos

- .NET 9.0 SDK
- Docker e Docker Compose
- PostgreSQL (via Docker)
- Redis (via Docker)
- Elasticsearch + Kibana (via Docker)

## 🔧 Configuração

### 1. **Configurar Banco de Dados**

```bash
# Subir os serviços de infraestrutura
docker-compose up -d postgres redis elasticsearch kibana logstash rabbitmq pgadmin
```

### 2. **Aplicar Migrações**

```bash
# No diretório do EChamado.Server
cd Server/EChamado.Server
dotnet ef database update
```

### 3. **Configurar Variáveis de Ambiente**

O arquivo `.env` já está configurado com as seguintes portas:

- **EChamado.Server**: `https://localhost:7296`
- **EChamado.Auth**: `https://localhost:7132`
- **EChamado.Client**: `https://localhost:7274`

## 🚀 Execução

### Ordem de Execução

1. **Iniciar EChamado.Server** (Servidor OpenIddict)

   ```bash
   cd Server/EChamado.Server
   dotnet run
   ```

2. **Iniciar EChamado.Auth** (UI de Autenticação)

   ```bash
   cd Echamado.Auth
   dotnet run
   ```

3. **Iniciar EChamado.Client** (Blazor WebAssembly)

   ```bash
   cd Client/EChamado.Client
   dotnet run
   ```

## 🔐 Fluxo de Autenticação

1. Usuario acessa `https://localhost:7274` (Client)
2. Client redireciona para `https://localhost:7132` (Auth UI)
3. Auth UI exibe login/registro
4. Após login, redireciona para `https://localhost:7296/connect/authorize` (Server)
5. Server emite código de autorização
6. Client troca código por tokens
7. Client usa tokens para chamar APIs

## 🔧 Configurações Importantes

### EChamado.Server (`appsettings.json`)

- Configuração do OpenIddict como Authorization Server
- Endpoints: `/connect/authorize`, `/connect/token`, `/connect/userinfo`
- Clientes configurados automaticamente via seeder

### EChamado.Auth (`appsettings.json`)

- UI de autenticação (Blazor Server)
- Redireciona para EChamado.Server após login

### EChamado.Client (`wwwroot/appsettings.json`)

- Configuração OIDC para Authorization Code + PKCE
- Authority: `https://localhost:7296`
- Client ID: `bwa-client`

## 📦 Serviços Docker

| Serviço | Porta | Usuário | Senha |
|---------|-------|---------|--------|
| PostgreSQL | 5432 | postgres | dsv@123 |
| PgAdmin | 15432 | <admin@echamado.com> | dsv@123 |
| Redis | 6379 | - | dsv@123 |
| Elasticsearch | 9200 | elastic | dsv@123 |
| Kibana | 5601 | - | - |
| RabbitMQ | 5672/15672 | admin | dsv@123 |

## 🐛 Troubleshooting

### Problema: "Client not found"

- Verifique se o EChamado.Server está rodando
- Confirme que as migrações foram aplicadas
- O seeder deve executar automaticamente

### Problema: "Authority not found"

- Verifique se o EChamado.Server está rodando na porta 7296
- Confirme a configuração no `appsettings.json` do Client

### Problema: "Redirect URI mismatch"

- Verifique se as URLs estão corretas no seeder
- Confirme que o Client está rodando na porta 7274

## 📚 Documentação

- [Fluxo de Autenticação](docs/auth_flow.md)
- [Guia do Desenvolvedor](docs/DEVELOPER_GUIDE.md)
