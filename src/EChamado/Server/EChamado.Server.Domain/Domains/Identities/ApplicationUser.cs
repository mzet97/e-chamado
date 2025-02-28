using Microsoft.AspNetCore.Identity;

namespace EChamado.Server.Domain.Domains.Identities;

public class ApplicationUser : IdentityUser<Guid>
{
    public string? Photo { get; set; }

    public ICollection<ApplicationUserClaim> Claims { get; set; } = null!;
    public ICollection<ApplicationUserRole> UserRoles { get; set; } = null!;
    public ICollection<ApplicationUserLogin> Logins { get; set; } = null!;
    public ICollection<ApplicationUserToken> Tokens { get; set; } = null!;
}
