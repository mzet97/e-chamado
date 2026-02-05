namespace EChamado.Server.Application.Users.Abstractions;

public interface IUserReadRepository
{
    Task<UserDetailsDto?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    Task<PagedResult<UserListItemDto>> SearchAsync(UserSearchFilter filter, CancellationToken cancellationToken);
}
