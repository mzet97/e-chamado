using EChamado.Core.Domains.Identities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EChamado.Core.Services.Interface;

public interface IRoleService
{
    Task<IdentityResult> CreateRoleAsync(ApplicationRole role);

    Task<ApplicationRole?> GetRoleByIdAsync(Guid roleId);

    Task<ApplicationRole?> GetRoleByNameAsync(string roleName);

    Task<IEnumerable<ApplicationRole>> GetAllRolesAsync();

    Task<IdentityResult> UpdateRoleAsync(ApplicationRole role);

    Task<IdentityResult> DeleteRoleAsync(Guid roleId);

    Task<bool> RoleExistsAsync(string roleName);
}
