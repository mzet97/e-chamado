using System.Security.Claims;

namespace EChamado.Application.Features.Auth.ViewModels;

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
