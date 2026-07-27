using EChamado.Server.Application.Users.Abstractions;
using EChamado.Server.Application.Users.Queries;
using Paramore.Brighter;

namespace EChamado.Server.Application.Users.Handlers;

public sealed class GetUserByEmailHandler(IUserReadRepository repository)
    : RequestHandlerAsync<GetUserByEmailQuery>
{
    public override async Task<GetUserByEmailQuery> HandleAsync(
        GetUserByEmailQuery query,
        CancellationToken cancellationToken = default)
    {
        query.Result = await repository.GetByEmailAsync(query.Email, cancellationToken);
        return await base.HandleAsync(query, cancellationToken);
    }
}
