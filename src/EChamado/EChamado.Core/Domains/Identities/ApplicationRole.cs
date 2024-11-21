using Microsoft.AspNetCore.Identity;

namespace EChamado.Core.Domains.Identities;

public class ApplicationRole : IdentityRole<Guid>
{
    public ICollection<ApplicationUserRole> UserRoles { get; set; } = null!;
    public ICollection<ApplicationRoleClaim> RoleClaims { get; set; } = null!;
}
