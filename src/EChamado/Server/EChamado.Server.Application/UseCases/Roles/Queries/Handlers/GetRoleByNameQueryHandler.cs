using EChamado.Server.Application.UseCases.Roles.ViewModels;
using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Services.Interface;
using EChamado.Shared.Responses;
using Paramore.Brighter;

namespace EChamado.Server.Application.UseCases.Roles.Queries.Handlers;

public class GetRoleByNameQueryHandler(IRoleService roleService) :
    RequestHandlerAsync<GetRoleByNameQuery>
{
    public override async Task<GetRoleByNameQuery> HandleAsync(
        GetRoleByNameQuery query,
        CancellationToken cancellationToken = default)
    {
        var role = await roleService.GetRoleByNameAsync(query.Name);

        if (role == null)
            throw new NotFoundException("Role não encontrada");

        var rolesViewModel = new RolesViewModel(role.Id, role.Name);

        query.Result = new BaseResult<RolesViewModel>(rolesViewModel, true, "Obtido com sucesso");

        return await base.HandleAsync(query, cancellationToken);
    }
}
