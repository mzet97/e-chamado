# Arquitetura e Sistema de Autenticação - EChamado

## 📋 Índice
1. [Visão Geral da Arquitetura](#visão-geral-da-arquitetura)
2. [Sistema de Autenticação OpenIddict](#sistema-de-autenticação-openiddict)
3. [Fluxo de Autenticação](#fluxo-de-autenticação)
4. [Componentes do Sistema](#componentes-do-sistema)
5. [Configuração e Execução](#configuração-e-execução)
6. [Configuração do Banco de Dados](#configuração-do-banco-de-dados)
7. [Testando o Sistema](#testando-o-sistema)
8. [Troubleshooting](#troubleshooting)
9. [Estrutura de Arquivos](#estrutura-de-arquivos)
10. [Endpoints](#endpoints)

---

## 🏗️ Visão Geral da Arquitetura

O EChamado utiliza uma arquitetura de **três camadas** com separação clara de responsabilidades:

### Camadas da Aplicação

```mermaid
graph TB
    subgraph "EChamado System Architecture"
        direction LR

        %% Blazor Client
        BC[("🖥️ Blazor Client<br/>Porta 7274<br/>Blazor WebAssembly<br/>MudBlazor UI")]

        %% Auth Server
        AS[("🔐 Auth Server<br/>Porta 7132<br/>OpenIddict Authorization<br/>Server")]

        %% API Server
        API[("⚙️ API Server<br/>Porta 7296<br/>OpenIddict Resource<br/>Server")]

        %% Database
        DB[("🗄️ PostgreSQL<br/>Porta 5432<br/>OpenIddict + Identity<br/>+ Domain Data")]

        %% Services
        subgraph "Infrastructure Services"
            Redis[("📦 Redis<br/>Cache")]
            RabbitMQ[("📮 RabbitMQ<br/>Message Bus")]
            ELK[("📊 ELK Stack<br/>Logs")]
        end

        %% Connections
        BC -->|1. Request API| API
        API -->|2. 401 Unauthorized| BC
        BC -->|3. Redirect| AS
        AS -->|4. Login Form| BC
        BC -->|5. Login Credentials| AS
        AS -->|6. Generate JWT| AS
        AS -->|7. Redirect with code| BC
        BC -->|8. Exchange code for token| AS
        AS -->|9. JWT Token| BC
        BC -->|10. API Request + JWT| API
        API -->|11. Validate JWT| API
        API -->|12. Access DB| DB
        API -->|13. Success| BC

        BC -.->|Uses| Redis
        API -.->|Uses| Redis
        API -.->|Publishes| RabbitMQ
        API -.->|Logs to| ELK
    end
```

### Banco de Dados

```mermaid
erDiagram
    %% OpenIddict Tables
    OpenIddictApplications {
        string Id PK
        string ClientId
        string ClientSecret
        string DisplayName
        string Type
        json RedirectUris
        json PostLogoutRedirectUris
        json Permissions
        json Requirements
        datetime Created
        datetime Updated
    }

    OpenIddictAuthorizations {
        string Id PK
        string ApplicationId FK
        string Subject
        string Status
        string Scopes
        datetime Expiration
        datetime Created
        datetime Updated
    }

    OpenIddictScopes {
        string Id PK
        string Name
        string Resources
        datetime Created
        datetime Updated
    }

    OpenIddictTokens {
        string Id PK
        string ApplicationId FK
        string AuthorizationId FK
        string Type
        string Status
        string Subject
        string Scopes
        string CreationDate
        datetime Expiration
        datetime Created
        datetime Updated
    }

    %% Identity Tables
    AspNetUsers {
        string Id PK
        string UserName
        string NormalizedUserName
        string Email
        string NormalizedEmail
        bool EmailConfirmed
        string PasswordHash
        string SecurityStamp
        string ConcurrencyStamp
        string PhoneNumber
        bool PhoneNumberConfirmed
        bool TwoFactorEnabled
        datetime LockoutEnd
        bool LockoutEnabled
        int AccessFailedCount
    }

    AspNetRoles {
        string Id PK
        string Name
        string NormalizedName
        string ConcurrencyStamp
    }

    AspNetUserRoles {
        string UserId PK,FK
        string RoleId PK,FK
    }

    AspNetUserClaims {
        int Id PK
        string UserId FK
        string ClaimType
        string ClaimValue
    }

    AspNetRoleClaims {
        int Id PK
        string RoleId FK
        string ClaimType
        string ClaimValue
    }

    %% Domain Entities
    Categories {
        uuid Id PK
        string Name
        string Description
        bool IsActive
        datetime CreatedAt
        datetime UpdatedAt
    }

    SubCategories {
        uuid Id PK
        uuid CategoryId FK
        string Name
        string Description
        bool IsActive
        datetime CreatedAt
        datetime UpdatedAt
    }

    Orders {
        uuid Id PK
        string Title
        string Description
        uuid CategoryId FK
        uuid SubCategoryId FK
        uuid DepartmentId FK
        uuid OrderTypeId FK
        uuid StatusTypeId FK
        uuid AssignedToUserId FK
        uuid CreatedByUserId FK
        datetime DueDate
        string Priority
        string Status
        datetime CreatedAt
        datetime UpdatedAt
        datetime ClosedAt
    }

    Comments {
        uuid Id PK
        uuid OrderId FK
        uuid UserId FK
        string Comment
        datetime CreatedAt
    }

    %% Relationships
    OpenIddictAuthorizations ||--o{ OpenIddictTokens : has
    OpenIddictApplications ||--o{ OpenIddictAuthorizations : authorizes
    OpenIddictApplications ||--o{ OpenIddictTokens : issues

    AspNetUsers ||--o{ AspNetUserRoles : has
    AspNetRoles ||--o{ AspNetUserRoles : has
    AspNetUsers ||--o{ AspNetUserClaims : has
    AspNetRoles ||--o{ AspNetRoleClaims : has

    Categories ||--o{ SubCategories : contains
    Orders }o--|| Categories : belongs_to
    Orders }o--|| SubCategories : belongs_to
    Orders ||--o{ Comments : has
```

### Serviços de Infraestrutura

```mermaid
graph LR
    subgraph "Infrastructure Services"
        direction TB

        Redis[("📦 Redis<br/>Porta 6379<br/>• Cache distribuído<br/>• Output caching<br/>• Session storage")]

        RabbitMQ[("📮 RabbitMQ<br/>Porta 5672 (5671 TLS)<br/>• Message broker<br/>• Domain events<br/>• Async processing")]

        subgraph "ELK Stack"
            Elastic[("🔍 Elasticsearch<br/>Porta 9200<br/>• Log storage<br/>• Search engine<br/>• Analytics")]

            Logstash[("📥 Logstash<br/>Porta 5044-5046<br/>• Log processing<br/>• Data transformation<br/>• Pipeline")]

            Kibana[("📊 Kibana<br/>Porta 5601<br/>• Log visualization<br/>• Dashboards<br/>• Monitoring UI")]
        end

        %% Connections
        API -.->|Publish| RabbitMQ
        API -.->|Cache| Redis
        API -.->|Logs| Logstash
        Logstash -->|Store| Elastic
        Elastic -->|Visualize| Kibana
    end
```

---

## 🔐 Sistema de Autenticação OpenIddict

### Por que OpenIddict?

OpenIddict é uma implementação .NET completa dos padrões OAuth 2.0 e OpenID Connect, oferecendo:

- ✅ **Padrões Abertos**: OAuth 2.0 + OpenID Connect
- ✅ **Segurança**: PKCE, token encryption, refresh tokens
- ✅ **Flexibilidade**: Múltiplos flows de autenticação
- ✅ **Performance**: Validação local de tokens
- ✅ **Escalabilidade**: Suporte a Redis para cache distribuído

### Flows Suportados

1. **Authorization Code Flow + PKCE** (recomendado para SPA)
   - Usado pelo Blazor Client
   - Mais seguro para aplicações JavaScript

2. **Password Flow** (para APIslegacy)
   - Usado pelo AccountController
   - Autenticação direta com usuário/senha

3. **Client Credentials Flow** (para serviços)
   - Para comunicação service-to-service
   - Sem contexto de usuário

---

## 🔄 Fluxo de Autenticação

### Fluxo Padrão (Authorization Code + PKCE)

```mermaid
sequenceDiagram
    participant BC as Blazor Client
    participant AS as Auth Server (OpenIddict)
    participant API as API Server (OpenIddict Validation)
    participant DB as PostgreSQL

    Note over BC,DB: 🔄 Fluxo de Autenticação Completo

    %% Step 1: Initial Request
    BC->>API: 1️⃣ GET /v1/category
    Note right of BC: Headers: (sem token)

    %% Step 2: Unauthorized
    API->>BC: 2️⃣ 401 Unauthorized
    Note right of API: OpenIddict não encontra token
    Note right of API: Redirect: /connect/authorize

    %% Step 3: Redirect to Auth
    BC->>AS: 3️⃣ GET /connect/authorize?response_type=code&client_id=...
    Note right of BC: Authorization request

    %% Step 4: Show Login Form
    AS->>BC: 4️⃣ Redirect: /Account/Login?returnUrl=...
    Note right of AS: Usuário não autenticado

    %% Step 5: User Login
    BC->>AS: 5️⃣ POST /Account/DoLogin
    Note right of BC: email, password

    %% Step 6: Validate Credentials
    AS->>DB: 6️⃣ SELECT * FROM AspNetUsers WHERE Email = ?
    DB-->>AS: User data
    Note right of AS: Password OK ✓

    %% Step 7: Create Auth Cookie
    AS->>AS: 7️⃣ SignIn("External", userPrincipal)
    Note right of AS: Cookie criado

    %% Step 8: Generate Authorization Code
    AS->>AS: 8️⃣ Generate authorization_code
    AS->>DB: Store in OpenIddictAuthorizations
    AS->>BC: 9️⃣ Redirect to returnUrl with code

    %% Step 10: Exchange Code for Token
    BC->>AS: 🔟 POST /connect/token
    Note right of BC: grant_type=authorization_code<br/>code=xyz<br/>client_id=...

    %% Step 11: Validate Code
    AS->>DB: 🔍 SELECT * FROM OpenIddictAuthorizations WHERE Code=xyz
    DB-->>AS: Authorization data
    Note right of AS: Code valid ✓

    %% Step 12: Generate JWT
    AS->>AS: 🔑 Sign JWT with secret
    Note right of AS: iss=https://localhost:7132<br/>sub=user_id<br/>aud=blazor-client<br/>exp=...

    %% Step 13: Return Tokens
    AS-->>BC: 🔄 Access Token + Refresh Token
    Note right of AS: JWT: eyJhbGciOi...

    %% Step 14: Store Tokens
    BC->>BC: 💾 localStorage.setItem("access_token", ...)
    Note right of BC: Tokens salvos

    %% Step 15: Retry API Request
    BC->>API: 1️⃣5️⃣ GET /v1/category
    Note right of BC: Authorization: Bearer eyJ...

    %% Step 16: Validate JWT
    API->>API: 🔍 Validate JWT signature
    Note right of API: Verify with issuer key
    Note right of API: Check expiration
    Note right of API: Check audience

    %% Step 17: Extract Claims
    API->>API: 📋 Extract: user_id, roles, email
    Note right of API: Create ClaimsPrincipal

    %% Step 18: Success Response
    API->>DB: 1️⃣8️⃣ SELECT * FROM Categories
    DB-->>API: Category data
    API-->>BC: ✅ 200 OK + Categories JSON

    Note over BC,DB: ✨ Autenticação concluída com sucesso!
```

### Fluxo Detalhado

#### 1. **Solicitação Inicial**
```
Cliente → API Server: GET /v1/category
```
**Resposta:** `401 Unauthorized` + redirect

#### 2. **Redirecionamento para Auth**
```
API Server → Cliente: Location: https://localhost:7132/connect/authorize?...
```

#### 3. **Processo de Login**
```
Cliente → Auth Server: /Account/Login
    └── POST /Account/DoLogin (email, password)

Auth Server:
    ✓ Valida credenciais
    ✓ Cria cookie de autenticação
    ✓ Redireciona para returnUrl
```

#### 4. **Geração de Token**
```
Auth Server (OpenIddict):
    ✓ Valida authorization code
    ✓ Gera access token (JWT)
    ✓ Gera refresh token
    ✓ Retorna tokens para cliente
```

#### 5. **Armazenamento de Token**
```
Blazor Client:
    ✓ Armazena access token no localStorage
    ✓ Armazena refresh token
    ✓ Configura Authorization header
```

#### 6. **Acesso à API**
```
Cliente → API Server: GET /v1/category
    Header: Authorization: Bearer <jwt-token>

API Server (OpenIddict Validation):
    ✓ Valida assinatura do token
    ✓ Verifica expiração
    ✓ Valida escopos
    ✓ Extrai claims (user id, roles)
    ✓ Cria Identity
    ✓ Autoriza acesso
```

---

## 🧩 Componentes do Sistema

### 1. Auth Server (`Echamado.Auth`)

**Responsabilidade:** Authorization Server único que gera tokens JWT.

```mermaid
graph TB
    subgraph "Auth Server (Porta 7132)"
        direction TB

        subgraph "Web Layer"
            Login[("🖥️ Login.razor<br/>Página de Login")]
            Register[("📝 Register.razor<br/>Página de Registro")]
            AccountCtrl[("⚙️ AccountController<br/>Login/Logout Endpoints")]
        end

        subgraph "OpenIddict Layer"
            OIDC[("🔐 OpenIddict Server<br/>Authorization Server")]
            Authorize[("📋 /connect/authorize<br/>Authorization Endpoint")]
            Token[("🎫 /connect/token<br/>Token Endpoint")]
            Worker[("🚀 OpenIddictWorker<br/>Inicialização")]
        end

        subgraph "Authentication Layer"
            Identity[("🔑 ASP.NET Identity<br/>User Management")]
            SignIn[("✅ SignInManager<br/>Autenticação")]
            UserMgr[("👤 UserManager<br/>Usuários")]
        end

        subgraph "Data Layer"
            DB[("🗄️ PostgreSQL<br/>OpenIddict Tables:<br/>• Applications<br/>• Authorizations<br/>• Tokens<br/>• Scopes<br/><br/>Identity Tables:<br/>• Users<br/>• Roles<br/>• Claims")]
        end

        %% Connections
        Login --> AccountCtrl
        Register --> AccountCtrl
        AccountCtrl --> SignIn
        AccountCtrl --> UserMgr

        SignIn --> Identity
        UserMgr --> Identity

        Identity --> DB
        OIDC --> DB

        OIDC --> Authorize
        OIDC --> Token
        Worker --> OIDC
    end
```

#### Arquivos Principais

```
Echamado.Auth/
├── Program.cs                          # Configuração do OpenIddict
├── OpenIddictWorker.cs                 # Inicialização do OpenIddict
├── Controllers/
│   └── AccountController.cs            # Login/Logout
├── Components/
│   └── Pages/
│       └── Accounts/
│           ├── Login.razor             # UI de login
│           └── Register.razor          # UI de registro
└── appsettings.json                    # Configurações

Configuração OpenIddict:
- Issuer: https://localhost:7132
- Endpoints: /connect/authorize, /connect/token
- Flows: Authorization Code + PKCE, Password
- Secret: MXFhejJ3c3gzZWRjZHdkd3dxZnFlZ3JoanlrdWlsbw==
```

#### Endpoints Principais

| Endpoint | Método | Descrição |
|----------|--------|-----------|
| `/connect/authorize` | GET | Authorization endpoint (OAuth2) |
| `/connect/token` | POST | Token endpoint (JWT generation) |
| `/Account/Login` | GET | Página de login |
| `/Account/DoLogin` | POST | Processa login |
| `/Account/Logout` | GET/POST | Logout |

### 2. API Server (`EChamado.Server`)

**Responsabilidade:** Resource Server que valida tokens e expõe APIs.

```mermaid
graph TB
    subgraph "API Server (Porta 7296)"
        direction TB

        subgraph "API Layer"
            Cats[("📂 /v1/category<br/>Category Endpoints")]
            Orders[("📋 /v1/order<br/>Order Endpoints")]
            Depts[("🏢 /v1/department<br/>Department Endpoints")]
            Users[("👥 /v1/user<br/>User Endpoints")]
        end

        subgraph "Middleware Layer"
            Auth[("🔐 Authentication<br/>UseAuthentication()")]
            Authz[("🛡️ Authorization<br/>UseAuthorization()")]
            CORS[("🌐 CORS<br/>AllowBlazorClient")]
            Logging[("📝 Logging<br/>Request/Performance")]
            Compression[("🗜️ Compression<br/>Response")]
        end

        subgraph "CQRS Layer (Brighter)"
            Commands[("📤 Commands<br/>Create/Update/Delete")]
            Queries[("📥 Queries<br/>Read operations")]
            Handlers[("⚙️ Command/Query<br/>Handlers")]
            Validators[("✅ Validators<br/>FluentValidation")]
            Notifications[("📢 Notifications<br/>Domain Events")]
        end

        subgraph "OpenIddict Validation"
            Validator[("🔍 OpenIddict<br/>Validation Handler")]
            Claims[("📋 Claims<br/>Extract/Transform")]
            Identity[("👤 ClaimsPrincipal<br/>User Identity")]
        end

        subgraph "Infrastructure Layer"
            EF[("🗄️ Entity Framework<br/>Data Access")]
            Redis[("📦 Redis Cache<br/>Output Cache")]
            Rabbit[("📮 RabbitMQ<br/>Message Bus")]
            Health[("❤️ Health Checks<br/>DB/Redis/RabbitMQ")]
        end

        subgraph "Data Layer"
            DB[("🗄️ PostgreSQL<br/>Application Data")]
        end

        %% API Flow
        Cats --> Auth
        Orders --> Auth
        Depts --> Auth
        Users --> Auth

        Auth --> Authz
        Authz --> CORS
        CORS --> Logging
        Logging --> Compression

        %% CQRS Flow
        Compression --> Commands
        Compression --> Queries
        Commands --> Handlers
        Queries --> Handlers
        Handlers --> Validators
        Validators --> Notifications

        %% Authentication Flow
        Auth --> Validator
        Validator --> Claims
        Claims --> Identity

        %% Infrastructure
        Handlers --> EF
        EF --> DB
        EF --> Redis
        EF --> Rabbit
        EF --> Health
    end
```

#### Arquivos Principais

```
EChamado.Server/
├── Program.cs                          # Configuração geral
├── Infrastructure/
│   └── Configuration/
│       └── IdentityConfig.cs           # OpenIddict Validation
├── Endpoints/                          # Minimal API endpoints
│   ├── Categories/
│   ├── Orders/
│   ├── Departments/
│   └── Users/
└── appsettings.json

Configuração OpenIddict:
- Issuer: https://localhost:7132 (valida tokens do Auth Server)
- Esquema: OpenIddict.Validation.AspNetCore
- Funcionalidade: Apenas validação (não gera tokens)
```

#### Middleware de Autenticação

```csharp
// Em Program.cs
app.UseAuthentication();  // ✅ Habilitado
app.UseAuthorization();   // ✅ Habilitado

// Em IdentityConfig.cs
services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = "External";
})
```

### 3. Blazor Client (`EChamado.Client`)

**Responsabilidade:** Frontend SPA que consome APIs.

```mermaid
graph TB
    subgraph "Blazor Client (Porta 7274)"
        direction TB

        subgraph "Pages Layer"
            Home[("🏠 Home<br/>Dashboard")]
            Login[("🔐 Login<br/>Authentication Page")]
            Orders[("📋 Orders<br/>List/Create/Edit")]
            Categories[("📂 Categories<br/>Admin Page")]
            Departments[("🏢 Departments<br/>Admin Page")]
        end

        subgraph "UI Components"
            MudBlazor[("🎨 MudBlazor<br/>UI Framework")]
            Layout[("📐 MainLayout<br/>Layout Component")]
            Dialogs[("💬 Dialogs<br/>Create/Edit Forms")]
            Tables[("📊 Tables<br/>Data Display")]
        end

        subgraph "Authentication Layer"
            AuthState[("👤 AuthenticationState<br/>Authentication Provider")]
            CookieProvider[("🍪 CookieAuthenticationState<br/>Custom Provider")]
            HttpClient[("🌐 HttpClient<br/>Authenticated Client")]
            CookieHandler[("🔧 CookieHandler<br/>Credentials Handler")]
        end

        subgraph "Services Layer"
            CategorySvc[("📂 CategoryService<br/>Categories API")]
            OrderSvc[("📋 OrderService<br/>Orders API")]
            DeptSvc[("🏢 DepartmentService<br/>Departments API")]
            LookupSvc[("🔍 LookupService<br/>Lookups API")]
            CommentSvc[("💬 CommentService<br/>Comments API")]
        end

        subgraph "HTTP Configuration"
            AuthHttpClient[("🔗 Auth Server<br/>https://localhost:7132")]
            APIHttpClient[("🔗 API Server<br/>https://localhost:7296")]
            BaseAddress[("📍 BaseAddress<br/>Authorization Handler")]
        end

        subgraph "Storage"
            LocalStorage[("💾 localStorage<br/>Tokens & Cache")]
            BrowserStorage[("🌐 Browser Storage<br/>User Preferences")]
        end

        %% Page Flow
        Home --> MudBlazor
        Login --> CookieProvider
        Orders --> CategorySvc
        Categories --> CategorySvc
        Departments --> DeptSvc

        %% Layout
        Layout --> MudBlazor
        Dialogs --> MudBlazor
        Tables --> MudBlazor

        %% Auth Flow
        CookieProvider --> AuthState
        AuthState --> HttpClient
        HttpClient --> CookieHandler

        %% Services
        CategorySvc --> APIHttpClient
        OrderSvc --> APIHttpClient
        DeptSvc --> APIHttpClient
        LookupSvc --> APIHttpClient
        CommentSvc --> APIHttpClient

        %% HTTP Setup
        AuthHttpClient --> BaseAddress
        APIHttpClient --> BaseAddress

        %% Storage
        AuthState -.->|Store tokens| LocalStorage
        CookieHandler -.->|Send cookies| BaseAddress
        LookupSvc -.->|Cache data| BrowserStorage
    end
```

#### Arquivos Principais

```
EChamado.Client/
├── Program.cs                          # Configuração HTTP clients
├── Authentication/
│   └── CookieAuthenticationStateProvider.cs  # Estado de auth
├── Services/                           # HTTP clients para APIs
│   ├── CategoryService.cs
│   ├── OrderService.cs
│   └── ...
├── Security/
│   └── CookieHandler.cs                # Manipula cookies
├── Pages/
│   ├── Authentication/
│   │   ├── Login.razor
│   │   └── ...
│   └── ...
└── wwwroot/
    └── appsettings.json

Configuração:
- AuthServerUrl: https://localhost:7132
- BackendUrl: https://localhost:7296
```

---

## ⚙️ Configuração e Execução

### Pré-requisitos

- .NET 9 SDK
- PostgreSQL (com dados de exemplo)
- Redis (opcional, para cache)
- RabbitMQ (opcional, para eventos)

### 1. Configurar Arquivos de Configuração

#### Auth Server (`appsettings.json`)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=192.168.31.52;Port=5432;Database=e-chamado;..."
  },
  "AppSettings": {
    "Secret": "MXFhejJ3c3gzZWRjZHdkd3dxZnFlZ3JoanlrdWlsbw==",
    "ValidOn": "https://localhost:7132"
  }
}
```

#### API Server (`appsettings.json`)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=192.168.31.52;Port=5432;Database=e-chamado;..."
  },
  "AppSettings": {
    "Secret": "MXFhejJ3c3gzZWRjZHdkd3dxZnFlZ3JoanlrdWlsbw==",
    "ValidOn": "https://localhost:7296"
  }
}
```

#### Blazor Client (`wwwroot/appsettings.json`)
```json
{
  "AuthServerUrl": "https://localhost:7132",
  "BackendUrl": "https://localhost:7296"
}
```

### 2. Executar os Serviços

```mermaid
flowchart TD
    subgraph "Terminal 1"
        A1[("🔐 Auth Server")]
        A1 --> A2["cd src/EChamado/Echamado.Auth"]
        A2 --> A3["dotnet run"]
        A3 --> A4[("✅ Listening on<br/>https://localhost:7132")]
    end

    subgraph "Terminal 2"
        B1[("⚙️ API Server")]
        B1 --> B2["cd src/EChamado/Server/EChamado.Server"]
        B2 --> B3["dotnet run"]
        B3 --> B4[("✅ Listening on<br/>https://localhost:7296")]
    end

    subgraph "Terminal 3"
        C1[("🖥️ Blazor Client")]
        C1 --> C2["cd src/EChamado/Client/EChamado.Client"]
        C2 --> C3["dotnet run"]
        C3 --> C4[("✅ Listening on<br/>https://localhost:7274")]
    end

    subgraph "Infrastructure (Docker)"
        D1[("🐳 PostgreSQL<br/>Porta 5432")]
        D2[("🐳 Redis (Opcional)<br/>Porta 6379")]
        D3[("🐳 RabbitMQ (Opcional)<br/>Porta 5672")]
    end

    %% Dependencies
    A4 -.->|Uses| D1
    B4 -.->|Uses| D1
    A4 -.->|Uses| D2
    B4 -.->|Uses| D2
    B4 -.->|Uses| D3

    subgraph "Verification"
        V1[("✅ Test Auth Server<br/>curl https://localhost:7132/health")]
        V2[("✅ Test API Server<br/>curl https://localhost:7296/health")]
        V3[("✅ Test Blazor Client<br/>https://localhost:7274")]
    end

    A4 --> V1
    B4 --> V2
    C4 --> V3

    style A4 fill:#90EE90
    style B4 fill:#90EE90
    style C4 fill:#90EE90
```

#### Comandos Detalhados

##### Terminal 1 - Auth Server
```bash
cd src/EChamado/Echamado.Auth
dotnet run
```
**Output esperado:**
```
info: OpenIddictWorker[0]
      🔑 Database ready for OpenIddict
info: Microsoft.Hosting.Lifetime[0]
      Now listening on: https://localhost:7132
```

##### Terminal 2 - API Server
```bash
cd src/EChamado/Server/EChamado.Server
dotnet run
```
**Output esperado:**
```
info: Microsoft.Hosting.Lifetime[0]
      Now listening on: https://localhost:7296
info: EChamado.Server.Infrastructure.IdentityConfig[0]
      OpenIddict validation configured for issuer: https://localhost:7132
```

##### Terminal 3 - Blazor Client
```bash
cd src/EChamado/Client/EChamado.Client
dotnet run
```
**Output esperado:**
```
info: Microsoft.Hosting.Lifetime[0]
      Now listening on: https://localhost:7274
```

### 3. Verificar Status

Acesse:
- **Auth Server**: https://localhost:7132/health
- **API Server**: https://localhost:7296/health
- **Blazor Client**: https://localhost:7274

---

## 🔄 Exemplo Prático: Requisição Completa

```mermaid
sequenceDiagram
    participant User as 👤 Usuário
    participant BC as 🖥️ Blazor Client
    participant API as ⚙️ API Server
    participant OIDC as 🔐 OpenIddict Validator
    participant AS as 🔐 Auth Server
    participant DB as 🗄️ PostgreSQL

    Note over User,DB: Exemplo: Listar Categorias

    %% User clicks button
    User->>BC: Clica em "Listar Categorias"

    %% Client makes request
    BC->>API: GET /v1/category
    Note right of BC: Authorization: Bearer null

    %% API validates token
    OIDC->>OIDC: No token found
    API-->>BC: 401 Unauthorized + redirect

    %% Redirect flow
    BC->>AS: GET /connect/authorize?...
    AS-->>BC: 302 Redirect to /Account/Login

    %% User login
    User->>BC: Insere credenciais
    BC->>AS: POST /Account/DoLogin
    Note right of BC: email: admin@echamado.com<br/>password: Admin@123

    %% Auth Server validates
    AS->>DB: SELECT * FROM AspNetUsers WHERE Email=admin@echamado.com
    DB-->>AS: User found
    AS->>AS: Validate password ✓
    AS->>AS: SignIn("External")

    %% Generate code
    AS->>AS: Generate authorization_code
    AS->>DB: Store in OpenIddictAuthorizations
    AS-->>BC: Redirect with code

    %% Exchange for token
    BC->>AS: POST /connect/token
    Note right of BC: grant_type=authorization_code

    AS->>DB: Validate code
    DB-->>AS: Code valid

    AS->>AS: Sign JWT
    Note right of AS: iss=https://localhost:7132<br/>sub=user-uuid<br/>aud=blazor-client

    AS-->>BC: Return access_token + refresh_token
    Note right of AS: JWT: eyJhbGciOi...

    %% Client stores token
    BC->>BC: localStorage.setItem("access_token", ...)

    %% Retry API request
    BC->>API: GET /v1/category
    Note right of BC: Authorization: Bearer eyJ...

    %% API validates
    OIDC->>OIDC: Validate JWT signature
    OIDC->>OIDC: Check issuer: https://localhost:7132 ✓
    OIDC->>OIDC: Check expiration ✓
    OIDC->>OIDC: Extract claims

    %% API processes request
    API->>DB: SELECT * FROM Categories
    DB-->>API: Categories data

    %% Success response
    API-->>BC: 200 OK + [{"id":..., "name":...}]

    %% Client displays
    BC->>BC: Update UI with categories
    BC->>User: Exibe lista de categorias ✅
```

## 💾 Configuração do Banco de Dados

### Usuários Padrão

Após executar as migrations, o sistema cria automaticamente:

| Email | Senha | Roles |
|-------|-------|-------|
| admin@echamado.com | Admin@123 | Admin, User |
| user@echamado.com | User@123 | User |

### Estrutura do Banco

#### Tabelas OpenIddict
```sql
-- Tabelas criadas automaticamente pelo OpenIddict
OpenIddictApplications     # Aplicações cliente registradas
OpenIddictAuthorizations   # Autorizações ativas
OpenIddictScopes          # Escopos (scopes) definidos
OpenIddictTokens          # Tokens (access, refresh)
```

#### Tabelas Identity
```sql
AspNetUsers              # Usuários do sistema
AspNetRoles             # Roles (Admin, User, etc.)
AspNetUserRoles         # Relacionamento User-Role
AspNetUserClaims        # Claims dos usuários
AspNetRoleClaims        # Claims das roles
```

#### Tabelas de Domínio
```sql
Categories              # Categorias de orders
SubCategories          # Subcategorias
Departments            # Departamentos
OrderTypes             # Tipos de ordem
StatusTypes            # Status de ordem
Orders                 # Ordens (tickets)
Comments               # Comentários das ordens
```

---

## 🧪 Testando o Sistema

### 1. Teste via Swagger (API Server)

Acesse: https://localhost:7296/swagger

1. Clique em "Authorize"
2. Cole um JWT token válido
3. Teste endpoints protegidos

### 2. Teste via Interface (Blazor Client)

Acesse: https://localhost:7274

1. Clicar em "Login"
2. Inserir credenciais:
   - Email: `admin@echamado.com`
   - Senha: `Admin@123`
3. Acessar "Orders" ou "Categories"

### 3. Teste via Postman

#### Login (Password Flow)
```http
POST https://localhost:7132/connect/token
Content-Type: application/x-www-form-urlencoded

grant_type=password&
username=admin@echamado.com&
password=Admin@123&
client_id=blazor-client&
scope=openid profile email roles api
```

**Resposta:**
```json
{
  "access_token": "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9...",
  "token_type": "Bearer",
  "expires_in": 3600,
  "refresh_token": "...",
  "scope": "openid profile email roles api"
}
```

#### Usar Token
```http
GET https://localhost:7296/v1/categories
Authorization: Bearer eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9...
```

### 4. Verificar Validação de Token

```bash
# Decodificar JWT (use jwt.io ou ferramentas online)
# Verificar campos:
# - iss (issuer): https://localhost:7132
# - aud (audience): blazor-client
# - exp (expiration): timestamp futuro
# - sub (subject): user ID
# - role: Admin ou User
```

---

## 🔧 Troubleshooting

### Problema: 401 Unauthorized ao acessar API

#### Causas Possíveis:

1. **Token não enviado**
   ```bash
   # Verificar se o header está presente:
   curl -i https://localhost:7296/v1/categories
   # Deve mostrar: Authorization: Bearer <token>
   ```

2. **Token expirado**
   ```bash
   # Verificar se exp < timestamp atual
   # Solução: Fazer login novamente
   ```

3. **Issuer incorreto**
   ```bash
   # Verificar se o token foi gerado por:
   # Auth Server (https://localhost:7132)
   # E não pelo API Server
   ```

### Problema: 302 Redirect Loop

#### Solução:
Verificar se o Auth Server está rodando na porta 7132.

```bash
curl -i https://localhost:7132/health
# Deve retornar 200 OK
```

### Problema: Token inválido

#### Verificações:

1. **Chave secreta**
   ```bash
   # Verificar se Auth Server e API Server usam a mesma chave:
   # Auth Server: appsettings.json > AppSettings > Secret
   # API Server: appsettings.json > AppSettings > Secret
   ```

2. **Issuer**
   ```bash
   # Auth Server: https://localhost:7132
   # API Server deve validar: https://localhost:7132
   ```

3. **Issuer na configuração do API Server**
   ```csharp
   // IdentityConfig.cs linha ~184
   options.SetIssuer(new Uri("https://localhost:7132")); // ✅ Correto
   ```

### Problema: CORS Error

#### Solução:
Verificar CORS no API Server:

```csharp
// Program.cs
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", policy =>
    {
        policy.WithOrigins(
            "https://localhost:7274",  // Blazor Client
            "https://localhost:7132"   // Auth Server
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();          // ✅ Importante
    });
});

app.UseCors("AllowBlazorClient");      // ✅ Chamado antes de UseRouting
```

### Problema: Cookie não compartilhado

#### Solução:
Verificar SameSite no Auth Server:

```csharp
// Program.cs - linha ~100
options.Cookie.SameSite = SameSiteMode.None;  // ✅ Para跨域
options.Cookie.SecurePolicy = CookieSecurePolicy.Always;  // ✅ HTTPS only
```

**⚠️ Importante:** O Auth Server deve estar em HTTPS (`https://localhost:7132`) mesmo em desenvolvimento.

### Problema: "AuthenticationScheme: OpenIddict.Validation.AspNetCore was not authenticated"

#### Causa: Token não presente ou inválido

**Soluções:**
1. Verificar se o cliente está enviando o token
2. Verificar se o token está no formato correto: `Bearer <token>`
3. Verificar se o token não expirou
4. Verificar se o issuer do token é `https://localhost:7132`

---

## 📁 Estrutura de Arquivos

### Projetos da Solução

```
src/EChamado/
├── EChamado.Shared/                    # DTOs e modelos compartilhados
│   ├── ViewModels/
│   └── Shared/
│       └── Settings/
│
├── Server/
│   ├── EChamado.Server.Domain/         # Entidades e interfaces
│   │   ├── Domains/
│   │   │   ├── Identities/            # User, Role, Claim
│   │   │   └── Orders/                # Order, Category, etc.
│   │   ├── Repositories/
│   │   └── Services/
│   │
│   ├── EChamado.Server.Application/    # CQRS + Brighter
│   │   ├── UseCases/
│   │   │   ├── Auth/                  # Login, Register
│   │   │   ├── Orders/
│   │   │   ├── Categories/
│   │   │   └── ...
│   │   ├── Configuration/
│   │   └── Services/
│   │
│   ├── EChamado.Server.Infrastructure/ # EF Core, OpenIddict
│   │   ├── Persistence/
│   │   │   ├── Mappings/              # Entity Framework mappings
│   │   │   ├── Repositories/
│   │   │   └── Migrations/
│   │   ├── Configuration/
│   │   │   └── IdentityConfig.cs      # ⚡ OpenIddict Validation
│   │   ├── Redis/
│   │   ├── MessageBus/
│   │   └── OpenIddict/
│   │
│   └── EChamado.Server/               # ASP.NET Core API
│       ├── Endpoints/                 # Minimal API endpoints
│       ├── Configuration/
│       ├── Middlewares/
│       └── Program.cs
│
├── Client/
│   ├── EChamado.Client.Application/   # Client-side handlers
│   │   └── UseCases/Auth/
│   │
│   └── EChamado.Client/               # Blazor WebAssembly
│       ├── Authentication/
│       │   └── CookieAuthenticationStateProvider.cs  # ⚡ Auth state
│       ├── Services/                  # HTTP clients
│       ├── Pages/
│       ├── Components/
│       ├── Security/
│       │   └── CookieHandler.cs       # ⚡ Manipula cookies
│       ├── Configuration.cs
│       └── Program.cs
│
└── Echamado.Auth/                     # ⚡ OpenIddict Authorization Server
    ├── Controllers/
    │   └── AccountController.cs       # ⚡ Login/Logout
    ├── Components/
    ├── OpenIddictWorker.cs            # ⚡ Inicialização OpenIddict
    ├── Program.cs                     # ⚡ Configuração OpenIddict
    └── appsettings.json               # ⚡ AppSettings (Secret, Issuer)
```

---

## 🌐 Endpoints

### Auth Server (https://localhost:7132)

| Categoria | Endpoint | Método | Descrição |
|-----------|----------|--------|-----------|
| **OpenIddict** | | | |
| | `/connect/authorize` | GET | Authorization endpoint |
| | `/connect/token` | POST | Token endpoint |
| **Account** | | | |
| | `/Account/Login` | GET | Página de login |
| | `/Account/DoLogin` | POST | Processar login |
| | `/Account/Logout` | GET/POST | Logout |
| | `/Account/Register` | GET | Página de registro |
| **Health** | | | |
| | `/health` | GET | Health check |

### API Server (https://localhost:7296)

| Categoria | Endpoint | Método | Auth | Descrição |
|-----------|----------|--------|------|-----------|
| **Categories** | | | | |
| | `/v1/category` | GET | ✅ | Listar categorias |
| | `/v1/category` | POST | ✅ | Criar categoria |
| | `/v1/category/{id}` | GET | ✅ | Obter categoria |
| | `/v1/category/{id}` | PUT | ✅ | Atualizar categoria |
| | `/v1/category/{id}` | DELETE | ✅ | Excluir categoria |
| **Orders** | | | | |
| | `/v1/order` | GET | ✅ | Listar ordens |
| | `/v1/order` | POST | ✅ | Criar ordem |
| | `/v1/order/{id}` | GET | ✅ | Obter ordem |
| | `/v1/order/{id}` | PUT | ✅ | Atualizar ordem |
| | `/v1/order/{id}` | DELETE | ✅ | Excluir ordem |
| **Departments** | | | | |
| | `/v1/department` | GET | ✅ | Listar departamentos |
| | `/v1/department` | POST | ✅ | Criar departamento |
| | `/v1/department/{id}` | GET | ✅ | Obter departamento |
| | `/v1/department/{id}` | PUT | ✅ | Atualizar departamento |
| | `/v1/department/{id}` | DELETE | ✅ | Excluir departamento |
| **Users** | | | | |
| | `/v1/user` | GET | ✅ | Listar usuários |
| | `/v1/user/{id}` | GET | ✅ | Obter usuário |
| **Health** | | | | |
| | `/health` | GET | | Health check |

### Blazor Client (https://localhost:7274)

| Categoria | Rota | Descrição |
|-----------|------|-----------|
| **Pages** | | |
| | `/` | Dashboard |
| | `/orders` | Listagem de ordens |
| | `/orders/{id}` | Detalhes da ordem |
| | `/orders/create` | Criar nova ordem |
| | `/admin/categories` | Admin - Categorias |
| | `/admin/departments` | Admin - Departamentos |
| | `/admin/order-types` | Admin - Tipos de ordem |
| | `/admin/status-types` | Admin - Status |
| | `/admin/users` | Admin - Usuários |
| **Authentication** | | |
| | `/authentication/login` | Login |
| | `/authentication/logout` | Logout |
| | `/authentication/register` | Registro |

---

## 📚 Referências

### Documentação OpenIddict
- [OpenIddict官方文档](https://documentation.openiddict.com/)
- [OpenIddict Samples](https://github.com/openiddict/openiddict-samples)

### Padrões OAuth2/OpenID Connect
- [OAuth 2.0](https://oauth.net/2/)
- [OpenID Connect](https://openid.net/connect/)

### Fluxos de Autenticação
- [Authorization Code Flow](https://tools.ietf.org/html/rfc6749#section-4.1)
- [PKCE (RFC 7636)](https://tools.ietf.org/html/rfc7636)

---

## ✅ Checklist de Validação

- [ ] Auth Server rodando em https://localhost:7132
- [ ] API Server rodando em https://localhost:7296
- [ ] Blazor Client rodando em https://localhost:7274
- [ ] Banco de dados conectado (PostgreSQL)
- [ ] OpenIddict configurado no Auth Server
- [ ] OpenIddict Validation configurado no API Server
- [ ] Chave secreta idêntica em Auth Server e API Server
- [ ] CORS configurado no API Server
- [ ] Usuários criados no banco (admin@echamado.com)
- [ ] Login funcionando (gerando JWT)
- [ ] API aceitando token JWT
- [ ] Logout funcionando

---

**📝 Última atualização:** 2025-11-14
**📌 Versão:** 1.0
**👨‍💻 Autor:** Sistema EChamado
