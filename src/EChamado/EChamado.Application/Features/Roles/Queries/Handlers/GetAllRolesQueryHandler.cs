using EChamado.Application.Features.Roles.ViewModels;
using EChamado.Core.Responses;
using EChamado.Core.Services.Interface;
using MediatR;

namespace EChamado.Application.Features.Roles.Queries.Handlers;

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
