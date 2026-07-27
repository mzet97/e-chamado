# Plano: Frontend Next.js + Angular para EChamado

## Visão Geral

Criar dois frontends completos que coexistem com o Blazor Client atual, consumindo a mesma API REST (.NET 10) e o mesmo Auth Server (OpenIddict).

- **Next.js 15** (React 19) + Tailwind CSS + shadcn/ui — porta `3000`
- **Angular 19** + PrimeNG — porta `4200`

Ambos replicam 100% da UI do Blazor Client (20+ páginas, 10+ services, OIDC auth, AI query, OData, dark mode).

---

## Fase 1 — Infraestrutura e Auth (ambos)

### 1.1 Next.js Setup
- Criar projeto `src/Frontend/nextjs/` com `create-next-app` (App Router, TypeScript, Tailwind)
- Instalar: `shadcn/ui`, `next-auth` (ou `oidc-client-ts`), `axios`, `recharts`, `react-hook-form`, `zod`
- Configurar `next.config.ts` com proxy para API (`localhost:7296`) e Auth Server (`localhost:7133`)
- Criar `src/lib/api-client.ts` — axios instance com interceptor que lê JWT de `localStorage`/cookies e anexa `Authorization: Bearer`
- Criar `src/lib/auth.ts` — módulo OIDC: `login()` (PKCE authorize redirect), `handleCallback()` (exchange code→token), `logout()` (end_session redirect), `getUser()` (parse JWT claims)
- Criar `src/types/api.ts` — tipos TypeScript para `BaseResult<T>`, `BaseResultList<T>`, `PagedResult`, todos os DTOs do backend
- Implementar `src/app/auth/login/page.tsx` (redirect), `src/app/auth/callback/page.tsx` (exchange), `src/app/auth/logout/page.tsx`
- Criar `src/contexts/AuthContext.tsx` — React Context com `user`, `isAuthenticated`, `login()`, `logout()`, `getToken()`
- Criar `src/middleware.ts` — Next.js middleware para proteger rotas autenticadas

### 1.2 Angular Setup
- Criar projeto `src/Frontend/angular/` com `ng new` (standalone components, SCSS, routing)
- Instalar: `primeng`, `primeicons`, `angular-oauth2-oidc`, `chart.js`, `ng2-charts`
- Configurar `environment.ts` com `apiUrl`, `authUrl`, `clientId`, `redirectUri`
- Criar `src/app/core/services/api.service.ts` — HttpClient com interceptor que anexa Bearer token
- Criar `src/app/core/services/auth.service.ts` — OIDC: `login()`, `handleCallback()`, `logout()`, `getUser()`, `getToken()`
- Criar `src/app/core/guards/auth.guard.ts` — CanActivate guard para rotas protegidas
- Criar `src/app/core/models/` — interfaces TypeScript para todos os DTOs
- Implementar `src/app/auth/login/`, `src/app/auth/callback/`, `src/app/auth/logout/`
- Criar `src/app/core/interceptors/auth.interceptor.ts` — HTTP interceptor

### 1.3 Ambos: Testar Auth Flow
- Validar login→redirect→callback→token→API call em ambos
- Testar logout→end_session→callback→cleanup

---

## Fase 2 — Layout e Navegação

### 2.1 Next.js Layout
- `src/app/layout.tsx` — Root layout com sidebar + topbar (shadcn `Sheet` para mobile)
- `src/components/Sidebar.tsx` — Menu colapsável com os mesmos itens do Blazor:
  - Dashboard (`/`)
  - Chamados: Lista, Novo, Busca IA, OData
  - Admin (role-gated): Categorias, Subcategorias, Departamentos, Tipos, Status, Usuários, Perfis
- `src/components/TopBar.tsx` — Dark mode toggle, user menu com avatar, logout
- `src/components/AuthGuard.tsx` — Wrapper que redireciona para login se não autenticado
- Implementar dark mode via `next-themes` + Tailwind `dark:` classes

### 2.2 Angular Layout
- `src/app/layout/` — Shell com `p-sidebar` + `p-toolbar`
- `src/app/layout/sidebar/sidebar.component.ts` — PrimeNG `p-panelMenu` com mesma estrutura de navegação
- `src/app/layout/topbar/topbar.component.ts` — Dark mode toggle, user menu
- `src/app/core/guards/role.guard.ts` — Guard para rotas Admin
- Implementar dark mode via PrimeNG `primeng/config` + CSS variables

---

## Fase 3 — Dashboard (Home)

### 3.1 Next.js
- `src/app/page.tsx` — Dashboard com:
  - 4 KPI cards (shadcn `Card` com borda colorida)
  - `recharts` DonutChart por status
  - Tabela de últimos 10 chamados (shadcn `DataTable`)
  - Quick actions: "Novo Chamado", "Meus Atribuídos"

### 3.2 Angular
- `src/app/pages/dashboard/dashboard.component.ts` — Mesmos 4 KPI cards (PrimeNG `p-card`)
- PrimeNG `p-chart` (Chart.js) para donut
- PrimeNG `p-table` para últimos chamados

---

## Fase 4 — Orders (Chamados) — CRUD completo

### 4.1 Ambos: OrderService
- `GET /v1/orders?PageIndex=&PageSize=&Title=&StatusTypeId=&...`
- `POST /v1/orders` (create), `PUT /v1/orders` (update)
- `GET /v1/orders/{id}`, `POST /v1/orders/close`, `POST /v1/orders/assign`, `POST /v1/orders/status`
- `POST /v1/comments`, `GET /v1/comments/{orderId}/comments`

### 4.2 OrderList (Next.js)
- `src/app/orders/page.tsx` — shadcn `DataTable` com:
  - Filtros: texto, status dropdown, dept dropdown, type dropdown, date range, overdue toggle
  - Server-side pagination (shadcn DataTable com `onPaginationChange`)
  - Status chips coloridos (aberto=blue, andamento=amber, fechado=green)
  - Ações: Ver, Editar
  - `?assigned=true` query string para "Meus Atribuídos"

### 4.3 OrderList (Angular)
- `src/app/pages/orders/order-list/` — PrimeNG `p-table` com:
  - Mesmos filtros via `p-dropdown`, `p-calendar`, `p-inputSwitch`
  - Server-side pagination via `p-table` lazy loading
  - Status chips via `p-tag`

### 4.4 OrderForm (ambos)
- Create/edit form com: Title, Description, Type (required), Dept, Category→SubCategory cascading, DueDate
- Next.js: shadcn `Form` + `react-hook-form` + `zod`
- Angular: PrimeNG `p-dropdown`, `p-calendar`, reactive forms

### 4.5 OrderDetails (ambos)
- 2-colunas: Descrição + Comments timeline | Detalhes + Ações
- Comments: lista com badge "Interno", form de novo comentário com checkbox interno
- Ações: Mudar status, Assumir, Fechar (dialog com estrelas 1-5)
- Next.js: shadcn `Card`, `Dialog`, `Badge`, `Textarea`
- Angular: PrimeNG `p-card`, `p-dialog`, `p-tag`, `p-rating`

### 4.6 OrderListAI (ambos)
- Campo de texto para linguagem natural → POST `/v1/ai/nl-to-gridify` → aplica filtro Gridify
- Manual Gridify filter input
- Resultados em tabela

### 4.7 OrderListOData (ambos)
- Campo de filtro OData ($filter, $orderby, $top)
- Quick filter chips
- Contador de resultados

---

## Fase 5 — Admin Pages (CRUD)

### 5.1 Padrão Admin (ambos)
Cada entidade segue o mesmo padrão:
- **List page**: Tabela com filtros, delete com ConfirmationDialog, link para create/edit
- **Form page**: Create/edit com validação

### 5.2 Páginas a implementar (ambos)
| Página | Rota | Fields |
|--------|------|--------|
| Categories | `/admin/categories` | Name, Description |
| CategoryForm | `/admin/categories/create`, `/{id}/edit` | Name, Description |
| SubCategories | `/admin/subcategories` | Name, Description, CategoryId |
| SubCategoryForm | `/admin/subcategories/create`, `/{id}/edit` | Category dropdown, Name, Description |
| Departments | `/admin/departments` | Name, Description |
| DepartmentForm | `/admin/departments/create`, `/{id}/edit` | Name, Description |
| OrderTypes | `/admin/ordertypes` | Name, Description |
| OrderTypeForm | `/admin/ordertypes/create`, `/{id}/edit` | Name, Description |
| StatusTypes | `/admin/statustypes` | Name, Description |
| StatusTypeForm | `/admin/statustypes/create`, `/{id}/edit` | Name, Description |
| Users | `/admin/users` | Read-only: Email, UserName, EmailConfirmed, 2FA, Status |
| Roles | `/admin/roles` | CRUD via dialog: Name, Description |

---

## Fase 6 — Features Avançadas

### 6.1 SLA Dashboard (ambos)
- Endpoint: `GET /v1/dashboard/sla/stats`
- Cards: Overdue, At Risk, On Time, Compliance Rate

### 6.2 Team Stats (ambos)
- Endpoint: `GET /v1/dashboard/team/stats`
- Tabela de agentes: Email, Open, Closed, Overdue, Avg Resolution

### 6.3 Reports (ambos)
- Endpoint: `POST /v1/reports/generate`
- Formulário: Report Type (volume/performance/sla), Date range, Department
- Exibe resultado JSON

### 6.4 Full-text Search (ambos)
- Endpoint: `GET /v1/orders/search?q=texto`
- Campo de busca no OrderList com toggle para full-text

### 6.5 Attachments (ambos)
- Endpoint: `POST /v1/attachments` (upload), `GET /v1/attachments/{id}` (download)
- No OrderDetails: lista de anexos por comentário, botão de upload

---

## Fase 7 — Polish e Deploy

### 7.1 Ambos
- Dark mode completo (todas as páginas)
- Responsividade mobile (sidebar colapsável, tabelas scroll)
- Loading states (skeletons/spinners)
- Error handling global (toast notifications)
- Portuguese localization (labels, mensagens de erro)

### 7.2 Dockerfiles
- Next.js: `Dockerfile` multi-stage (node:20-alpine build → standalone output)
- Angular: `Dockerfile` multi-stage (node:20-alpine build → nginx serve)
- Atualizar `docker-compose.prod.yml` com os 2 novos services

### 7.3 CI/CD
- Atualizar `.github/workflows/ci-cd.yml` com build+test dos frontends

---

## Estrutura de Pastas

```
src/Frontend/
├── nextjs/                          # Next.js 15 + shadcn/ui
│   ├── src/
│   │   ├── app/
│   │   │   ├── layout.tsx           # Root layout (sidebar + topbar)
│   │   │   ├── page.tsx             # Dashboard
│   │   │   ├── auth/
│   │   │   │   ├── login/page.tsx
│   │   │   │   ├── callback/page.tsx
│   │   │   │   └── logout/page.tsx
│   │   │   ├── orders/
│   │   │   │   ├── page.tsx         # OrderList
│   │   │   │   ├── create/page.tsx
│   │   │   │   ├── [id]/page.tsx    # OrderDetails
│   │   │   │   ├── [id]/edit/page.tsx
│   │   │   │   ├── ai/page.tsx      # OrderListAI
│   │   │   │   └── odata/page.tsx   # OrderListOData
│   │   │   └── admin/
│   │   │       ├── categories/...
│   │   │       ├── subcategories/...
│   │   │       ├── departments/...
│   │   │       ├── ordertypes/...
│   │   │       ├── statustypes/...
│   │   │       ├── users/page.tsx
│   │   │       └── roles/page.tsx
│   │   ├── components/
│   │   │   ├── Sidebar.tsx
│   │   │   ├── TopBar.tsx
│   │   │   ├── AuthGuard.tsx
│   │   │   ├── NLQueryInput.tsx
│   │   │   ├── ConfirmationDialog.tsx
│   │   │   └── StatusChip.tsx
│   │   ├── lib/
│   │   │   ├── api-client.ts
│   │   │   ├── auth.ts
│   │   │   └── utils.ts
│   │   ├── services/
│   │   │   ├── order.service.ts
│   │   │   ├── category.service.ts
│   │   │   ├── department.service.ts
│   │   │   ├── lookup.service.ts
│   │   │   ├── comment.service.ts
│   │   │   ├── user.service.ts
│   │   │   ├── role.service.ts
│   │   │   ├── odata.service.ts
│   │   │   └── nl-query.service.ts
│   │   ├── types/
│   │   │   └── api.ts              # Todos os DTOs TypeScript
│   │   └── contexts/
│   │       └── AuthContext.tsx
│   ├── package.json
│   ├── tailwind.config.ts
│   ├── Dockerfile
│   └── next.config.ts
│
├── angular/                         # Angular 19 + PrimeNG
│   ├── src/
│   │   ├── app/
│   │   │   ├── app.component.ts
│   │   │   ├── app.routes.ts
│   │   │   ├── layout/
│   │   │   │   ├── shell.component.ts
│   │   │   │   ├── sidebar/sidebar.component.ts
│   │   │   │   └── topbar/topbar.component.ts
│   │   │   ├── auth/
│   │   │   │   ├── login/login.component.ts
│   │   │   │   ├── callback/callback.component.ts
│   │   │   │   └── logout/logout.component.ts
│   │   │   ├── pages/
│   │   │   │   ├── dashboard/dashboard.component.ts
│   │   │   │   ├── orders/
│   │   │   │   │   ├── order-list/order-list.component.ts
│   │   │   │   │   ├── order-form/order-form.component.ts
│   │   │   │   │   ├── order-details/order-details.component.ts
│   │   │   │   │   ├── order-list-ai/order-list-ai.component.ts
│   │   │   │   │   └── order-list-odata/order-list-odata.component.ts
│   │   │   │   └── admin/
│   │   │   │       ├── categories/...
│   │   │   │       ├── subcategories/...
│   │   │   │       ├── departments/...
│   │   │   │       ├── ordertypes/...
│   │   │   │       ├── statustypes/...
│   │   │   │       ├── users/users.component.ts
│   │   │   │       └── roles/roles.component.ts
│   │   │   ├── core/
│   │   │   │   ├── services/
│   │   │   │   │   ├── api.service.ts
│   │   │   │   │   ├── auth.service.ts
│   │   │   │   │   ├── order.service.ts
│   │   │   │   │   ├── category.service.ts
│   │   │   │   │   ├── department.service.ts
│   │   │   │   │   ├── lookup.service.ts
│   │   │   │   │   ├── comment.service.ts
│   │   │   │   │   ├── user.service.ts
│   │   │   │   │   ├── role.service.ts
│   │   │   │   │   ├── odata.service.ts
│   │   │   │   │   └── nl-query.service.ts
│   │   │   │   ├── guards/
│   │   │   │   │   ├── auth.guard.ts
│   │   │   │   │   └── role.guard.ts
│   │   │   │   ├── interceptors/
│   │   │   │   │   └── auth.interceptor.ts
│   │   │   │   └── models/
│   │   │   │       └── api.interfaces.ts
│   │   │   └── shared/
│   │   │       ├── components/
│   │   │       │   ├── nl-query-input/nl-query-input.component.ts
│   │   │       │   ├── confirmation-dialog/confirmation-dialog.component.ts
│   │   │       │   └── status-chip/status-chip.component.ts
│   │   │       └── pipes/
│   │   │           └── date-format.pipe.ts
│   │   ├── environments/
│   │   │   ├── environment.ts
│   │   │   └── environment.prod.ts
│   │   └── styles.scss
│   ├── angular.json
│   ├── package.json
│   ├── Dockerfile
│   └── tsconfig.json
```

---

## Estimativa de Esforço

| Fase | Escopo | Next.js | Angular | Total |
|------|--------|---------|---------|-------|
| 1 | Infra + Auth | 3h | 3h | 6h |
| 2 | Layout + Nav | 2h | 2h | 4h |
| 3 | Dashboard | 2h | 2h | 4h |
| 4 | Orders CRUD | 6h | 6h | 12h |
| 5 | Admin CRUD | 5h | 5h | 10h |
| 6 | Features avançadas | 4h | 4h | 8h |
| 7 | Polish + Deploy | 3h | 3h | 6h |
| **TOTAL** | | **25h** | **25h** | **50h** |

## Ordem de Execução

1. **Fase 1** — Setup + Auth (ambos em paralelo)
2. **Fase 2** — Layout (ambos em paralelo)
3. **Fase 3** — Dashboard (ambos em paralelo)
4. **Fase 4** — Orders (mais complexo, fazer primeiro Next.js depois Angular)
5. **Fase 5** — Admin (padrão repetitivo, fazer em paralelo)
6. **Fase 6** — Features avançadas (em paralelo)
7. **Fase 7** — Polish + Deploy (em paralelo)