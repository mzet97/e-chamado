using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;
using System.Security.Claims;
using EChamado.Server.Domain.Services.Interface;
using Microsoft.AspNetCore.Authentication;

namespace EChamado.Server.Controllers;

public class AuthorizationController(
    IOpenIddictApplicationManager applicationManager,
    IOpenIddictService openIddictService
    ) : Controller
{

    [HttpPost("~/connect/token"), Produces("application/json")]
    public async Task<IActionResult> Exchange()
    {
        var request = HttpContext.GetOpenIddictServerRequest();
        if (request.IsClientCredentialsGrantType())
        {
            // Note: the client credentials are automatically validated by OpenIddict:
            // if client_id or client_secret are invalid, this action won't be invoked.

            var application = await applicationManager.FindByClientIdAsync(request.ClientId) ??
                throw new InvalidOperationException("The application cannot be found.");

            // Create a new ClaimsIdentity containing the claims that
            // will be used to create an id_token, a token or a code.
            var identity = new ClaimsIdentity(TokenValidationParameters.DefaultAuthenticationType, Claims.Name, Claims.Role);

            // Use the client_id as the subject identifier.
            identity.SetClaim(Claims.Subject, await applicationManager.GetClientIdAsync(application));
            identity.SetClaim(Claims.Name, await applicationManager.GetDisplayNameAsync(application));

            identity.SetDestinations(static claim => claim.Type switch
            {
                // Allow the "name" claim to be stored in both the access and identity tokens
                // when the "profile" scope was granted (by calling principal.SetScopes(...)).
                Claims.Name when claim.Subject.HasScope(Scopes.Profile)
                    => [Destinations.AccessToken, Destinations.IdentityToken],

                // Otherwise, only store the claim in the access tokens.
                _ => [Destinations.AccessToken]
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
                Claims.Name or Claims.Email when claim.Subject.HasScope(Scopes.Profile) => [Destinations.AccessToken, Destinations.IdentityToken],
                Claims.Role => [Destinations.AccessToken],
                _ => [Destinations.AccessToken]
            });

            return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

        }

        throw new NotImplementedException("The specified grant is not implemented.");
    }
}
