using EChamado.Server.Application.UseCases.Users.ViewModels;
using EChamado.Server.Domain.Services.Interface;
using EChamado.Shared.Responses;
using MediatR;

namespace EChamado.Server.Application.UseCases.Users.Queries.Handlers;

public class GetByEmailUserQueryHandler(IApplicationUserService applicationUserService) :
    IRequestHandler<GetByEmailUserQuery, BaseResult<ApplicationUserViewModel>>
{
    public async Task<BaseResult<ApplicationUserViewModel>> Handle(
        GetByEmailUserQuery request,
        CancellationToken cancellationToken)
{
    var users = await applicationUserService.FindByEmailAsync(request.Email);

    return new BaseResult<ApplicationUserViewModel>(
        new ApplicationUserViewModel(users),
        true,
        "Obtido com sucesso");
}
}
