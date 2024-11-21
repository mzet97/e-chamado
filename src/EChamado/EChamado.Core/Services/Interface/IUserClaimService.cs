using EChamado.Core.Domains.Identities;

namespace EChamado.Core.Services.Interface;

public interface IUserClaimService
{
    Task AddClaimToUserAsync(ApplicationUser user, string claimType, string claimValue);

    Task RemoveClaimFromUserAsync(ApplicationUser user, string claimType);

    Task<IList<ApplicationUserClaim>> GetUserClaimsAsync(ApplicationUser user);
}
