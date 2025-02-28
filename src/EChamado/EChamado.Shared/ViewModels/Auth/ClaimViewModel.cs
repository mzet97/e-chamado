using System.Security.Claims;

namespace EChamado.Shared.ViewModels.Auth;

public class ClaimViewModel
{
    public string Value { get; set; }
    public string Type { get; set; }


    public ClaimViewModel(Claim claim)
    {
        Value = claim.Value;
        Type = claim.Type;
    }
}
