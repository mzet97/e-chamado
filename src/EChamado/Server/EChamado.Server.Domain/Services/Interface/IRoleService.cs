﻿using EChamado.Server.Domain.Domains.Identities;
using Microsoft.AspNetCore.Identity;

namespace EChamado.Server.Domain.Services.Interface;

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
