using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using static OpenIddict.Abstractions.OpenIddictConstants;
using EChamado.Server.Domain.Services.Interface;
using Microsoft.AspNetCore.Identity;
using EChamado.Server.Domain.Domains.Identities;

namespace EChamado.Server.Controllers
{
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
            var request = HttpContext.GetOpenIddictServerRequest()
                          ?? throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

            logger.LogInformation("Authorization request received. Client: {ClientId}, RedirectUri: {RedirectUri}, Scope: {Scope}, ResponseType: {ResponseType}, State: {State}, CodeChallenge: {CodeChallenge}",
                request.ClientId, request.RedirectUri, request.Scope, request.ResponseType, request.State, request.CodeChallenge);

            // Tenta obter o usuário autenticado via cookie "External"
            var result = await HttpContext.AuthenticateAsync("External");
            if (!result.Succeeded)
            {
                logger.LogInformation("User not authenticated via External cookie. Redirecting to login.");

                // Se não estiver autenticado, redireciona para a aplicação externa de login
                var redirectUri = Request.PathBase + Request.Path +
                                  QueryString.Create(Request.HasFormContentType
                                      ? Request.Form.ToList()
                                      : Request.Query.ToList());

                logger.LogInformation("Redirect URI for login: {RedirectUri}", redirectUri);

                return Challenge(
                    authenticationSchemes: new[] { "External" },
                    properties: new AuthenticationProperties
                    {
                        RedirectUri = redirectUri
                    });
            }

            logger.LogInformation("User authenticated via External cookie. UserId: {UserId}",
                result.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            // Busca o usuário completo do Identity
            var userId = result.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Challenge(authenticationSchemes: new[] { "External" });
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Challenge(authenticationSchemes: new[] { "External" });
            }

            // Busca roles do usuário
            var roles = await userManager.GetRolesAsync(user);

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

            logger.LogInformation("Generating authorization code. Will redirect to: {RedirectUri}", request.RedirectUri);

            // SignIn retorna um código de autorização e redireciona automaticamente para request.RedirectUri
            return SignIn(claimsPrincipal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        [HttpPost("~/connect/token"), Produces("application/json")]
        public async Task<IActionResult> Exchange()
        {
            var request = HttpContext.GetOpenIddictServerRequest();

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
                var identity = await openIddictService.LoginOpenIddictAsync(request.Username, request.Password);
                if (identity == null)
                {
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

                return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }

            if (request.IsAuthorizationCodeGrantType())
            {
                var authenticateResult = await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

                if (!authenticateResult.Succeeded || authenticateResult.Principal == null)
                {
                    return Forbid(
                        authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                        properties: new AuthenticationProperties(new Dictionary<string, string>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The authorization code is no longer valid."
                        }));
                }

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

            throw new NotImplementedException("The specified grant is not implemented.");
        }
    }
}
