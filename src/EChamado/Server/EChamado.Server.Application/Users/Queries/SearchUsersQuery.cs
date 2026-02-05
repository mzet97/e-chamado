using EChamado.Server.Application.Common.Messaging;

namespace EChamado.Server.Application.Users.Queries;

public sealed class SearchUsersQuery : BrighterRequest<PagedResult<UserListItemDto>>
{
    public UserSearchFilter Filter { get; init; }

    public SearchUsersQuery(UserSearchFilter filter)
    {
        Filter = filter;
    }
}
