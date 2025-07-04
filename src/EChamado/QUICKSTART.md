# 🚀 EChamado - Início Rápido

## ⚡ Execução Rápida

### Windows (PowerShell)

```powershell
# Executar todos os serviços
.\start-all.ps1

# Parar todos os serviços
.\stop-all.ps1
```

### Linux/macOS (Bash)

```bash
# Executar todos os serviços
./start-all.sh

# Parar todos os serviços
./stop-all.sh
```

### Manual (Windows)

```powershell
# 1. Subir infraestrutura
docker-compose up -d

# 2. Executar projetos (em terminais separados)
# Terminal 1 - Server
cd Server\EChamado.Server
dotnet run

# Terminal 2 - Auth
cd Echamado.Auth
dotnet run

# Terminal 3 - Client
cd Client\EChamado.Client
dotnet run
```

## 🌐 URLs Importantes

| Serviço | URL | Descrição |
|---------|-----|-----------|
| **Client** | <https://localhost:7274> | Interface principal (Blazor WASM) |
| **Auth** | <https://localhost:7132> | UI de login/registro |
| **Server** | <https://localhost:7296> | API e OpenIddict |
| **Swagger** | <https://localhost:7296/swagger> | Documentação da API |

## 📊 Infraestrutura

| Serviço | URL | Credenciais |
|---------|-----|-------------|
| **PgAdmin** | <http://localhost:15432> | <admin@echamado.com> / dsv@123 |
| **Kibana** | <http://localhost:5601> | - |
| **RabbitMQ** | <http://localhost:15672> | admin / dsv@123 |

## 🔐 Fluxo de Autenticação

1. **Usuário acessa**: `https://localhost:7274`
2. **Redirecionamento**: `https://localhost:7132` (Auth UI)
3. **Login/Registro**: Interface amigável
4. **Autorização**: `https://localhost:7296/connect/authorize`
5. **Token Exchange**: Client recebe tokens JWT
6. **Acesso às APIs**: Usando Bearer token

## 🛠️ Comandos Úteis

```bash
# Verificar status dos containers
docker-compose ps

# Ver logs dos containers
docker-compose logs -f

# Aplicar migrações
cd Server/EChamado.Server
dotnet ef database update

# Limpar e reconstruir
dotnet clean
dotnet build

# Executar testes
dotnet test
```

## 🐛 Troubleshooting

### Problema: Porta em uso

```bash
# Verificar processo usando a porta
netstat -ano | findstr :7296
# Ou no Linux/macOS
lsof -i :7296

# Parar processo
taskkill /PID <PID> /F
# Ou no Linux/macOS
kill -9 <PID>
```

### Problema: Docker não responde

```bash
# Parar todos os containers
docker-compose down

# Limpar volumes (cuidado: perde dados!)
docker-compose down -v

# Reconstruir containers
docker-compose up -d --build
```

### Problema: Erro de certificado HTTPS

```bash
# Limpar certificados de desenvolvimento
dotnet dev-certs https --clean

# Recriar certificados
dotnet dev-certs https --trust
```

## 📝 Desenvolvimento

1. **Sempre inicie o Server primeiro** - Outros serviços dependem dele
2. **Use HTTPS** - Configuração obrigatória para OpenIddict
3. **Verifique os logs** - Disponíveis em `./logs/` quando usar scripts
4. **Migrations** - Aplicar sempre que houver mudanças no banco
5. **Rebuild** - Após mudanças em projetos compartilhados

## 🔧 Configuração Personalizada

Para alterar portas ou URLs, edite:

- `Properties/launchSettings.json` (cada projeto)
- `appsettings.json` (configurações)
- `wwwroot/appsettings.json` (Client)
- `.env` (variáveis de ambiente)

## 📚 Documentação Completa

- [README.md](README.md) - Documentação detalhada
- [docs/auth_flow.md](docs/auth_flow.md) - Fluxo de autenticação
- [docs/DEVELOPER_GUIDE.md](docs/DEVELOPER_GUIDE.md) - Guia do desenvolvedor
