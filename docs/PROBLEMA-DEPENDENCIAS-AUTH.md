# Problema: Dependências Faltando no Echamado.Auth

## Resumo do Problema

Quando o `Echamado.Auth` foi configurado para usar `EChamado.Server.Application` (para registrar `IOpenIddictService`), ele puxou TODAS as dependências da camada de aplicação, que por sua vez dependem de:

1. **Infrastructure Layer** - Repositories, UnitOfWork
2. **Redis/Cache** - IDistributedCache
3. **MessageBus** - IMessageBusClient
4. **User Services** - IUserReadRepository

## Erro Original

```
System.DirectoryNotFoundException: E:\mnt\e\TI\git\e-chamado\src\EChamado\Echamado.Auth\wwwroot\
```

Isso era apenas um sintoma. O problema real:

```
System.InvalidOperationException: Unable to resolve service for type:
- 'EChamado.Server.Application.Users.Abstractions.IUserReadRepository'
- 'EChamado.Server.Domain.Repositories.IUnitOfWork'
- 'EChamado.Server.Domain.Services.Interface.IMessageBusClient'
- 'Microsoft.Extensions.Caching.Distributed.IDistributedCache'
```

## Soluções Possíveis

### Opção 1: Configurar TODAS as Dependências (Complexo)

Adicionar ao `Echamado.Auth/Program.cs`:

```csharp
// Redis/Cache
builder.Services.AddMemoryCache();
builder.Services.AddDistributedMemoryCache();

// MessageBus (Null implementation como fallback)
builder.Services.AddScoped<IMessageBusClient, NullMessageBusClient>();

// User Read Repository
builder.Services.AddScoped<IUserReadRepository, EfUserReadRepository>();

// Application Services
builder.Services.AddApplicationServices();
builder.Services.ResolveDependenciesApplication();

// Infrastructure Services
builder.Services.ResolveDependenciesInfrastructure();
```

**Problema:** O `Echamado.Auth` fica muito pesado, replicando toda a configuração do `EChamado.Server`.

### Opção 2: Mover o `AuthorizationController` de Volta (Recomendado)

O `AuthorizationController` deveria estar apenas no `Echamado.Auth`, e o `EChamado.Server` NÃO precisa dele (é apenas Resource Server).

**Mas já fizemos isso!** O problema é que o `IOpenIddictService` está na camada de Application e traz muitas dependências.

### Opção 3: Simplificar o `IOpenIddictService` ⭐ MELHOR SOLUÇÃO

Criar uma implementação leve do `IOpenIddictService` diretamente no `Echamado.Auth`, sem depender de `EChamado.Server.Application`.

## ✅ Solução Implementada (Temporária)

Adicionei as dependências mínimas necessárias no `Echamado.Auth/Program.cs`:

```csharp
builder.Services.AddMemoryCache();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddScoped<IMessageBusClient, NullMessageBusClient>();
builder.Services.AddScoped<IUserReadRepository, EfUserReadRepository>();
builder.Services.AddApplicationServices();
builder.Services.ResolveDependenciesApplication();
builder.Services.ResolveDependenciesInfrastructure();
```

## Como Testar Agora

```bash
cd src/EChamado/Echamado.Auth
dotnet run
```

**Aguarde os logs:**
```
✅ Database ready for OpenIddict
✅ Scope 'api' registered
✅ Scope 'chamados' registered
✅ Client 'bwa-client' created
✅ Client 'mobile-client' created
✅ OpenIddict clients and scopes configured successfully

Now listening on: https://localhost:7132
```

**Teste a autenticação:**
```bash
curl -k -X POST https://localhost:7132/connect/token \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=password" \
  -d "username=admin@admin.com" \
  -d "password=Admin@123" \
  -d "client_id=mobile-client" \
  -d "scope=openid profile email roles api chamados"
```

## Arquitetura Recomendada (Futura Refatoração)

```
┌─────────────────────────────────────────────────┐
│ Echamado.Auth - Lite (Authorization Server)    │
│ ✅ OpenIddict Server                            │
│ ✅ AuthorizationController                      │
│ ✅ OpenIddictService (implementação leve)       │
│ ✅ Identity (UserManager, SignInManager)        │
│ ❌ NÃO precisa: Repositories, CQRS, MessageBus  │
└─────────────────────────────────────────────────┘
                          ↓ emite tokens
┌─────────────────────────────────────────────────┐
│ EChamado.Server (Resource Server)               │
│ ✅ OpenIddict Validation                        │
│ ✅ API Endpoints                                │
│ ✅ CQRS (Brighter)                              │
│ ✅ Repositories, MessageBus                     │
│ ❌ NÃO tem: AuthorizationController             │
└─────────────────────────────────────────────────┘
```

## Próximos Passos (Opcional - Refatoração)

1. **Criar `AuthService` leve no `Echamado.Auth`**:
   - Implementar diretamente com `UserManager` e `SignInManager`
   - Não depender de `EChamado.Server.Application`

2. **Remover referência ao `EChamado.Server.Application`** do `Echamado.Auth`

3. **Manter apenas**:
   - `EChamado.Server.Domain` (para entidades Identity)
   - `EChamado.Server.Infrastructure` (apenas para DbContext e Identity)

## Documentos Relacionados

- `TESTE-RAPIDO-AUTH.md` - Guia de teste rápido
- `docs/ARQUITETURA-AUTENTICACAO.md` - Arquitetura completa
- `CORRECAO-IOPENIDDICTSERVICE.md` - Correção do IOpenIddictService

## Status Atual

✅ Compilando sem erros
⚠️ Aguardando teste de execução
📝 Recomendado: Refatorar para simplificar dependências
