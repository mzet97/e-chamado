using EChamado.Server.Domain.Domains.Identities;
using EChamado.Server.Domain.Services.Interface;
using EChamado.Server.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EChamado.Server.Infrastructure.Services;

public sealed class UserTokenService : IUserTokenService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;

    public UserTokenService(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
    {
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task AddUserTokenAsync(ApplicationUserToken userToken)
    {
        if (userToken == null) throw new ArgumentNullException(nameof(userToken));

        await _context.UserTokens.AddAsync(userToken);
        await _context.SaveChangesAsync();
    }

    public async Task<ApplicationUserToken?> GetUserTokenAsync(Guid userId, string loginProvider, string name)
    {
        if (string.IsNullOrWhiteSpace(loginProvider)) throw new ArgumentException("Login provider cannot be nulo.", nameof(loginProvider));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Token name cannot be nulo.", nameof(name));

        return await _context.UserTokens
            .FirstOrDefaultAsync(ut => ut.UserId == userId && ut.LoginProvider == loginProvider && ut.Name == name);
    }

    public async Task RemoveUserTokenAsync(ApplicationUserToken userToken)
    {
        if (userToken == null) throw new ArgumentNullException(nameof(userToken));

        _context.UserTokens.Remove(userToken);
        await _context.SaveChangesAsync();
    }
}
