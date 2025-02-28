using EChamado.Server.Domain.Domains.Identities;

namespace EChamado.Server.Domain.Services.Interface;

public interface IRoleClaimService
{
    Task AddClaimToRoleAsync(ApplicationRole role, string claimType, string claimValue);

    Task RemoveClaimFromRoleAsync(ApplicationRole role, string claimType);

    Task<IList<ApplicationRoleClaim>> GetRoleClaimsAsync(ApplicationRole role);
}