# Guia de Uso - Scalar API Documentation

## Visão Geral

A EChamado API agora utiliza **Scalar** como interface de documentação, substituindo o Swagger UI tradicional. O Scalar oferece uma experiência moderna, interativa e intuitiva para explorar e testar a API.

## Acesso à Documentação

### URLs

- **Documentação Scalar**: https://localhost:7296/api-docs/v1
- **Especificação OpenAPI**: https://localhost:7296/openapi/v1.json
- **API Base**: https://localhost:7296

**Nota**: O Scalar usa o padrão `/api-docs/{documentName}`, onde `{documentName}` corresponde à versão da API (neste caso, `v1`).

### Pré-requisitos

Para usar a documentação, os seguintes serviços devem estar em execução:

```bash
# Terminal 1 - Auth Server (OpenIddict)
cd src/EChamado/Echamado.Auth
dotnet run
# Disponível em: https://localhost:7132

# Terminal 2 - API Server
cd src/EChamado/Server/EChamado.Server
dotnet run
# Disponível em: https://localhost:7296

# Terminal 3 - Blazor Client (opcional, para fluxo completo)
cd src/EChamado/Client/EChamado.Client
dotnet run
# Disponível em: https://localhost:7274
```

### Configuração no Projeto

O Scalar está configurado no `Program.cs` com a ordem correta dos middlewares:

```csharp
// 1. UseRouting() - Define o roteamento
app.UseRouting();

// 2. UseAuthentication/UseAuthorization - Segurança
app.UseAuthentication();
app.UseAuthorization();

// 3. UseApiDocumentation() - Scalar (deve vir depois de UseRouting)
app.UseApiDocumentation();

// 4. Map endpoints - Mapeia as rotas da API
app.MapEndpoints();
app.MapControllers();
```

**Importante**: O método `UseApiDocumentation()` deve ser chamado **depois de `UseRouting()`** pois internamente usa `MapScalarApiReference()` que requer o roteamento estar configurado.

## Interface do Scalar

### Funcionalidades Principais

1. **Tema Purple com Dark Mode**: Interface moderna e agradável aos olhos
2. **Pesquisa Rápida**: Tecla de atalho `K` para buscar endpoints
3. **Sidebar Navegável**: Navegação por categorias e endpoints
4. **Modelos de Dados**: Visualização de schemas e DTOs
5. **Exemplos de Código**: Geração automática em C# (HttpClient), cURL, e outras linguagens
6. **Teste Interativo**: Execute requisições diretamente da documentação

### Layout da Interface

```
┌─────────────────────────────────────────────────────────────┐
│  EChamado API Documentation                    [🔍 Search K] │
├──────────┬──────────────────────────────────────────────────┤
│          │                                                   │
│ Sidebar  │  Endpoint Details                                │
│          │                                                   │
│ Auth     │  GET /v1/categories                              │
│ Category │  ├─ Description                                  │
│ Order    │  ├─ Parameters                                   │
│ Comment  │  ├─ Request Body                                 │
│ ...      │  ├─ Responses                                    │
│          │  └─ Try it out                                   │
│          │                                                   │
│ Models   │  [Authorize] [Execute] [Copy Request]           │
│          │                                                   │
└──────────┴──────────────────────────────────────────────────┘
```

## Autenticação na Documentação

A API EChamado utiliza **OAuth 2.0 com OpenIddict**. A documentação suporta dois métodos de autenticação:

### Método 1: Bearer Token (Recomendado para Testes)

Este é o método mais simples para testar a API via documentação.

#### Passo 1: Obter o Token

Use um dos scripts de teste disponíveis na raiz do projeto:

**Bash/Linux/WSL:**
```bash
./test-openiddict-login.sh
```

**PowerShell/Windows:**
```powershell
.\test-openiddict-login.ps1
```

**Python (multiplataforma):**
```bash
python3 test-openiddict-login.py
```

**Ou via cURL diretamente:**
```bash
curl -k -X POST https://localhost:7132/connect/token \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=password" \
  -d "username=admin@admin.com" \
  -d "password=Admin@123" \
  -d "client_id=mobile-client" \
  -d "scope=openid profile email roles api chamados"
```

#### Passo 2: Copiar o Access Token

A resposta será algo como:

```json
{
  "access_token": "eyJhbGciOiJSUzI1NiIsImtpZCI6IjEyMzQ1...",
  "token_type": "Bearer",
  "expires_in": 3600,
  "refresh_token": "...",
  "scope": "openid profile email roles api chamados"
}
```

Copie **apenas o valor de `access_token`**.

#### Passo 3: Autenticar no Scalar

1. Acesse https://localhost:7296/api-docs/v1
2. Clique no botão **"Authorize"** 🔓 (canto superior direito)
3. Na janela de autenticação, selecione **"Bearer"**
4. Cole o `access_token` no campo (NÃO inclua a palavra "Bearer")
5. Clique em **"Authorize"**
6. Você verá um ícone de cadeado fechado 🔒 indicando que está autenticado

#### Passo 4: Testar Endpoints

Agora você pode testar qualquer endpoint protegido:

1. Navegue até o endpoint desejado (ex: `GET /v1/orders`)
2. Clique em **"Try it out"**
3. Preencha os parâmetros necessários
4. Clique em **"Execute"**
5. Veja a resposta em tempo real

### Método 2: OAuth2 Password Flow

Para testar o fluxo completo de OAuth2:

1. Clique em **"Authorize"** 🔓
2. Selecione **"OAuth2"**
3. Escolha o fluxo **"Password"** (Resource Owner Password Credentials)
4. Preencha:
   - **Username**: admin@admin.com
   - **Password**: Admin@123
   - **Client ID**: mobile-client
   - **Scopes**: Selecione os escopos necessários
     - `openid` - OpenID Connect
     - `profile` - Perfil do usuário
     - `email` - Email do usuário
     - `roles` - Permissões/Roles
     - `api` - Acesso à API
     - `chamados` - Acesso aos chamados
5. Clique em **"Authorize"**

## Explorando os Endpoints

### Categorias (Category)

#### GET /v1/categories - Buscar Categorias

**Descrição**: Lista e filtra categorias com paginação

**Parâmetros**:
- `name` (opcional): Filtrar por nome (busca parcial, case-insensitive)
- `description` (opcional): Filtrar por descrição
- `pageIndex` (padrão: 1): Número da página
- `pageSize` (padrão: 10, máximo: 100): Itens por página
- `order` (opcional): Campo para ordenação (ex: name, createdAt)

**Exemplo de Requisição**:
```http
GET /v1/categories?name=Hardware&pageIndex=1&pageSize=10
Authorization: Bearer eyJhbGciOiJSUzI1NiIs...
```

**Exemplo de Resposta** (200 OK):
```json
{
  "success": true,
  "data": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "name": "Hardware",
      "description": "Problemas com equipamentos físicos",
      "createdAt": "2025-01-15T10:00:00Z",
      "subCategories": [
        {
          "id": "...",
          "name": "Impressoras",
          "description": "Problemas com impressoras"
        }
      ]
    }
  ],
  "pageIndex": 1,
  "pageSize": 10,
  "totalCount": 1,
  "totalPages": 1
}
```

#### POST /v1/categories - Criar Categoria

**Descrição**: Cria uma nova categoria

**Request Body**:
```json
{
  "name": "Hardware",
  "description": "Problemas com equipamentos físicos"
}
```

**Resposta** (201 Created):
```json
{
  "success": true,
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "Hardware",
    "description": "Problemas com equipamentos físicos",
    "createdAt": "2025-01-19T12:00:00Z"
  },
  "message": "Categoria criada com sucesso"
}
```

#### PUT /v1/categories/{id} - Atualizar Categoria

**Descrição**: Atualiza uma categoria existente

**Path Parameters**:
- `id` (UUID): ID da categoria

**Request Body**:
```json
{
  "name": "Hardware e Periféricos",
  "description": "Problemas com equipamentos físicos e periféricos"
}
```

#### DELETE /v1/categories/{id} - Excluir Categoria

**Descrição**: Remove uma categoria (soft delete)

**Path Parameters**:
- `id` (UUID): ID da categoria

**Resposta** (204 No Content): Sem corpo de resposta

### Chamados (Orders)

#### GET /v1/orders - Listar Chamados

**Descrição**: Lista chamados com filtros avançados e paginação

**Parâmetros**:
- `title` (opcional): Filtrar por título
- `status` (opcional): Filtrar por status
- `priority` (opcional): Filtrar por prioridade
- `assignedUserId` (opcional): Filtrar por usuário atribuído
- `categoryId` (opcional): Filtrar por categoria
- `departmentId` (opcional): Filtrar por departamento
- `createdBy` (opcional): Filtrar por criador
- `createdFrom` (opcional): Data inicial de criação
- `createdTo` (opcional): Data final de criação
- `pageIndex` (padrão: 1): Número da página
- `pageSize` (padrão: 10): Itens por página

**Exemplo**:
```http
GET /v1/orders?status=open&priority=high&pageIndex=1&pageSize=20
Authorization: Bearer eyJhbGciOiJSUzI1NiIs...
```

#### POST /v1/orders - Criar Chamado

**Request Body**:
```json
{
  "title": "Impressora não funciona",
  "description": "A impressora do 3º andar não está imprimindo",
  "priority": "high",
  "categoryId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "subCategoryId": "...",
  "departmentId": "...",
  "orderTypeId": "...",
  "dueDate": "2025-01-25T18:00:00Z"
}
```

#### GET /v1/orders/{id} - Detalhes do Chamado

**Path Parameters**:
- `id` (UUID): ID do chamado

**Resposta**:
```json
{
  "success": true,
  "data": {
    "id": "...",
    "title": "Impressora não funciona",
    "description": "A impressora do 3º andar não está imprimindo",
    "status": "open",
    "priority": "high",
    "createdAt": "2025-01-19T10:00:00Z",
    "dueDate": "2025-01-25T18:00:00Z",
    "category": {
      "id": "...",
      "name": "Hardware"
    },
    "assignedUser": {
      "id": "...",
      "name": "João Silva",
      "email": "joao@echamado.com"
    },
    "comments": [
      {
        "id": "...",
        "text": "Verificando o problema...",
        "createdBy": "João Silva",
        "createdAt": "2025-01-19T10:30:00Z"
      }
    ]
  }
}
```

#### PUT /v1/orders/{id} - Atualizar Chamado

**Path Parameters**:
- `id` (UUID): ID do chamado

**Request Body**:
```json
{
  "title": "Impressora HP do 3º andar não funciona",
  "description": "A impressora HP LaserJet do 3º andar não está imprimindo. Mensagem de erro: 'Papel atolado'",
  "priority": "medium"
}
```

#### POST /v1/orders/{id}/assign - Atribuir Chamado

**Descrição**: Atribui um chamado a um usuário

**Path Parameters**:
- `id` (UUID): ID do chamado

**Request Body**:
```json
{
  "assignedUserId": "user-uuid-here"
}
```

#### POST /v1/orders/{id}/change-status - Alterar Status

**Request Body**:
```json
{
  "statusTypeId": "status-type-uuid-here"
}
```

#### POST /v1/orders/{id}/close - Fechar Chamado

**Descrição**: Marca o chamado como fechado

### Comentários (Comments)

#### GET /v1/orders/{orderId}/comments - Listar Comentários

**Path Parameters**:
- `orderId` (UUID): ID do chamado

#### POST /v1/orders/{orderId}/comments - Adicionar Comentário

**Path Parameters**:
- `orderId` (UUID): ID do chamado

**Request Body**:
```json
{
  "text": "Problema resolvido. Papel estava atolado no compartimento traseiro."
}
```

#### DELETE /v1/comments/{id} - Excluir Comentário

**Path Parameters**:
- `id` (UUID): ID do comentário

## Códigos de Resposta HTTP

### Sucesso (2xx)

- **200 OK**: Requisição bem-sucedida, retorna dados
- **201 Created**: Recurso criado com sucesso
- **204 No Content**: Requisição bem-sucedida, sem dados de retorno (ex: DELETE)

### Erro do Cliente (4xx)

- **400 Bad Request**: Dados inválidos na requisição
  ```json
  {
    "success": false,
    "errors": [
      "O campo 'name' é obrigatório",
      "O campo 'description' deve ter no mínimo 10 caracteres"
    ]
  }
  ```

- **401 Unauthorized**: Não autenticado (token ausente ou inválido)
  ```json
  {
    "type": "https://tools.ietf.org/html/rfc7235#section-3.1",
    "title": "Unauthorized",
    "status": 401
  }
  ```

- **403 Forbidden**: Autenticado mas sem permissão
  ```json
  {
    "success": false,
    "message": "Você não tem permissão para executar esta ação"
  }
  ```

- **404 Not Found**: Recurso não encontrado
  ```json
  {
    "success": false,
    "message": "Categoria não encontrada"
  }
  ```

### Erro do Servidor (5xx)

- **500 Internal Server Error**: Erro interno do servidor
  ```json
  {
    "success": false,
    "message": "Ocorreu um erro interno. Por favor, tente novamente mais tarde."
  }
  ```

## Geração de Código

O Scalar pode gerar código automaticamente para consumir a API. Para cada endpoint:

1. Clique no endpoint desejado
2. Na seção **"Code Samples"**, selecione a linguagem:
   - **C# (HttpClient)** - Recomendado para aplicações .NET
   - **cURL** - Para testes via terminal
   - **JavaScript (Fetch)** - Para aplicações web
   - **Python (Requests)** - Para scripts Python

### Exemplo: C# HttpClient

```csharp
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

var client = new HttpClient();
client.BaseAddress = new Uri("https://localhost:7296");

// Adicionar token de autenticação
var token = "eyJhbGciOiJSUzI1NiIs...";
client.DefaultRequestHeaders.Authorization =
    new AuthenticationHeaderValue("Bearer", token);

// GET /v1/categories
var response = await client.GetAsync("/v1/categories?name=Hardware&pageIndex=1&pageSize=10");
response.EnsureSuccessStatusCode();

var jsonString = await response.Content.ReadAsStringAsync();
var categories = JsonSerializer.Deserialize<CategoryListResponse>(jsonString);
```

### Exemplo: cURL

```bash
curl -X GET "https://localhost:7296/v1/categories?name=Hardware&pageIndex=1&pageSize=10" \
  -H "Accept: application/json" \
  -H "Authorization: Bearer eyJhbGciOiJSUzI1NiIs..."
```

## Modelos de Dados

O Scalar exibe todos os schemas de dados disponíveis na seção **"Models"** da sidebar. Você pode visualizar:

- Propriedades de cada modelo
- Tipos de dados
- Validações (required, min/max length, etc.)
- Relações entre entidades
- Exemplos de valores

### Principais Modelos

#### CategoryViewModel
```json
{
  "id": "uuid",
  "name": "string (1-100 caracteres)",
  "description": "string (0-500 caracteres)",
  "createdAt": "datetime",
  "updatedAt": "datetime",
  "subCategories": "SubCategoryViewModel[]"
}
```

#### OrderViewModel
```json
{
  "id": "uuid",
  "title": "string (required, 1-200 caracteres)",
  "description": "string (required, 1-2000 caracteres)",
  "status": "string",
  "priority": "string (low|medium|high|critical)",
  "dueDate": "datetime",
  "createdAt": "datetime",
  "categoryId": "uuid",
  "subCategoryId": "uuid",
  "departmentId": "uuid",
  "orderTypeId": "uuid",
  "assignedUserId": "uuid (nullable)",
  "createdByUserId": "uuid"
}
```

#### CommentViewModel
```json
{
  "id": "uuid",
  "text": "string (required, 1-2000 caracteres)",
  "orderId": "uuid",
  "createdByUserId": "uuid",
  "createdByUserName": "string",
  "createdAt": "datetime"
}
```

## Usuários Padrão (Seed Data)

Para testes, o sistema possui usuários pré-cadastrados:

### Administrador
- **Email**: admin@admin.com
- **Senha**: Admin@123
- **Roles**: Admin, User
- **Permissões**: Acesso total ao sistema

### Usuário Padrão
- **Email**: user@echamado.com
- **Senha**: User@123
- **Roles**: User
- **Permissões**: Criar e visualizar próprios chamados

## Scopes OAuth2

| Scope | Descrição | Necessário Para |
|-------|-----------|----------------|
| `openid` | OpenID Connect | Autenticação básica |
| `profile` | Perfil do usuário | Obter nome, foto, etc. |
| `email` | Email do usuário | Obter email |
| `roles` | Roles/Permissões | Autorização baseada em roles |
| `api` | Acesso à API | Chamar endpoints da API |
| `chamados` | Acesso aos chamados | CRUD de chamados |

**Recomendação**: Para acesso completo, solicite todos os scopes:
```
openid profile email roles api chamados
```

## Troubleshooting

### Problema: "Failed to fetch" ao executar requisições

**Possíveis Causas**:
1. API Server não está rodando (https://localhost:7296)
2. Auth Server não está rodando (https://localhost:7132)
3. Token expirado (tokens JWT expiram em 60 minutos)
4. CORS mal configurado

**Solução**:
1. Verifique se os servidores estão rodando:
   ```bash
   # Terminal 1
   cd src/EChamado/Echamado.Auth && dotnet run

   # Terminal 2
   cd src/EChamado/Server/EChamado.Server && dotnet run
   ```

2. Obtenha um novo token:
   ```bash
   ./test-openiddict-login.sh
   ```

3. Atualize a autenticação no Scalar com o novo token

### Problema: 401 Unauthorized

**Causas**:
- Token não fornecido
- Token inválido ou malformado
- Token expirado
- Token com issuer incorreto

**Solução**:
1. Verifique se você clicou em "Authorize" no Scalar
2. Certifique-se de que copiou o token completo
3. Obtenha um novo token (tokens expiram em 1 hora)
4. Use apenas tokens gerados pelo OpenIddict (porta 7132)

### Problema: 403 Forbidden

**Causa**: Usuário autenticado mas sem permissão

**Solução**:
- Use o usuário `admin@admin.com` para operações administrativas
- Verifique se o endpoint requer roles específicas
- Solicite os scopes corretos ao obter o token

### Problema: 400 Bad Request

**Causa**: Dados inválidos na requisição

**Solução**:
- Verifique os campos obrigatórios no modelo de dados
- Valide os tipos de dados (UUIDs, datas, etc.)
- Leia a mensagem de erro detalhada na resposta
- Consulte os exemplos na documentação

### Problema: Certificado SSL não confiável

**Causa**: Desenvolvimento usa certificados auto-assinados

**Solução temporária**:
- Use a flag `-k` no cURL: `curl -k https://...`
- No navegador, aceite o certificado não confiável
- Scripts de teste já incluem a opção para ignorar SSL

## Recursos Adicionais

### Documentação Relacionada

- **Autenticação Externa**: `docs/AUTENTICACAO-SISTEMAS-EXTERNOS.md`
- **Exemplos de Autenticação**: `docs/exemplos-autenticacao-openiddict.md`
- **Guia de Migração**: `docs/MIGRATION-GUIDE-JWT-TO-OPENIDDICT.md`
- **CLAUDE.md**: Guia completo do projeto na raiz

### URLs Úteis

- **Scalar Docs**: https://localhost:7296/api-docs/v1
- **OpenAPI JSON**: https://localhost:7296/openapi/v1.json
- **Health Checks**: https://localhost:7296/health
- **Auth Server**: https://localhost:7132

### Scripts de Teste

Disponíveis na raiz do projeto:

- `test-openiddict-login.sh` (Bash)
- `test-openiddict-login.ps1` (PowerShell)
- `test-openiddict-login.py` (Python)

### Teclas de Atalho no Scalar

- **K**: Abrir pesquisa rápida
- **Esc**: Fechar modais/pesquisa
- **↑/↓**: Navegar entre endpoints
- **Enter**: Abrir endpoint selecionado

## Suporte

Para dúvidas ou problemas:

1. Consulte a documentação completa em `/docs`
2. Verifique os logs da aplicação (Console ou Kibana)
3. Revise o `CLAUDE.md` para informações arquiteturais
4. Abra uma issue no repositório do projeto

---

**Versão**: 1.0.0
**Última Atualização**: 2025-01-19
**Compatibilidade**: EChamado API v1.0.0 com OpenIddict
