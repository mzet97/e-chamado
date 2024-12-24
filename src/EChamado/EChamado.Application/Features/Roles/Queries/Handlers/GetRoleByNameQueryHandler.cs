using EChamado.Application.Features.Roles.ViewModels;
using EChamado.Core.Exceptions;
using EChamado.Core.Responses;
using EChamado.Core.Services.Interface;
using MediatR;

namespace EChamado.Application.Features.Roles.Queries.Handlers;

public class GetRoleByNameQueryHandler(IRoleService roleService) :
    IRequestHandler<GetRoleByNameQuery, BaseResult<RolesViewModel>>
{
    public async Task<BaseResult<RolesViewModel>> Handle(
        GetRoleByNameQuery request,
        CancellationToken cancellationToken)
    {
        var role = await roleService.GetRoleByNameAsync(request.Name);

        if (role == null)
            throw new NotFoundException("Role não encontrada");

        var rolesViewModel = new RolesViewModel(role.Id, role.Name);

        return new BaseResult<RolesViewModel>(rolesViewModel, true, "Obtido com sucesso");
    }
}
