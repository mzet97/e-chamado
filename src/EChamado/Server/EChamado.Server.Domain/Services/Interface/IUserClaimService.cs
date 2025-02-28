using EChamado.Server.Domain.Domains.Identities;

namespace EChamado.Server.Domain.Services.Interface;

public interface IUserClaimService
{
    Task AddClaimToUserAsync(ApplicationUser user, string claimType, string claimValue);

    Task RemoveClaimFromUserAsync(ApplicationUser user, string claimType);

    Task<IList<ApplicationUserClaim>> GetUserClaimsAsync(ApplicationUser user);
}
