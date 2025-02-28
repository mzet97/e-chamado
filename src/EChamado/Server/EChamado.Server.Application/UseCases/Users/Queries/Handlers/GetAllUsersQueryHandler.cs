using EChamado.Server.Application.UseCases.Users.ViewModels;
using EChamado.Server.Domain.Services.Interface;
using EChamado.Shared.Responses;
using MediatR;

namespace EChamado.Server.Application.UseCases.Users.Queries.Handlers;

public class GetAllUsersQueryHandler(IApplicationUserService applicationUserService) :
    IRequestHandler<GetAllUsersQuery, BaseResultList<ApplicationUserViewModel>>
{
    public async Task<BaseResultList<ApplicationUserViewModel>> Handle(
        GetAllUsersQuery request, 
        CancellationToken cancellationToken)
    {
        var users = await applicationUserService.GetAllUsersAsync();

        return new BaseResultList<ApplicationUserViewModel>(users
            .Select(x => new ApplicationUserViewModel(x)), 
            null, 
            true, 
            "Obtido com sucesso");
    }
}
