using EChamado.Server.Application.UseCases.Users.ViewModels;
using EChamado.Server.Domain.Services.Interface;
using EChamado.Shared.Responses;
using Paramore.Brighter;

namespace EChamado.Server.Application.UseCases.Users.Queries.Handlers;

public class GetByEmailUserQueryHandler(IApplicationUserService applicationUserService) :
    RequestHandlerAsync<GetByEmailUserQuery>
{
    public override async Task<GetByEmailUserQuery> HandleAsync(
        GetByEmailUserQuery query,
        CancellationToken cancellationToken = default)
    {
        var users = await applicationUserService.FindByEmailAsync(query.Email);

        query.Result = new BaseResult<ApplicationUserViewModel>(
            new ApplicationUserViewModel(users),
            true,
            "Obtido com sucesso");

        return await base.HandleAsync(query, cancellationToken);
    }
}
