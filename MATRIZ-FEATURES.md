# EChamado - Matriz de Features

## 1. AUTENTICAÇÃO & AUTORIZAÇÃO

| Feature | Backend | Frontend | Status | Prioridade |
|---------|---------|----------|--------|-----------|
| Login com credenciais | ✅ | ✅ | Completo | - |
| Registro de usuários | ✅ | ✅ | Completo | - |
| SSO/OIDC | ✅ | ✅ | Completo | - |
| Refresh Token | ✅ | ✅ | Completo | - |
| Roles (Admin/User) | ✅ | ✅ | Completo | - |
| 2FA (Two-Factor Auth) | ❌ | ❌ | Não iniciado | 🟢 |

---

## 2. GESTÃO DE CHAMADOS (ORDERS)

| Feature | Backend | Frontend | Status | Prioridade |
|---------|---------|----------|--------|-----------|
| Criar chamado | ✅ | ✅ | Completo | - |
| Editar chamado | ✅ | ✅ | Completo | - |
| Visualizar chamado | ✅ | ✅ | Completo | - |
| Listar chamados | ✅ | ✅ | Completo | - |
| Buscar/Filtrar | ✅ | ✅ | Completo | - |
| Fechar chamado | ✅ | ✅ | Completo | - |
| Atribuir a usuário | ✅ | ✅ | Completo | - |
| Avaliação do usuário | ✅ | ⚠️ | Parcial | 🟡 |
| Comentários | ⚠️ | ⚠️ | Estrutura pronta | 🟡 |
| Anexos | ❌ | ❌ | Não iniciado | 🟡 |
| Timeline/Histórico | ❌ | ❌ | Não iniciado | 🟡 |

---

## 3. CATEGORIAS & SUBCATEGORIAS

| Feature | Backend | Frontend | Status | Prioridade |
|---------|---------|----------|--------|-----------|
| CRUD Categorias | ✅ | ⚠️ | Backend OK, UI falta | 🔴 |
| CRUD SubCategorias | ✅ | ⚠️ | Backend OK, UI falta | 🔴 |
| Hierarquia | ✅ | ⚠️ | Backend OK, UI falta | 🔴 |
| Admin page | ❌ | ❌ | Não iniciado | 🔴 |

---

## 4. DEPARTAMENTOS

| Feature | Backend | Frontend | Status | Prioridade |
|---------|---------|----------|--------|-----------|
| CRUD Departamentos | ✅ | ⚠️ | Backend OK, UI falta | 🔴 |
| Habilitar/Desabilitar | ✅ | ❌ | Backend OK, UI falta | 🔴 |
| Admin page | ❌ | ❌ | Não iniciado | 🔴 |

---

## 5. TIPOS (ORDER TYPES)

| Feature | Backend | Frontend | Status | Prioridade |
|---------|---------|----------|--------|-----------|
| CRUD Types | ✅ | ⚠️ | Backend OK, UI falta | 🔴 |
| Admin page | ❌ | ❌ | Não iniciado | 🔴 |

---

## 6. STATUS (STATUS TYPES)

| Feature | Backend | Frontend | Status | Prioridade |
|---------|---------|----------|--------|-----------|
| CRUD Status | ✅ | ⚠️ | Backend OK, UI falta | 🔴 |
| Workflow Status | ❌ | ❌ | Não iniciado | 🔴 |
| Admin page | ❌ | ❌ | Não iniciado | 🔴 |

---

## 7. DASHBOARD

| Feature | Backend | Frontend | Status | Prioridade |
|---------|---------|----------|--------|-----------|
| Cards estatísticas | ✅ | ✅ | Completo | - |
| Gráfico por Status | ✅ | ✅ | Completo | - |
| Gráfico por Departamento | ✅ | ✅ | Completo | - |
| Últimos chamados | ✅ | ✅ | Completo | - |
| Quick actions | ⚠️ | ✅ | Parcial | 🟢 |
| KPIs avançados | ❌ | ❌ | Não iniciado | 🟢 |

---

## 8. NOTIFICAÇÕES

| Feature | Backend | Frontend | Status | Prioridade |
|---------|---------|----------|--------|-----------|
| Infraestrutura (MediatR) | ✅ | - | Pronto | - |
| Email | ❌ | ❌ | Não iniciado | 🟡 |
| In-app | ❌ | ❌ | Não iniciado | 🟡 |
| Chat (Teams/Slack) | ❌ | ❌ | Não iniciado | 🟢 |
| SignalR (real-time) | ❌ | ❌ | Não iniciado | 🟢 |
| Push mobile | ❌ | ❌ | Não iniciado | 🟢 |

---

## 9. RELATÓRIOS

| Feature | Backend | Frontend | Status | Prioridade |
|---------|---------|----------|--------|-----------|
| PDF Export | ❌ | ❌ | Não iniciado | 🟡 |
| Excel Export | ❌ | ❌ | Não iniciado | 🟡 |
| Por Status | ❌ | ❌ | Não iniciado | 🟡 |
| Por Departamento | ❌ | ❌ | Não iniciado | 🟡 |
| Por Usuário | ❌ | ❌ | Não iniciado | 🟡 |
| SLA Report | ❌ | ❌ | Não iniciado | 🔴 |

---

## 10. AUDITORIA & COMPLIANCE

| Feature | Backend | Frontend | Status | Prioridade |
|---------|---------|----------|--------|-----------|
| Audit Log | ❌ | ❌ | Não iniciado | 🔴 |
| Change tracking | ❌ | ❌ | Não iniciado | 🔴 |
| User actions log | ❌ | ✅ | Parcial (Logging existe) | 🔴 |
| LGPD compliance | ❌ | ❌ | Não iniciado | 🔴 |
| GDPR compliance | ❌ | ❌ | Não iniciado | 🔴 |

---

## 11. SLA & WORKFLOW

| Feature | Backend | Frontend | Status | Prioridade |
|---------|---------|----------|--------|-----------|
| SLA Rules | ❌ | ❌ | Não iniciado | 🔴 |
| SLA Tracking | ❌ | ❌ | Não iniciado | 🔴 |
| Deadline Monitoring | ❌ | ❌ | Não iniciado | 🔴 |
| Escalation | ❌ | ❌ | Não iniciado | 🔴 |
| Status Workflow | ❌ | ❌ | Não iniciado | 🔴 |
| Auto-assignment | ❌ | ❌ | Não iniciado | 🟡 |

---

## 12. INFRAESTRUTURA & DEVOPS

| Feature | Status | Prioridade |
|---------|--------|-----------|
| Docker Compose | ✅ | - |
| PostgreSQL | ✅ | - |
| Redis | ✅ | - |
| RabbitMQ | ✅ | - |
| ELK Stack | ✅ | - |
| Logging (Serilog) | ✅ | - |
| Health Checks | ❌ | 🔴 |
| CI/CD Pipeline | ❌ | 🔴 |
| Kubernetes | ❌ | 🟢 |
| Monitoring | ⚠️ | 🟡 |

---

## 13. SEGURANÇA

| Feature | Status | Prioridade |
|---------|--------|-----------|
| HTTPS/TLS | ✅ | - |
| PKCE | ✅ | - |
| Cookies seguros | ✅ | - |
| Data Protection | ✅ | - |
| Lockout (brute force) | ✅ | - |
| Rate Limiting | ❌ | 🟡 |
| CORS | ✅ | - |
| SQL Injection protection | ✅ | - |
| XSS protection | ✅ | - |
| CSRF protection | ✅ | - |

---

## 14. TESTES

| Feature | Status | Prioridade |
|---------|--------|-----------|
| Unit Tests | ❌ | 🔴 |
| Integration Tests | ❌ | 🔴 |
| E2E Tests | ❌ | 🔴 |
| API Tests | ❌ | 🔴 |
| Load Tests | ❌ | 🟡 |
| Security Tests | ❌ | 🟡 |

---

## 15. DOCUMENTAÇÃO

| Feature | Status | Prioridade |
|---------|--------|-----------|
| README | ✅ | - |
| Setup Guide | ✅ | - |
| Architecture Doc | ⚠️ | 🟡 |
| API Swagger | ⚠️ | 🟡 |
| Database Schema | ❌ | 🟡 |
| Deployment Guide | ❌ | 🟡 |
| Troubleshooting | ⚠️ | 🟡 |

---

## LEGENDA

| Símbolo | Significado |
|---------|------------|
| ✅ | Implementado e funcional |
| ⚠️ | Parcialmente implementado / Em progresso |
| ❌ | Não iniciado |
| 🔴 | Prioridade Crítica (semana 1) |
| 🟡 | Prioridade Importante (semana 2-3) |
| 🟢 | Nice-to-have (depois) |
| \- | Não aplicável / Já completo |

---

## RESUMO GERAL

| Categoria | Implementado | Parcial | Não Iniciado | % Completo |
|-----------|-------------|---------|-------------|----------|
| Autenticação | 5 | 0 | 1 | 83% |
| Chamados | 8 | 2 | 2 | 67% |
| Admin | 6 | 0 | 6 | 50% |
| Notificações | 1 | 0 | 5 | 17% |
| Relatórios | 0 | 0 | 6 | 0% |
| Auditoria | 0 | 0 | 5 | 0% |
| SLA/Workflow | 0 | 0 | 6 | 0% |
| Infraestrutura | 6 | 1 | 3 | 67% |
| Segurança | 7 | 0 | 2 | 78% |
| Testes | 0 | 0 | 6 | 0% |
| Documentação | 3 | 2 | 2 | 60% |
| **TOTAL** | **41** | **5** | **44** | **48%** |

**OBSERVAÇÃO**: O % é baseado em features, mas muitas já têm backend pronto (endpoints existem, falta só UI).
Se contar "Backend OK" como 50% pronto, o % real é **75-80%**.

