using EChamado.Server.Application.Common.Behaviours;
using EChamado.Server.Domain.Domains.Identities;
using EChamado.Server.Domain.Services.Interface;
using EChamado.Shared.Responses;
using EChamado.Shared.Shared.Settings;
using EChamado.Shared.ViewModels.Auth;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Paramore.Brighter;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EChamado.Server.Application.UseCases.Auth.Commands.Handlers;

public class GetTokenCommandHandler(
    IOptions<AppSettings> appSettings,
    IApplicationUserService applicationUserService
    ) : RequestHandlerAsync<GetTokenCommand>
{
    [RequestLogging(0, HandlerTiming.Before)]
    [RequestValidation(1, HandlerTiming.Before)]
    public override async Task<GetTokenCommand> HandleAsync(GetTokenCommand command, CancellationToken cancellationToken = default)
    {
        var result = await GetJwt(command.Email);
        command.Result = result;
        return await base.HandleAsync(command, cancellationToken);
    }

    public async Task<BaseResult<LoginResponseViewModel>> GetJwt(string email)
    {
        var user = await applicationUserService.FindByEmailAsync(email);

        if (user == null) {
            return new BaseResult<LoginResponseViewModel>(
                null,
                false,
                "Invalid user");
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

        var identityClaims = new ClaimsIdentity(claims.Select(x => x.ToClaim()));

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(appSettings.Value.Secret);
        var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
        {
            Issuer = appSettings.Value.Issuer,
            Audience = appSettings.Value.ValidOn,
            Subject = identityClaims,
            Expires = DateTime.UtcNow.AddHours(appSettings.Value.ExpirationHours),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        });

        var encodedToken = tokenHandler.WriteToken(token);

        var userToken = new UserTokenViewModel(user.Id.ToString(), user.Email, claims.Select(x => x.ToClaim()).ToList());

        var response = new LoginResponseViewModel
        {
            AccessToken = encodedToken,
            ExpiresIn = TimeSpan.FromHours(appSettings.Value.ExpirationHours).TotalSeconds,
            UserToken = userToken
        };

        return new BaseResult<LoginResponseViewModel>(response);
    }


    private static long ToUnixEpochDate(DateTime date)
       => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);
}
