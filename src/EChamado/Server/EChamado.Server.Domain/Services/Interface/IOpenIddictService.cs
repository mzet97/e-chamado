using System.Security.Claims;

namespace EChamado.Server.Domain.Services.Interface;

public interface IOpenIddictService
{
    Task<ClaimsIdentity> LoginOpenIddictAsync(string email, string password);
    Task<ClaimsIdentity> GetClaimsIdentity(string email);
}
