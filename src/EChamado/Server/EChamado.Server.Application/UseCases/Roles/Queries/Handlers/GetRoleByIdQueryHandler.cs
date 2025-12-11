using EChamado.Server.Application.UseCases.Roles.ViewModels;
using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Services.Interface;
using EChamado.Shared.Responses;
using Paramore.Brighter;

namespace EChamado.Server.Application.UseCases.Roles.Queries.Handlers;

public class GetRoleByIdQueryHandler(IRoleService roleService) :
    RequestHandlerAsync<GetRoleByIdQuery>
{
    public override async Task<GetRoleByIdQuery> HandleAsync(
        GetRoleByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var role = await roleService.GetRoleByIdAsync(query.Id);

        if (role == null)
            throw new NotFoundException("Role não encontrada");

        var rolesViewModel = new RolesViewModel(role.Id, role.Name);

        query.Result = new BaseResult<RolesViewModel>(rolesViewModel, true, "Obtido com sucesso");

        return await base.HandleAsync(query, cancellationToken);
    }
}
