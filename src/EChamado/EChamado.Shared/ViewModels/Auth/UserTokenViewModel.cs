using System.Security.Claims;

namespace EChamado.Shared.ViewModels.Auth;

public class UserTokenViewModel
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public IEnumerable<ClaimViewModel> Claims { get; set; } = Enumerable.Empty<ClaimViewModel>();

    public UserTokenViewModel()
    {

    }

    public UserTokenViewModel(string id, string email, IList<Claim> claims)
    {
        Id = id;
        Email = email;
        Claims = claims.Select(c => new ClaimViewModel(c));
    }
}
