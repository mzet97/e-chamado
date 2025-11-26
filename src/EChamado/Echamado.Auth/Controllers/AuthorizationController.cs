using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using static OpenIddict.Abstractions.OpenIddictConstants;
using Microsoft.AspNetCore.Identity;
using EChamado.Server.Domain.Domains.Identities;
using EChamado.Server.Domain.Services.Interface;

namespace Echamado.Auth.Controllers;

public class AuthorizationController(
    IOpenIddictApplicationManager applicationManager,
    IOpenIddictService openIddictService,
    UserManager<ApplicationUser> userManager,
    ILogger<AuthorizationController> logger
) : Controller
{
    [HttpGet("~/connect/authorize")]
    [HttpPost("~/connect/authorize")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Authorize()
    {
        logger.LogInformation("🔵 /connect/authorize CALLED - Method: {Method}, QueryString: {Query}",
            Request.Method, Request.QueryString.Value);

        var request = HttpContext.GetOpenIddictServerRequest()
                              ?? throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

        logger.LogInformation("✅ OpenIddict request parsed. Client: {ClientId}, RedirectUri: {RedirectUri}, Scope: {Scope}, ResponseType: {ResponseType}, State: {State}, CodeChallenge: {CodeChallenge}, CodeChallengeMethod: {CodeChallengeMethod}",
            request.ClientId, request.RedirectUri, request.Scope, request.ResponseType, request.State, request.CodeChallenge, request.CodeChallengeMethod);

        // Tenta obter o usuário autenticado via cookie "External"
        var result = await HttpContext.AuthenticateAsync("External");
        if (!result.Succeeded)
        {
            logger.LogInformation("❌ User NOT authenticated via External cookie. Redirecting to login.");

            // Se não estiver autenticado, redireciona para a aplicação externa de login
            var redirectUri = Request.PathBase + Request.Path +
                              QueryString.Create(Request.HasFormContentType
                                  ? Request.Form.ToList()
                                  : Request.Query.ToList());

            logger.LogInformation("🔀 Redirect URI for login: {RedirectUri}", redirectUri);

            return Challenge(
                authenticationSchemes: new[] { "External" },
                properties: new AuthenticationProperties
                {
                    RedirectUri = redirectUri
                });
        }

        logger.LogInformation("✅ User authenticated via External cookie. UserId: {UserId}",
            result.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        // Busca o usuário completo do Identity
        var userId = result.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            logger.LogWarning("❌ UserId not found in External cookie claims");
            return Challenge(authenticationSchemes: new[] { "External" });
        }

        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            logger.LogWarning("❌ User not found in database: {UserId}", userId);
            return Challenge(authenticationSchemes: new[] { "External" });
        }

        logger.LogInformation("✅ User found: {Email}", user.Email);

        // Busca roles do usuário
        var roles = await userManager.GetRolesAsync(user);
        logger.LogInformation("✅ User roles: {Roles}", string.Join(", ", roles));

        // Cria claims principal para gerar authorization code
        var claims = new List<Claim>
        {
            new Claim(Claims.Subject, user.Id.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(Claims.Email, user.Email ?? string.Empty),
            new Claim(Claims.Name, user.UserName ?? string.Empty),
            new Claim(Claims.PreferredUsername, user.UserName ?? string.Empty)
        };

        // Adiciona roles como claims
        foreach (var role in roles)
        {
            claims.Add(new Claim(Claims.Role, role));
        }

        var claimsIdentity = new ClaimsIdentity(
            claims,
            TokenValidationParameters.DefaultAuthenticationType,
            Claims.Name,
            Claims.Role);

        // Define os destinos dos claims na ClaimsIdentity
        claimsIdentity.SetDestinations(claim => claim.Type switch
        {
            Claims.Name or Claims.Email => new[] { Destinations.AccessToken, Destinations.IdentityToken },
            Claims.Role => new[] { Destinations.AccessToken },
            Claims.Subject => new[] { Destinations.AccessToken, Destinations.IdentityToken },
            ClaimTypes.NameIdentifier => new[] { Destinations.AccessToken, Destinations.IdentityToken },
            Claims.PreferredUsername => new[] { Destinations.AccessToken, Destinations.IdentityToken },
            _ => new[] { Destinations.AccessToken }
        });

        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        // Seta os escopos solicitados
        claimsPrincipal.SetScopes(request.GetScopes());

        logger.LogInformation("🎯 Generating authorization CODE. Will redirect to: {RedirectUri} with code in URL",
            request.RedirectUri);
        logger.LogInformation("📋 Claims count: {Count}, Scopes: {Scopes}",
            claims.Count, string.Join(", ", request.GetScopes()));

        logger.LogInformation("✅ About to call SignIn to generate authorization code...");

        // SignIn retorna um código de autorização e redireciona automaticamente para request.RedirectUri
        return SignIn(claimsPrincipal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    [HttpPost("~/connect/token"), Produces("application/json")]
    public async Task<IActionResult> Exchange()
    {
        var request = HttpContext.GetOpenIddictServerRequest()
                              ?? throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

        logger.LogInformation("Token exchange request received. Grant Type: {GrantType}, Client: {ClientId}, Scope: {Scope}",
            request.GrantType, request.ClientId, request.Scope);

        if (request.IsClientCredentialsGrantType())
        {
            var application = await applicationManager.FindByClientIdAsync(request.ClientId)
                              ?? throw new InvalidOperationException("The application cannot be found.");

            var identity = new ClaimsIdentity(TokenValidationParameters.DefaultAuthenticationType, Claims.Name, Claims.Role);
            identity.SetClaim(Claims.Subject, await applicationManager.GetClientIdAsync(application));
            identity.SetClaim(Claims.Name, await applicationManager.GetDisplayNameAsync(application));

            identity.SetDestinations(claim => claim.Type switch
            {
                Claims.Name when claim.Subject.HasScope(Scopes.Profile) =>
                    new[] { Destinations.AccessToken, Destinations.IdentityToken },
                _ => new[] { Destinations.AccessToken }
            });

            return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        if (request.IsPasswordGrantType())
        {
            logger.LogInformation("Processing password grant for user: {Username}", request.Username);

            var identity = await openIddictService.LoginOpenIddictAsync(request.Username, request.Password);
            if (identity == null)
            {
                logger.LogWarning("Password grant failed for user: {Username}", request.Username);
                return Forbid(
                    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(new Dictionary<string, string>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The username/password couple is invalid."
                    }));
            }

            // SetDestinations na identity ANTES de criar o principal
            identity.SetDestinations(claim => claim.Type switch
            {
                Claims.Name or Claims.Email => new[] { Destinations.AccessToken, Destinations.IdentityToken },
                Claims.Role => new[] { Destinations.AccessToken },
                JwtRegisteredClaimNames.Sub => new[] { Destinations.AccessToken, Destinations.IdentityToken },
                _ => new[] { Destinations.AccessToken }
            });

            var principal = new ClaimsPrincipal(identity);
            principal.SetScopes(request.GetScopes());

            logger.LogInformation("Password grant successful for user: {Username}", request.Username);

            return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        if (request.IsAuthorizationCodeGrantType())
        {
            logger.LogInformation("🔄 Processing Authorization Code grant. Code: {Code}, CodeVerifier: {Verifier}",
                request.Code?.Length > 0 ? $"{request.Code.Substring(0, Math.Min(10, request.Code.Length))}..." : "MISSING",
                request.CodeVerifier?.Length > 0 ? $"{request.CodeVerifier.Substring(0, Math.Min(10, request.CodeVerifier.Length))}..." : "MISSING");

            var authenticateResult = await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

            if (!authenticateResult.Succeeded || authenticateResult.Principal == null)
            {
                logger.LogWarning("❌ Authorization code authentication FAILED");
                return Forbid(
                    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(new Dictionary<string, string>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The authorization code is no longer valid."
                    }));
            }

            logger.LogInformation("✅ Authorization code validated successfully");

            var principal = authenticateResult.Principal;

            // Criar novo principal com claims destinations configurados
            var identity = (ClaimsIdentity)principal.Identity!;
            identity.SetDestinations(claim => claim.Type switch
            {
                Claims.Name or Claims.Email => new[] { Destinations.AccessToken, Destinations.IdentityToken },
                Claims.Role => new[] { Destinations.AccessToken },
                Claims.Subject => new[] { Destinations.AccessToken, Destinations.IdentityToken },
                ClaimTypes.NameIdentifier => new[] { Destinations.AccessToken, Destinations.IdentityToken },
                Claims.PreferredUsername => new[] { Destinations.AccessToken, Destinations.IdentityToken },
                _ => new[] { Destinations.AccessToken }
            });

            return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        if (request.IsRefreshTokenGrantType())
        {
            // Recupera o principal do refresh token
            var principal = (await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)).Principal;

            if (principal == null)
            {
                return Forbid(
                    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(new Dictionary<string, string>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The refresh token is no longer valid."
                    }));
            }

            // Busca o usuário para garantir que ainda existe e está ativo
            var userId = principal.FindFirst(Claims.Subject)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                var user = await userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return Forbid(
                        authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                        properties: new AuthenticationProperties(new Dictionary<string, string>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The user no longer exists."
                        }));
                }
            }

            // ✅ Define os destinos dos claims na Identity ANTES do SignIn
            var identity = (ClaimsIdentity)principal.Identity!;
            identity.SetDestinations(claim => claim.Type switch
            {
                Claims.Name or Claims.Email =>
                    new[] { Destinations.AccessToken, Destinations.IdentityToken },
                Claims.Role => new[] { Destinations.AccessToken },
                Claims.Subject => new[] { Destinations.AccessToken, Destinations.IdentityToken },
                ClaimTypes.NameIdentifier => new[] { Destinations.AccessToken, Destinations.IdentityToken },
                _ => new[] { Destinations.AccessToken }
            });

            return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        logger.LogWarning("Token exchange request failed. Unsupported grant type: {GrantType}", request.GrantType);
        throw new NotImplementedException("The specified grant is not implemented.");
    }

    [HttpPost("~/connect/introspect"), Produces("application/json")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Introspect()
    {
        var request = HttpContext.GetOpenIddictServerRequest()
                              ?? throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

        logger.LogInformation("Introspection request received from client: {ClientId}", request.ClientId);

        // Valida o cliente que está fazendo a introspecção (API Server)
        var application = await applicationManager.FindByClientIdAsync(request.ClientId);
        if (application == null)
        {
            logger.LogWarning("Introspection request from unknown client: {ClientId}", request.ClientId);
            return Forbid(
                authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                properties: new AuthenticationProperties(new Dictionary<string, string>
                {
                    [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidClient,
                    [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The client application was not found."
                }));
        }

        // Verifica se o cliente tem permissão para usar introspecção
        if (!await applicationManager.HasPermissionAsync(application,
            OpenIddict.Abstractions.OpenIddictConstants.Permissions.Endpoints.Introspection))
        {
            logger.LogWarning("Introspection denied for client: {ClientId} - missing permission", request.ClientId);
            return Forbid(
                authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                properties: new AuthenticationProperties(new Dictionary<string, string>
                {
                    [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.UnauthorizedClient,
                    [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "This client is not allowed to use introspection."
                }));
        }

        // O OpenIddict processa automaticamente o token e retorna o resultado via SignIn
        // Apenas retornamos sucesso e o OpenIddict cuida do resto
        var principal = (await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)).Principal;

        if (principal == null)
        {
            logger.LogInformation("Introspection: token is not active");
            return Ok(new { active = false });
        }

        logger.LogInformation("Introspection successful for subject: {Subject}",
            principal.FindFirst(Claims.Subject)?.Value);

        return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }
}