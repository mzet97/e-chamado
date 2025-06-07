using EChamado.Server.Application.UseCases.Roles.ViewModels;
using EChamado.Server.Domain.Services.Interface;
using EChamado.Shared.Responses;
using MediatR;

namespace EChamado.Server.Application.UseCases.Roles.Queries.Handlers;

public class GetAllRolesQueryHandler(IRoleService roleService) :
    IRequestHandler<GetAllRolesQuery, BaseResultList<RolesViewModel>>
{
    public async Task<BaseResultList<RolesViewModel>> Handle(
        GetAllRolesQuery request,
        CancellationToken cancellationToken)
{
    var roles = await roleService.GetAllRolesAsync();

    var rolesViewModel = roles.Select(role => new RolesViewModel(role.Id, role.Name));

    return new BaseResultList<RolesViewModel>(rolesViewModel, null, true, "Obtido com sucesso");
}
}
