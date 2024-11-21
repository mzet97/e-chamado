using EChamado.Core.Domains.Identities;
using Microsoft.AspNetCore.Identity;

namespace EChamado.Core.Services.Interface;

public interface IUserLoginService
{
    Task AddLoginAsync(ApplicationUser user, UserLoginInfo login);

    Task RemoveLoginAsync(ApplicationUser user, string loginProvider, string providerKey);

    Task<IList<UserLoginInfo>> GetLoginsAsync(ApplicationUser user);

    Task<ApplicationUser?> FindByLoginAsync(string loginProvider, string providerKey);
}
