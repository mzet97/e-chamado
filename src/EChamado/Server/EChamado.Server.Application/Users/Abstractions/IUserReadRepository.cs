using EChamado.Server.Application.Users;

namespace EChamado.Server.Application.Users.Abstractions;

public interface IUserReadRepository
{
    Task<UserDetailsDto?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    Task<UserDetailsDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<PagedResult<UserListItemDto>> SearchAsync(UserSearchFilter filter, CancellationToken cancellationToken);
}
