using EChamado.Server.Application.UseCases.Users.ViewModels;
using EChamado.Server.Domain.Services.Interface;
using EChamado.Shared.Responses;
using MediatR;

namespace EChamado.Server.Application.UseCases.Users.Queries.Handlers;

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
