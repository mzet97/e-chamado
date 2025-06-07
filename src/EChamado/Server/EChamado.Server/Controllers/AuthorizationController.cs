using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;
using EChamado.Server.Domain.Services.Interface;

namespace EChamado.Server.Controllers
{
    public class AuthorizationController(
        IOpenIddictApplicationManager applicationManager,
        IOpenIddictService openIddictService
    ) : Controller
    {
        [HttpGet("~/connect/authorize")]
    [HttpPost("~/connect/authorize")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Authorize()
    {
        var request = HttpContext.GetOpenIddictServerRequest()
                      ?? throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

        // Tenta obter o usuário autenticado via cookie
        var result = await HttpContext.AuthenticateAsync();
        if (!result.Succeeded)
        {
            // Se não estiver autenticado, redireciona para a aplicação externa de login (esquema "External")
            return Challenge(
                authenticationSchemes: new[] { "External" },
                properties: new AuthenticationProperties
                {
                    RedirectUri = Request.PathBase + Request.Path +
                                  QueryString.Create(Request.HasFormContentType
                                      ? Request.Form.ToList()
                                      : Request.Query.ToList())
                });
        }

        // Se autenticado, cria claims principal para gerar authorization code
        var claims = new List<Claim>
            {
                new Claim(Claims.Subject, result.Principal.Identity.Name),
                new Claim(Claims.Email, result.Principal.FindFirst(Claims.Email)?.Value ?? string.Empty),
                new Claim(Claims.Name, result.Principal.Identity.Name ?? string.Empty)
            };

        var claimsIdentity = new ClaimsIdentity(claims, TokenValidationParameters.DefaultAuthenticationType);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        // Seta os escopos solicitados
        claimsPrincipal.SetScopes(request.GetScopes());

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

            identity.SetDestinations(claim => claim.Type switch
            {
                Claims.Name or Claims.Email when claim.Subject.HasScope(Scopes.Profile) =>
                    new[] { Destinations.AccessToken, Destinations.IdentityToken },
                Claims.Role => new[] { Destinations.AccessToken },
                _ => new[] { Destinations.AccessToken }
            });

            return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        if (request.IsAuthorizationCodeGrantType())
        {
            var principal = (await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)).Principal;
            principal.SetDestinations(claim => claim.Type switch
            {
                Claims.Name or Claims.Email when principal.HasScope(Scopes.Profile) =>
                    new[] { Destinations.AccessToken, Destinations.IdentityToken },
                Claims.Role => new[] { Destinations.AccessToken },
                _ => new[] { Destinations.AccessToken }
            });

            return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        throw new NotImplementedException("The specified grant is not implemented.");
    }
}
}
