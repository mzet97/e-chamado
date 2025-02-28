using EChamado.Server.Domain.Domains.Identities;

namespace EChamado.Server.Application.UseCases.Users.ViewModels;

public class ApplicationUserViewModel
{
    public Guid Id { get; set; }
    public string? Photo { get; set; }
    public string? UserName { get; set; }
    public string? NormalizedUserName { get; set; }
    public string? Email { get; set; }
    public string? NormalizedEmail { get; set; }
    public bool EmailConfirmed { get; set; }
    public string? PhoneNumber { get; set; }
    public bool PhoneNumberConfirmed { get; set; }
    public bool TwoFactorEnabled { get; set; }
    public DateTimeOffset? LockoutEnd { get; set; }
    public bool LockoutEnabled { get; set; }
    public int AccessFailedCount { get; set; }

    public ApplicationUserViewModel()
    {
        
    }

    public ApplicationUserViewModel(
        Guid id,
        string? photo,
        string? userName,
        string? normalizedUserName,
        string? email,
        string? normalizedEmail,
        bool emailConfirmed,
        string? phoneNumber,
        bool phoneNumberConfirmed,
        bool twoFactorEnabled,
        DateTimeOffset? lockoutEnd,
        bool lockoutEnabled,
        int accessFailedCount)
    {
        Id = id;
        Photo = photo;
        UserName = userName;
        NormalizedUserName = normalizedUserName;
        Email = email;
        NormalizedEmail = normalizedEmail;
        EmailConfirmed = emailConfirmed;
        PhoneNumber = phoneNumber;
        PhoneNumberConfirmed = phoneNumberConfirmed;
        TwoFactorEnabled = twoFactorEnabled;
        LockoutEnd = lockoutEnd;
        LockoutEnabled = lockoutEnabled;
        AccessFailedCount = accessFailedCount;
    }

    public ApplicationUserViewModel(ApplicationUser user)
    {
        Id = user.Id;
        Photo = user.Photo;
        UserName = user.UserName;
        NormalizedUserName = user.NormalizedUserName;
        Email = user.Email;
        NormalizedEmail = user.NormalizedEmail;
        EmailConfirmed = user.EmailConfirmed;
        PhoneNumber = user.PhoneNumber;
        PhoneNumberConfirmed = user.PhoneNumberConfirmed;
        TwoFactorEnabled = user.TwoFactorEnabled;
        LockoutEnd = user.LockoutEnd;
        LockoutEnabled = user.LockoutEnabled;
        AccessFailedCount = user.AccessFailedCount;
    }
}
