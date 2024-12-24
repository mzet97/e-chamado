using EChamado.Application.Features.Users.ViewModels;
using EChamado.Core.Responses;
using EChamado.Core.Services.Interface;
using MediatR;

namespace EChamado.Application.Features.Users.Queries.Handlers;

public class GetByIdUserQueryHandler(IApplicationUserService applicationUserService) :
    IRequestHandler<GetByIdUserQuery, BaseResult<ApplicationUserViewModel>>
{
    public async Task<BaseResult<ApplicationUserViewModel>> Handle(GetByIdUserQuery request, CancellationToken cancellationToken)
    {
        var users = await applicationUserService.FindByIdAsync(request.Id);
        
        return new BaseResult<ApplicationUserViewModel>(
            new ApplicationUserViewModel(users), 
            true, 
            "Obtido com sucesso");
    }
}
