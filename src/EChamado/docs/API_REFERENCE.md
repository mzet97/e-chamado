# 📖 API Reference - EChamado

## 📋 Índice

- [Visão Geral](#-visão-geral)
- [Autenticação](#-autenticação)
- [Endpoints de Autenticação](#-endpoints-de-autenticação)
- [Endpoints de Chamados](#-endpoints-de-chamados)
- [Endpoints de Usuários](#-endpoints-de-usuários)
- [Endpoints de Departamentos](#-endpoints-de-departamentos)
- [Endpoints de Relatórios](#-endpoints-de-relatórios)
- [Modelos de Dados](#-modelos-de-dados)
- [Códigos de Erro](#-códigos-de-erro)
- [Rate Limiting](#-rate-limiting)
- [Versionamento](#-versionamento)

## 🌟 Visão Geral

A API REST do EChamado segue os princípios RESTful e utiliza JSON para comunicação. Todas as operações requerem autenticação via JWT Bearer tokens obtidos através do fluxo OAuth2/OpenID Connect.

### 📊 Informações da API

| Propriedade | Valor |
|-------------|-------|
| **Base URL** | `https://api.echamado.com/v1` |
| **Versão** | 1.0 |
| **Protocolo** | HTTPS |
| **Formato** | JSON |
| **Charset** | UTF-8 |
| **OpenAPI** | 3.0.3 |

### 🔗 URLs por Ambiente

| Ambiente | Base URL | Swagger UI |
|----------|----------|------------|
| **Development** | `https://localhost:7296/v1` | `https://localhost:7296/swagger` |
| **Staging** | `https://staging-api.echamado.com/v1` | `https://staging-api.echamado.com/swagger` |
| **Production** | `https://api.echamado.com/v1` | `https://api.echamado.com/swagger` |

## 🔐 Autenticação

### Bearer Token Authentication

Todos os endpoints (exceto os de autenticação) requerem um token JWT válido no header `Authorization`:

```http
Authorization: Bearer eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9...
```

### Obtenção de Token

1. **Authorization Code Flow** (Recomendado para SPAs)
2. **Client Credentials Flow** (Para integrações server-to-server)
3. **Password Flow** (Apenas para clientes confiáveis)

### Scopes Disponíveis

| Scope | Descrição |
|-------|----------|
| `openid` | Identificação básica do usuário |
| `profile` | Informações do perfil |
| `email` | Endereço de email |
| `roles` | Roles e permissões |
| `api` | Acesso geral à API |
| `chamados` | Operações com chamados |
| `admin` | Operações administrativas |

## 🔑 Endpoints de Autenticação

### OAuth2/OpenID Connect Endpoints

#### Authorization Endpoint
```http
GET /connect/authorize
```

**Parâmetros Query:**
- `client_id` (required): ID do cliente
- `response_type` (required): `code`
- `scope` (required): Scopes solicitados
- `redirect_uri` (required): URI de callback
- `state` (required): Estado para CSRF protection
- `code_challenge` (required): PKCE challenge
- `code_challenge_method` (required): `S256`

**Exemplo:**
```http
GET /connect/authorize?client_id=bwa-client&response_type=code&scope=openid%20profile%20email%20api&redirect_uri=https://app.echamado.com/callback&state=xyz&code_challenge=abc&code_challenge_method=S256
```

#### Token Endpoint
```http
POST /connect/token
Content-Type: application/x-www-form-urlencoded
```

**Authorization Code Exchange:**
```http
grant_type=authorization_code&
client_id=bwa-client&
code=AUTHORIZATION_CODE&
redirect_uri=https://app.echamado.com/callback&
code_verifier=CODE_VERIFIER
```

**Refresh Token:**
```http
grant_type=refresh_token&
client_id=bwa-client&
refresh_token=REFRESH_TOKEN
```

**Response:**
```json
{
  "access_token": "eyJhbGciOiJSUzI1NiIs...",
  "token_type": "Bearer",
  "expires_in": 3600,
  "refresh_token": "def50200...",
  "scope": "openid profile email api"
}
```

#### UserInfo Endpoint
```http
GET /connect/userinfo
Authorization: Bearer {access_token}
```

**Response:**
```json
{
  "sub": "123e4567-e89b-12d3-a456-426614174000",
  "email": "user@example.com",
  "name": "João Silva",
  "email_verified": true,
  "roles": ["User", "Admin"]
}
```

## 📋 Endpoints de Chamados

### Listar Chamados
```http
GET /v1/orders
Authorization: Bearer {token}
```

**Parâmetros Query:**
- `page` (optional): Número da página (default: 1)
- `pageSize` (optional): Itens por página (default: 20, max: 100)
- `status` (optional): Filtro por status
- `departmentId` (optional): Filtro por departamento
- `assignedTo` (optional): Filtro por responsável
- `createdFrom` (optional): Data inicial (ISO 8601)
- `createdTo` (optional): Data final (ISO 8601)
- `search` (optional): Busca textual
- `sortBy` (optional): Campo para ordenação
- `sortDirection` (optional): `asc` ou `desc`

**Response:**
```json
{
  "data": [
    {
      "id": "123e4567-e89b-12d3-a456-426614174000",
      "title": "Problema com impressora",
      "description": "A impressora não está funcionando",
      "status": {
        "id": "456e7890-e89b-12d3-a456-426614174000",
        "name": "Aberto",
        "color": "#ff6b6b"
      },
      "priority": "Medium",
      "department": {
        "id": "789e0123-e89b-12d3-a456-426614174000",
        "name": "TI"
      },
      "category": {
        "id": "012e3456-e89b-12d3-a456-426614174000",
        "name": "Hardware"
      },
      "requestingUser": {
        "id": "345e6789-e89b-12d3-a456-426614174000",
        "name": "João Silva",
        "email": "joao@empresa.com"
      },
      "responsibleUser": {
        "id": "678e9012-e89b-12d3-a456-426614174000",
        "name": "Maria Santos",
        "email": "maria@empresa.com"
      },
      "createdAt": "2025-01-15T10:30:00Z",
      "updatedAt": "2025-01-15T14:20:00Z",
      "dueDate": "2025-01-17T18:00:00Z"
    }
  ],
  "pagination": {
    "page": 1,
    "pageSize": 20,
    "totalItems": 150,
    "totalPages": 8,
    "hasNext": true,
    "hasPrevious": false
  }
}
```

### Obter Chamado por ID
```http
GET /v1/orders/{id}
Authorization: Bearer {token}
```

**Parâmetros Path:**
- `id` (required): UUID do chamado

**Response:**
```json
{
  "id": "123e4567-e89b-12d3-a456-426614174000",
  "title": "Problema com impressora",
  "description": "A impressora HP LaserJet não está funcionando. Quando tento imprimir, aparece erro de papel atolado, mas não há papel atolado.",
  "status": {
    "id": "456e7890-e89b-12d3-a456-426614174000",
    "name": "Em Andamento",
    "color": "#feca57"
  },
  "priority": "High",
  "department": {
    "id": "789e0123-e89b-12d3-a456-426614174000",
    "name": "TI",
    "description": "Departamento de Tecnologia da Informação"
  },
  "category": {
    "id": "012e3456-e89b-12d3-a456-426614174000",
    "name": "Hardware",
    "description": "Problemas relacionados a equipamentos"
  },
  "subCategory": {
    "id": "234e5678-e89b-12d3-a456-426614174000",
    "name": "Impressoras",
    "description": "Problemas com impressoras e multifuncionais"
  },
  "requestingUser": {
    "id": "345e6789-e89b-12d3-a456-426614174000",
    "name": "João Silva",
    "email": "joao@empresa.com",
    "department": "Financeiro"
  },
  "responsibleUser": {
    "id": "678e9012-e89b-12d3-a456-426614174000",
    "name": "Maria Santos",
    "email": "maria@empresa.com",
    "department": "TI"
  },
  "createdAt": "2025-01-15T10:30:00Z",
  "updatedAt": "2025-01-15T14:20:00Z",
  "dueDate": "2025-01-17T18:00:00Z",
  "closedAt": null,
  "evaluation": null,
  "attachments": [
    {
      "id": "890e1234-e89b-12d3-a456-426614174000",
      "fileName": "erro_impressora.jpg",
      "fileSize": 245760,
      "contentType": "image/jpeg",
      "uploadedAt": "2025-01-15T10:32:00Z"
    }
  ],
  "comments": [
    {
      "id": "567e8901-e89b-12d3-a456-426614174000",
      "content": "Verificando o problema. Vou até o local em 30 minutos.",
      "author": {
        "id": "678e9012-e89b-12d3-a456-426614174000",
        "name": "Maria Santos"
      },
      "createdAt": "2025-01-15T11:00:00Z"
    }
  ]
}
```

### Criar Chamado
```http
POST /v1/orders
Authorization: Bearer {token}
Content-Type: application/json
```

**Request Body:**
```json
{
  "title": "Problema com impressora",
  "description": "A impressora não está funcionando corretamente",
  "departmentId": "789e0123-e89b-12d3-a456-426614174000",
  "categoryId": "012e3456-e89b-12d3-a456-426614174000",
  "subCategoryId": "234e5678-e89b-12d3-a456-426614174000",
  "priority": "Medium",
  "dueDate": "2025-01-17T18:00:00Z",
  "attachments": [
    {
      "fileName": "erro_impressora.jpg",
      "fileContent": "base64_encoded_content",
      "contentType": "image/jpeg"
    }
  ]
}
```

**Response:**
```json
{
  "id": "123e4567-e89b-12d3-a456-426614174000",
  "title": "Problema com impressora",
  "status": {
    "id": "456e7890-e89b-12d3-a456-426614174000",
    "name": "Aberto"
  },
  "createdAt": "2025-01-15T10:30:00Z",
  "message": "Chamado criado com sucesso"
}
```

### Atualizar Chamado
```http
PUT /v1/orders/{id}
Authorization: Bearer {token}
Content-Type: application/json
```

**Request Body:**
```json
{
  "title": "Problema com impressora - URGENTE",
  "description": "A impressora não está funcionando e precisa ser resolvido hoje",
  "priority": "High",
  "statusId": "789e0123-e89b-12d3-a456-426614174000",
  "responsibleUserId": "678e9012-e89b-12d3-a456-426614174000",
  "dueDate": "2025-01-16T18:00:00Z"
}
```

### Fechar Chamado
```http
POST /v1/orders/{id}/close
Authorization: Bearer {token}
Content-Type: application/json
```

**Request Body:**
```json
{
  "resolution": "Problema resolvido. Impressora estava com papel atolado no compartimento traseiro.",
  "evaluation": {
    "rating": 5,
    "comment": "Atendimento excelente e rápido!"
  }
}
```

### Adicionar Comentário
```http
POST /v1/orders/{id}/comments
Authorization: Bearer {token}
Content-Type: application/json
```

**Request Body:**
```json
{
  "content": "Problema identificado. Iniciando reparo.",
  "isInternal": false
}
```

## 👥 Endpoints de Usuários

### Listar Usuários
```http
GET /v1/users
Authorization: Bearer {token}
```

**Requer Scope:** `admin`

**Parâmetros Query:**
- `page` (optional): Número da página
- `pageSize` (optional): Itens por página
- `search` (optional): Busca por nome ou email
- `departmentId` (optional): Filtro por departamento
- `role` (optional): Filtro por role
- `isActive` (optional): Filtro por status ativo

### Obter Usuário por ID
```http
GET /v1/users/{id}
Authorization: Bearer {token}
```

### Criar Usuário
```http
POST /v1/users
Authorization: Bearer {token}
Content-Type: application/json
```

**Requer Scope:** `admin`

**Request Body:**
```json
{
  "email": "novo.usuario@empresa.com",
  "name": "Novo Usuário",
  "departmentId": "789e0123-e89b-12d3-a456-426614174000",
  "roles": ["User"],
  "isActive": true
}
```

### Atualizar Usuário
```http
PUT /v1/users/{id}
Authorization: Bearer {token}
Content-Type: application/json
```

### Desativar Usuário
```http
DELETE /v1/users/{id}
Authorization: Bearer {token}
```

## 🏢 Endpoints de Departamentos

### Listar Departamentos
```http
GET /v1/departments
Authorization: Bearer {token}
```

**Response:**
```json
{
  "data": [
    {
      "id": "789e0123-e89b-12d3-a456-426614174000",
      "name": "TI",
      "description": "Departamento de Tecnologia da Informação",
      "isActive": true,
      "createdAt": "2025-01-01T00:00:00Z",
      "userCount": 15
    }
  ]
}
```

### Criar Departamento
```http
POST /v1/departments
Authorization: Bearer {token}
Content-Type: application/json
```

**Requer Scope:** `admin`

**Request Body:**
```json
{
  "name": "Recursos Humanos",
  "description": "Departamento de gestão de pessoas"
}
```

### Atualizar Departamento
```http
PUT /v1/departments/{id}
Authorization: Bearer {token}
Content-Type: application/json
```

### Desativar Departamento
```http
DELETE /v1/departments/{id}
Authorization: Bearer {token}
```

## 📊 Endpoints de Relatórios

### Dashboard Metrics
```http
GET /v1/reports/dashboard
Authorization: Bearer {token}
```

**Parâmetros Query:**
- `period` (optional): `today`, `week`, `month`, `quarter`, `year`
- `departmentId` (optional): Filtro por departamento

**Response:**
```json
{
  "totalOrders": 1250,
  "openOrders": 45,
  "inProgressOrders": 23,
  "closedOrders": 1182,
  "averageResolutionTime": "2.5 days",
  "satisfactionRating": 4.7,
  "ordersByStatus": [
    {"status": "Aberto", "count": 45, "percentage": 3.6},
    {"status": "Em Andamento", "count": 23, "percentage": 1.8},
    {"status": "Fechado", "count": 1182, "percentage": 94.6}
  ],
  "ordersByDepartment": [
    {"department": "TI", "count": 450, "percentage": 36.0},
    {"department": "RH", "count": 200, "percentage": 16.0},
    {"department": "Financeiro", "count": 300, "percentage": 24.0}
  ],
  "ordersByPriority": [
    {"priority": "Low", "count": 500, "percentage": 40.0},
    {"priority": "Medium", "count": 600, "percentage": 48.0},
    {"priority": "High", "count": 120, "percentage": 9.6},
    {"priority": "Critical", "count": 30, "percentage": 2.4}
  ]
}
```

### Relatório Detalhado
```http
GET /v1/reports/detailed
Authorization: Bearer {token}
```

**Parâmetros Query:**
- `startDate` (required): Data inicial (ISO 8601)
- `endDate` (required): Data final (ISO 8601)
- `departmentId` (optional): Filtro por departamento
- `format` (optional): `json`, `csv`, `pdf`

### Exportar Relatório
```http
POST /v1/reports/export
Authorization: Bearer {token}
Content-Type: application/json
```

**Request Body:**
```json
{
  "reportType": "orders",
  "format": "pdf",
  "filters": {
    "startDate": "2025-01-01T00:00:00Z",
    "endDate": "2025-01-31T23:59:59Z",
    "departmentId": "789e0123-e89b-12d3-a456-426614174000"
  },
  "includeCharts": true,
  "includeDetails": true
}
```

**Response:**
```json
{
  "reportId": "report_123456",
  "status": "processing",
  "estimatedCompletion": "2025-01-15T15:30:00Z",
  "downloadUrl": null
}
```

### Status do Relatório
```http
GET /v1/reports/export/{reportId}
Authorization: Bearer {token}
```

**Response:**
```json
{
  "reportId": "report_123456",
  "status": "completed",
  "createdAt": "2025-01-15T15:25:00Z",
  "completedAt": "2025-01-15T15:28:00Z",
  "downloadUrl": "https://api.echamado.com/v1/reports/download/report_123456",
  "expiresAt": "2025-01-22T15:28:00Z"
}
```

## 📋 Modelos de Dados

### Order (Chamado)
```json
{
  "id": "string (uuid)",
  "title": "string (max: 200)",
  "description": "string (max: 2000)",
  "status": {
    "id": "string (uuid)",
    "name": "string",
    "color": "string (hex)"
  },
  "priority": "Low | Medium | High | Critical",
  "department": {
    "id": "string (uuid)",
    "name": "string"
  },
  "category": {
    "id": "string (uuid)",
    "name": "string"
  },
  "subCategory": {
    "id": "string (uuid)",
    "name": "string"
  },
  "requestingUser": {
    "id": "string (uuid)",
    "name": "string",
    "email": "string (email)"
  },
  "responsibleUser": {
    "id": "string (uuid)",
    "name": "string",
    "email": "string (email)"
  },
  "createdAt": "string (datetime)",
  "updatedAt": "string (datetime)",
  "dueDate": "string (datetime)",
  "closedAt": "string (datetime) | null",
  "evaluation": {
    "rating": "number (1-5)",
    "comment": "string"
  }
}
```

### User (Usuário)
```json
{
  "id": "string (uuid)",
  "email": "string (email)",
  "name": "string (max: 100)",
  "photo": "string (url) | null",
  "department": {
    "id": "string (uuid)",
    "name": "string"
  },
  "roles": ["string"],
  "isActive": "boolean",
  "lastLoginAt": "string (datetime) | null",
  "createdAt": "string (datetime)"
}
```

### Department (Departamento)
```json
{
  "id": "string (uuid)",
  "name": "string (max: 100)",
  "description": "string (max: 500) | null",
  "isActive": "boolean",
  "createdAt": "string (datetime)",
  "updatedAt": "string (datetime)",
  "userCount": "number"
}
```

### PagedResult<T>
```json
{
  "data": ["T"],
  "pagination": {
    "page": "number",
    "pageSize": "number",
    "totalItems": "number",
    "totalPages": "number",
    "hasNext": "boolean",
    "hasPrevious": "boolean"
  }
}
```

### BaseResult
```json
{
  "success": "boolean",
  "message": "string",
  "errors": ["string"] | null
}
```

### BaseResult<T>
```json
{
  "success": "boolean",
  "message": "string",
  "data": "T | null",
  "errors": ["string"] | null
}
```

## ❌ Códigos de Erro

### HTTP Status Codes

| Código | Descrição | Quando Ocorre |
|--------|-----------|---------------|
| **200** | OK | Requisição bem-sucedida |
| **201** | Created | Recurso criado com sucesso |
| **204** | No Content | Operação bem-sucedida sem conteúdo |
| **400** | Bad Request | Dados inválidos na requisição |
| **401** | Unauthorized | Token ausente ou inválido |
| **403** | Forbidden | Sem permissão para o recurso |
| **404** | Not Found | Recurso não encontrado |
| **409** | Conflict | Conflito de dados (ex: email duplicado) |
| **422** | Unprocessable Entity | Validação de negócio falhou |
| **429** | Too Many Requests | Rate limit excedido |
| **500** | Internal Server Error | Erro interno do servidor |
| **503** | Service Unavailable | Serviço temporariamente indisponível |

### Estrutura de Erro

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Validation Error",
  "status": 400,
  "detail": "One or more validation errors occurred.",
  "instance": "/v1/orders",
  "errors": {
    "title": ["The title field is required."],
    "departmentId": ["The departmentId field must be a valid UUID."]
  },
  "traceId": "0HN7GKQJKJ2QK:00000001"
}
```

### Códigos de Erro Customizados

| Código | Descrição |
|--------|----------|
| `INVALID_TOKEN` | Token JWT inválido ou expirado |
| `INSUFFICIENT_SCOPE` | Scope insuficiente para a operação |
| `RESOURCE_NOT_FOUND` | Recurso solicitado não encontrado |
| `DUPLICATE_RESOURCE` | Tentativa de criar recurso duplicado |
| `BUSINESS_RULE_VIOLATION` | Violação de regra de negócio |
| `EXTERNAL_SERVICE_ERROR` | Erro em serviço externo |
| `RATE_LIMIT_EXCEEDED` | Limite de requisições excedido |

## ⚡ Rate Limiting

### Limites por Endpoint

| Endpoint | Limite | Janela | Burst |
|----------|--------|-----------|-------|
| **Authentication** | 10 req/min | 1 minuto | 5 |
| **Orders (GET)** | 100 req/min | 1 minuto | 20 |
| **Orders (POST/PUT)** | 30 req/min | 1 minuto | 10 |
| **Reports** | 10 req/min | 1 minuto | 3 |
| **Admin Operations** | 50 req/min | 1 minuto | 10 |
| **File Upload** | 5 req/min | 1 minuto | 2 |

### Headers de Rate Limiting

```http
X-RateLimit-Limit: 100
X-RateLimit-Remaining: 95
X-RateLimit-Reset: 1642694400
X-RateLimit-Retry-After: 60
```

### Response de Rate Limit Excedido

```json
{
  "type": "https://httpstatuses.com/429",
  "title": "Too Many Requests",
  "status": 429,
  "detail": "Rate limit exceeded. Try again in 60 seconds.",
  "instance": "/v1/orders",
  "retryAfter": 60
}
```

## 📈 Versionamento

### Estratégia de Versionamento

A API utiliza versionamento via URL path:
- **Atual**: `/v1/`
- **Próxima**: `/v2/` (quando disponível)

### Política de Suporte

- **v1**: Suportada até dezembro de 2025
- **Deprecation Notice**: 6 meses antes da descontinuação
- **Breaking Changes**: Apenas em novas versões

### Headers de Versionamento

```http
API-Version: 1.0
API-Supported-Versions: 1.0
API-Deprecated-Versions: none
```

## 🔧 Ferramentas e SDKs

### Swagger/OpenAPI

- **Swagger UI**: `https://api.echamado.com/swagger`
- **OpenAPI Spec**: `https://api.echamado.com/swagger/v1/swagger.json`
- **Redoc**: `https://api.echamado.com/redoc`

### Postman Collection

```bash
# Importar collection
curl -o echamado-api.json https://api.echamado.com/postman/collection.json
```

### SDKs Disponíveis

| Linguagem | Repositório | Status |
|-----------|-------------|--------|
| **C#** | `EChamado.Client.SDK` | ✅ Disponível |
| **JavaScript** | `@echamado/api-client` | 🔄 Em desenvolvimento |
| **Python** | `echamado-python-sdk` | 📋 Planejado |
| **Java** | `echamado-java-sdk` | 📋 Planejado |

### Exemplo de Uso (C# SDK)

```csharp
using EChamado.Client.SDK;

// Configuração
var client = new EChamadoApiClient("https://api.echamado.com");
await client.AuthenticateAsync("your-access-token");

// Listar chamados
var orders = await client.Orders.GetAllAsync(new OrderSearchRequest
{
    Page = 1,
    PageSize = 20,
    Status = "Aberto"
});

// Criar chamado
var newOrder = await client.Orders.CreateAsync(new CreateOrderRequest
{
    Title = "Novo chamado",
    Description = "Descrição do problema",
    DepartmentId = Guid.Parse("..."),
    CategoryId = Guid.Parse("...")
});
```

## 🧪 Testes da API

### Health Check

```bash
# Verificar saúde da API
curl -X GET "https://api.echamado.com/health" \
  -H "accept: application/json"
```

### Autenticação de Teste

```bash
# Obter token de teste (apenas desenvolvimento)
curl -X POST "https://localhost:7296/connect/token" \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=client_credentials&client_id=test-client&client_secret=test-secret&scope=api"
```

### Exemplo de Requisição Completa

```bash
# Listar chamados
curl -X GET "https://api.echamado.com/v1/orders?page=1&pageSize=10" \
  -H "Authorization: Bearer eyJhbGciOiJSUzI1NiIs..." \
  -H "Accept: application/json" \
  -H "User-Agent: EChamado-Client/1.0"
```

---

## 📞 Suporte

### Contatos

- **Documentação**: https://docs.echamado.com
- **Suporte Técnico**: api-support@echamado.com
- **Status da API**: https://status.echamado.com
- **GitHub Issues**: https://github.com/echamado/api/issues

### SLA

- **Disponibilidade**: 99.9%
- **Tempo de Resposta**: < 200ms (P95)
- **Suporte**: 24/7 para clientes enterprise

---

**Última Atualização**: Janeiro 2025  
**Versão da API**: 1.0  
**Versão da Documentação**: 1.0