using EChamado.Server.Domain.Domains.Identities;
using EChamado.Server.Domain.Services.Interface;
using OpenIddict.Server.AspNetCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace EChamado.Server.Application.Services;

public class OpenIddictService(
     IApplicationUserService applicationUserService) : 
    IOpenIddictService
{
    public async Task<ClaimsIdentity> LoginOpenIddictAsync(string email, string password)
{
    var result = await applicationUserService.PasswordSignInAsync(email, password, false, false);

    if (result.Succeeded)
    {
        return await GetClaimsIdentity(email);
    }

    return null;
}

public async Task<ClaimsIdentity> GetClaimsIdentity(string email)
{
    var user = await applicationUserService.FindByEmailAsync(email);

    if (user == null)
    {
        return null;
    }

    var claims = await applicationUserService.GetClaimsAsync(user);
    var userRoles = await applicationUserService.GetRolesAsync(user);

    claims.Add(ApplicationUserClaim.Create(JwtRegisteredClaimNames.Sub, user.Id.ToString()));
    claims.Add(ApplicationUserClaim.Create(JwtRegisteredClaimNames.Email, user.Email));
    claims.Add(ApplicationUserClaim.Create(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
    claims.Add(ApplicationUserClaim.Create(JwtRegisteredClaimNames.Nbf, ToUnixEpochDate(DateTime.UtcNow).ToString()));
    claims.Add(ApplicationUserClaim.Create(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64));

    foreach (var userRole in userRoles)
    {
        claims.Add(ApplicationUserClaim.Create("role", userRole));
    }

    var identityClaims = new ClaimsIdentity(
         claims.Select(x => x.ToClaim()),
         OpenIddictServerAspNetCoreDefaults.AuthenticationScheme // Define um AuthenticationType
     );

    return identityClaims;
}

public static long ToUnixEpochDate(DateTime date)
   => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);
}
