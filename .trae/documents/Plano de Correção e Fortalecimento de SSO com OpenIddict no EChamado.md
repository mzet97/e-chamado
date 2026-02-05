## Visão Atual (Arquitetura e Código)
- Client (Blazor WASM): `AddOidcAuthentication` com `Authority=https://localhost:7296`, `ClientId=bwa-client`, callbacks em `/authentication/login-callback` e `/authentication/logout-callback` (`Client/wwwroot/appsettings.json:1–10`, `Client/Program.cs:26–68`).
- Server (API + OIDC): OpenIddict Server/Validation habilitados, endpoints `/connect/authorize` e `/connect/token` com `UseAspNetCore().EnableAuthorizationEndpointPassthrough()` e `UseLocalServer()` para validação (`Server.Infrastructure/Configuration/IdentityConfig.cs:140–177`). Clientes registrados em `OpenIddictWorker.cs:37–121` (PKCE, escopos, URIs).
- Auth (Blazor Server UI): `Account/Login` coleta credenciais e `Account/DoLogin` integra com Identity, emite cookie de login e redireciona ao `returnUrl` (`Echamado.Auth/Components/Pages/Accounts/Login.razor:18–45`, `Echamado.Auth/Controllers/AccountController.cs:24–79`).

## Avaliação (Segurança, Conformidade, Vulnerabilidades, Performance)
- Segurança
  - Assinatura simétrica do token no servidor OIDC (adequado para dev, deve migrar para chaves **assimétricas** em produção). Referência OpenIddict: configuração de **signing/encryption certificates**.
  - Validação de token com `UseLocalServer()` OK para estrutura co-locada; em cenários distribuídos, usar **OpenID Connect discovery** e `UseSystemNetHttp()` (doc OpenIddict).
  - Fluxo Authorization Code + **PKCE** já ativo (boa prática em SPA/WASM) e `state/nonce` gerenciados pelo provider do WASM.
  - Cookies de **DataProtection** persistidos em `TempPath` (adequado em dev, migrar para storage durável em prod).
- Conformidade Microsoft/OWASP
  - HTTPS-only, CORS restrito a origens conhecidas; reforçar **security headers** (CSP, HSTS, X-Content-Type-Options) na API.
  - CSRF: antiforgery habilitado no Auth; para APIs com bearer tokens, CSRF não se aplica diretamente, mas headers e same-site protegem UI.
- Vulnerabilidades WASM
  - Evitar expor tokens em `localStorage/sessionStorage`; manter em memória (`AddOidcAuthentication`) ou migrar para **BFF com cookies HttpOnly** se a política exigir.
  - Sanitizar `returnUrl` para prevenir **open redirect**; construção absoluta para `https://localhost:7296` quando relativo.
- Performance/Escala
  - Ativar caching para requisições de autorização e **resilience pipeline** `UseSystemNetHttp().SetHttpResiliencePipeline(...)` (doc OpenIddict) para discovery/validação.
  - Minimizar claims e payload; compressão habilitada nas APIs; health checks e observabilidade estão presentes.

## Abordagem A — Corrigir e Fortalecer a Implementação Existente
### Server (OIDC + API)
- Assinatura/Criptografia
  - Dev: manter `AddDevelopmentSigningCertificate()`.
  - Prod: usar **X509 certificados** reais para signing/encryption (doc OpenIddict: encryption-and-signing-credentials), remover chave simétrica para assinatura.
- Validação
  - Co-locado: manter `UseLocalServer()`; distribuído: configurar **discovery** com `SetIssuer(...)` + `UseSystemNetHttp()` (doc OpenIddict).
- Endpoints e recursos
  - Confirmar escopos e URIs do `bwa-client` (PKCE, redirect/logout) em `OpenIddictWorker.cs:81–84,113–118`.
  - Habilitar `EnableAuthorizationRequestCaching()` e `EnableLogoutEndpointPassthrough()` (doc OpenIddict ASP.NET Core integration) para fluxos com payload maior.
- Resiliência HTTP
  - Configurar `SetHttpResiliencePipeline` para client/validation (doc OpenIddict System.Net.Http) com retry/backoff controlado.
- CORS e headers
  - CORS mínimo (`Server/Program.cs:25–37`) e adicionar **CSP/HSTS/X-Content-Type-Options/Referrer-Policy** via middleware (doc security headers).

### Auth (UI de Login)
- Login
  - Após autenticação, emitir `SignInAsync("External", principal)` e redirecionar ao `returnUrl` absoluto para o OIDC, com **whitelist** de host/porta/path (sanitização). 
- Antiforgery e HTTPS
  - Manter `UseAntiforgery()` e forçar HTTPS; garantir SameSite=None e Secure no cookie.

### Client (Blazor WASM)
- Inicialização
  - Verificar `AuthenticationState` ao montar e exibir **botão de login** quando não autenticado.
- Fluxo
  - Continuar com `RemoteAuthenticatorView` (Authorization Code + PKCE); opcionalmente, customizar construção de URL com `state`, `nonce`, `code_challenge` explicitamente se necessário.
- Storage de tokens
  - Manter tokens em memória; evitar localStorage. Se exigido por política, optar pela **alternativa BFF** abaixo.

### Observabilidade e Auditoria
- Serilog: manter logs estruturados; reforçar logs de tentativas/sucesso/erro em Auth/Server; manter HealthChecks.

## Alternativa B — BFF (Backend For Frontend) Seguro (Opcional)
- Terminar o fluxo OIDC no Backend, armazenando tokens em **cookies HttpOnly** (mesma origem) e expor APIs ao WASM sem expor tokens ao cliente. 
- Benefícios: menor superfície de ataque (sem tokens no browser), controle de sessão/rotação/refresh centralizados.
- Impactos: remover `AddOidcAuthentication` no WASM e adaptar rotas `login/logout/callback` para o BFF; ajustar CORS para mesma origem.

## Fases de Implementação
- Fase 1: Correções de fluxo e sanitização
  - Auth: sanitizar `returnUrl` com allowlist; `SignInAsync("External")` explícito.
  - Server: alinhar URIs, habilitar caching/passthrough; ajustes de CORS mínimos.
- Fase 2: Hardening de segurança
  - Chaves **assimétricas** em produção; discovery; headers de segurança; DataProtection durável.
- Fase 3: Testes & Observabilidade
  - Unit tests: sanitização de `returnUrl`, handlers de OIDC (claims/destinations), `AuthorizationController`.
  - Integração: fluxo Authorization Code + PKCE com `WebApplicationFactory` e banco de teste.
  - E2E: Playwright para login/callback/logout.
  - Logs/traces: correlação e ProblemDetails nos erros.
- Fase 4 (Opcional): Migrar para **BFF** se requerido.

## Critérios de Aceite
- HTTPS-only; `state`/`nonce` ativos; PKCE requerido.
- Tokens com expiração adequada e refresh controlado.
- UI: estado atualizado pós-callback, funcionalidades liberadas.
- Auditoria: logs completos de tentativas/sucesso/erro; health-checks verdes.

## Referências (OpenIddict)
- `UseLocalServer()` (validação co-local). 
- Discovery remoto + `UseSystemNetHttp()`.
- `EnableAuthorizationRequestCaching()` e `EnableLogoutEndpointPassthrough()`.
- `SetHttpResiliencePipeline(...)` para robustez HTTP.

Confirma seguir a Abordagem A (com opção de BFF na Fase 4)? Ao aprovar, inicio a implementação faseada com testes e documentação.