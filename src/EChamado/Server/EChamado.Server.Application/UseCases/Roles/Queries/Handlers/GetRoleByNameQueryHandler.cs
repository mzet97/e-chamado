using EChamado.Server.Application.UseCases.Roles.ViewModels;
using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Services.Interface;
using EChamado.Shared.Responses;
using MediatR;

namespace EChamado.Server.Application.UseCases.Roles.Queries.Handlers;

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
