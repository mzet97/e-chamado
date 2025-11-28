using Microsoft.AspNetCore.Identity;

namespace EChamado.Server.Domain.Domains.Identities;

public class ApplicationUser : IdentityUser<Guid>
{
    public ApplicationUser()
    {
        Id = Guid.NewGuid();
        SecurityStamp = Guid.NewGuid().ToString();
        Claims = new HashSet<ApplicationUserClaim>();
        UserRoles = new HashSet<ApplicationUserRole>();
        Logins = new HashSet<ApplicationUserLogin>();
        Tokens = new HashSet<ApplicationUserToken>();
    }

    public string? Photo { get; set; }
    public string? FullName { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public ICollection<ApplicationUserClaim> Claims { get; set; }
    public ICollection<ApplicationUserRole> UserRoles { get; set; }
    public ICollection<ApplicationUserLogin> Logins { get; set; }
    public ICollection<ApplicationUserToken> Tokens { get; set; }
}
