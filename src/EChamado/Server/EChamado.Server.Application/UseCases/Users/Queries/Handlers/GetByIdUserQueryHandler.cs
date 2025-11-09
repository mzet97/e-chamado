using EChamado.Server.Application.UseCases.Users.ViewModels;
using EChamado.Server.Domain.Services.Interface;
using EChamado.Shared.Responses;
using Paramore.Brighter;

namespace EChamado.Server.Application.UseCases.Users.Queries.Handlers;

public class GetByIdUserQueryHandler(IApplicationUserService applicationUserService) :
    RequestHandlerAsync<GetByIdUserQuery>
{
    public override async Task<GetByIdUserQuery> HandleAsync(GetByIdUserQuery query, CancellationToken cancellationToken = default)
    {
        var users = await applicationUserService.FindByIdAsync(query.Id);

        query.Result = new BaseResult<ApplicationUserViewModel>(
            new ApplicationUserViewModel(users),
            true,
            "Obtido com sucesso");

        return await base.HandleAsync(query, cancellationToken);
    }
}
