using EChamado.Server.Application.Users.Abstractions;
using EChamado.Server.Application.Users.Queries;
using Paramore.Brighter;

namespace EChamado.Server.Application.Users.Handlers;

public sealed class SearchUsersHandler(IUserReadRepository repository)
    : RequestHandlerAsync<SearchUsersQuery>
{
    public override async Task<SearchUsersQuery> HandleAsync(
        SearchUsersQuery query,
        CancellationToken cancellationToken = default)
    {
        query.Filter.Normalize();
        query.Result = await repository.SearchAsync(query.Filter, cancellationToken);
        return await base.HandleAsync(query, cancellationToken);
    }
}
