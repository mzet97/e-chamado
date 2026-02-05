## Diagnóstico Atual
- Client (Blazor WASM): usa `RemoteAuthenticatorView` e `AddOidcAuthentication` com `Authority=https://localhost:7296` e `ClientId=bwa-client` (`Client/wwwroot/appsettings.json:1–10`).
- Server (API + OIDC): OpenIddict Server/Validation configurados com `Authorization`/`Token` endpoints e `UseLocalServer` para validação (`Server.Infrastructure/Configuration/IdentityConfig.cs:140–177`). CORS libera `https://localhost:7274` e `https://localhost:7132` (`Server/Program.cs:25–37`).
- Auth (Blazor Server UI): página `Account/Login` coleta credenciais e envia para `Account/DoLogin` (`Echamado.Auth/Components/Pages/Accounts/Login.razor:18–45`). Redireciona ao `returnUrl` e agora emite cookie no esquema "External" (`Echamado.Auth/Controllers/AccountController.cs:24–79`).
- Fluxo esperado: Client → OIDC (`/connect/authorize`) → desafio para Auth UI → login → volta ao OIDC → code/token → callback no Client.

## Problemas/Fragilidades Identificados
- **Assinatura Simétrica** no OIDC: `AddSigningKey(new SymmetricSecurityKey(key))` (de `AppSettings.Secret`) em `IdentityConfig.cs:156–158`: bom para dev, não recomendado em prod; preferir chaves assimétricas e discovery.
- **Retorno relativo** do `returnUrl`: exigia construção de URL absoluta para o OIDC; já coberto no Auth, mas validar contra open redirect.
- **Persistência de DataProtection**: armazena em `Path.GetTempPath()` (dev ok); em prod usar storage durável (file share/Key Vault).
- **PostLogoutRedirectUri divergente** (ajustado no Server `OpenIddictWorker.cs:81–84,113–118`), garantir consistência também no Client.
- **Tokens e refresh em WASM**: confirmar suporte a refresh e storage seguro (evitar localStorage quando possível; preferir in-memory/cookies HttpOnly via BFF, se for requisito).

## Referências de Boas Práticas (OpenIddict)
- Validação com servidor local: `UseLocalServer()` [OpenIddict docs].
- Discovery/assinatura assimétrica: `SetIssuer(...)` + chaves assimétricas [OpenIddict docs].
- Proteções de fluxo: `state`/`nonce` (o `RemoteAuthenticatorView` e o provider OIDC cuidam disso). 

## Abordagem Proposta (A) — Corrigir e Fortalecer a Implementação Atual
1. **OIDC Server (7296)**
   - Produção: trocar **assinatura simétrica** por **certificados** (já há `AddDevelopmentSigningCertificate` para dev). Manter `EnableAuthorization/TokenEndpointPassthrough` conforme necessário (`IdentityConfig.cs:167–170`).
   - Validar escopos e URIs dos clientes em `OpenIddictWorker.cs:37–121` (bwa-client, PKCE habilitado).
   - Ativar features: `EnableAuthorizationRequestCaching()` e `EnableLogoutEndpointPassthrough()` se o payload for grande.

2. **Auth UI (7132)**
   - Manter cookie `External` e **sign-in explícito** pós-login (`AccountController.cs:44–55`).
   - Validar `returnUrl` com **whitelist** de host/porta/path e construir absoluto quando relativo (`AccountController.cs:95–128`).
   - Garantir antiforgery no form (`Program.cs:88` já habilitado) e HTTPS.

3. **Client (7274)**
   - Confirmar que `RemoteAuthenticatorView` está configurado para **Authorization Code + PKCE** e que `RedirectUri`/`PostLogoutRedirectUri` batem com o Server (`Client/wwwroot/appsettings.json`).
   - Opcional: inicialização antecipada do status de login (verificar `AuthenticationState` inicial) e fallback para exibir botão login quando não autenticado (`Shared/LoginDisplay.razor`).

4. **Segurança Complementar**
   - Headers: adicionar CSP, HSTS, X-Content-Type-Options, etc. via middleware no Server.
   - DataProtection: mover storage fora de `TempPath` em produção.
   - Logs de auditoria: reforçar logs estruturados nas tentativas de login e passos do fluxo (já há Serilog). 

5. **Performance & Escala**
   - Resilience HTTP para discovery/validação (OpenIddict `UseSystemNetHttp().SetHttpResiliencePipeline(...)`).
   - Evitar roundtrips desnecessários; cache de metadata (authorization request caching).

## Alternativa (B) — BFF Minimalista para Storage Seguro
- Introduzir um pequeno BFF no Server para terminar o fluxo OIDC no servidor, armazenar tokens em **cookies HttpOnly** e expor APIs autenticadas ao WASM via mesma origem. Elimina exposição de tokens no lado cliente e facilita **rotacionar/renovar** tokens com refresh controlado no backend.
- Implica adaptar rotas de login/logout/callback para o BFF e remover `AddOidcAuthentication` do WASM.

## Implementação Planejada (Faseada)
- **Fase 1 — Correções OIDC/Redirecionamento**
  - Auth: manter e endurecer validação de `returnUrl`, sign-in no esquema `External`.
  - Server: revisar URIs/escopos e ajustar `PostLogoutRedirectUri`.

- **Fase 2 — Fortalecimento de Segurança**
  - Trocar assinatura para **certificados** em produção; ativar discovery.
  - Middleware de **security headers** e hardening de CORS.
  - Persistir DataProtection em storage durável.

- **Fase 3 — Testes & Observabilidade**
  - Unit tests: `AccountController` (retorno absoluto/relativo, open redirect), `AuthorizationController` (claims/destinations), validadores.
  - Integração: fluxo Authorization Code + PKCE com `WebApplicationFactory` e banco de teste.
  - E2E: Playwright cobrindo login, callback, logout.
  - Logs/traces: confirmar correlação e ProblemDetails nos erros.

- **Fase 4 — (Opcional) BFF**
  - Se requerido por política, migrar para BFF com cookies HttpOnly e callbacks no backend.

## Entregáveis
- Código refatorado (Server/Auth/Client) com padrões OIDC.
- Testes unitários e de integração; cenários críticos E2E.
- Observabilidade (Serilog + HealthChecks + traces) ajustada.
- Documentação técnica: fluxo, segurança, operação.

## Validações
- HTTPS-only, `state`/`nonce` ativos, PKCE requerido.
- Tokens com expiração adequada e refresh controlado.
- Auditoria: logs de tentativas/sucesso/erro.

Confirma este plano (A com opção BFF), ou prefere partirmos diretamente para a alternativa B (BFF) mais segura em produção? Com sua confirmação, inicio a implementação faseada e os testes.