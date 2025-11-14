using EChamado.Server.Application.UseCases.Users.ViewModels;
using EChamado.Server.Domain.Services.Interface;
using EChamado.Shared.Responses;
using Paramore.Brighter;

namespace EChamado.Server.Application.UseCases.Users.Queries.Handlers;

public class GetAllUsersQueryHandler(IApplicationUserService applicationUserService) :
    RequestHandlerAsync<GetAllUsersQuery>
{
    public override async Task<GetAllUsersQuery> HandleAsync(
        GetAllUsersQuery query,
        CancellationToken cancellationToken = default)
    {
        var users = await applicationUserService.GetAllUsersAsync();

        query.Result = new BaseResultList<ApplicationUserViewModel>(users
            .Select(x => new ApplicationUserViewModel(x)),
            null,
            true,
            "Obtido com sucesso");

        return await base.HandleAsync(query, cancellationToken);
    }
}
