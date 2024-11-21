using EChamado.Core.Domains.Identities;

namespace EChamado.Core.Services.Interface;

public interface IRoleClaimService
{
    Task AddClaimToRoleAsync(ApplicationRole role, string claimType, string claimValue);

    Task RemoveClaimFromRoleAsync(ApplicationRole role, string claimType);

    Task<IList<ApplicationRoleClaim>> GetRoleClaimsAsync(ApplicationRole role);
}