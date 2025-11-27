# Plano de Correção por Fases (Autenticação e Segurança)

## Objetivo
Eliminar brechas de elevação de privilégio, corrigir fluxo OIDC/PKCE e reforçar hardening de autenticação, com cobertura de testes alinhada.

---

## Fase 1 — Mitigação Imediata (hotfix)
- Remover ou proteger por feature flag/role o painel de debug e o botão “Login Teste” na Home (evitar password grant + credenciais hardcoded).
- Desativar password grant para o cliente público ou, se precisar manter temporariamente, habilitar lockout e rate limiting na rota `/connect/token`.
- Documentar e comunicar mudança de credenciais de admin (revogar `Admin@123`), se existirem em ambientes.

### Critério de pronto
- Painel de debug inacessível para usuários comuns em produção.
- Password grant bloqueado ou protegido por lockout/rate limit.
- Sem credenciais padrão ativas.

---

## Fase 2 — Correção do Fluxo PKCE/OIDC
- Persistir `state` no login (ex.: localStorage/sessionStorage) e validar no callback antes de trocar `code` por token; rejeitar se não corresponder.
- Garantir que `code_verifier` esteja sempre associado ao `state` e removido após uso.
- Revalidar redirecionamentos (`redirect_uri`, `post_logout_redirect_uri`) e CORS para ambientes de produção.

### Critério de pronto
- Login retorna 400/redirect seguro quando `state` é inválido ou ausente.
- Teste manual confirma troca de código somente com `state` válido.

---

## Fase 3 — Hardening do Auth Server e API
- Reforçar Identity/OpenIddict: lockout habilitado, rate limiting em `/connect/token`, MFA/2FA opcional para admins.
- Revisar CORS (evitar `AllowAnyOrigin` em produção); limitar origens aos clients oficiais.
- Validar logging: sem tokens ou PII em logs; máscara de dados sensíveis.
- Revisar políticas de roles/claims emitidas (apenas o mínimo necessário).

### Critério de pronto
- Policies de CORS restritas.
- Lockout e rate limiting ativos, com testes de carga confirmando bloqueio.
- Logs sem vazamento de tokens/PII.

---

## Fase 4 — Testes e Observabilidade
- Atualizar testes E2E para o fluxo PKCE real (login -> callback -> token armazenado -> chamadas autenticadas).
- Adicionar testes negativos: `state` inválido, `code_verifier` faltando, lockout após tentativas.
- Cobertura unitária/integration para handler de login/callback e validação de state.
- Observabilidade: métricas de falha de login, lockout e tentativas por IP/usuário.

### Critério de pronto
- Suíte E2E cobrindo cenários positivos/negativos de autenticação.
- Alarmes/alertas configurados para picos de falha e lockout.

---

## Fase 5 — Limpeza e Governança
- Remover artefatos de debug e credenciais de teste do repositório.
- Policy de secret management (dotnet user-secrets/KeyVault/SM) para não versionar segredos.
- Checklist de revisão de segurança em PRs envolvendo autenticação.

### Critério de pronto
- Repositório sem credenciais/test hooks.
- Processo de revisão com checklist aplicado.
