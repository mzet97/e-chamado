using EChamado.Application.Features.Users.ViewModels;
using EChamado.Core.Responses;
using EChamado.Core.Services.Interface;
using MediatR;

namespace EChamado.Application.Features.Users.Queries.Handlers;

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
