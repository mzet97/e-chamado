using EChamado.Server.Application.UseCases.Roles.ViewModels;
using EChamado.Server.Domain.Services.Interface;
using EChamado.Shared.Responses;
using Paramore.Brighter;

namespace EChamado.Server.Application.UseCases.Roles.Queries.Handlers;

public class GetAllRolesQueryHandler(IRoleService roleService) :
    RequestHandlerAsync<GetAllRolesQuery>
{
    public override async Task<GetAllRolesQuery> HandleAsync(
        GetAllRolesQuery query,
        CancellationToken cancellationToken = default)
    {
        var roles = await roleService.GetAllRolesAsync();

        var rolesViewModel = roles.Select(role => new RolesViewModel(role.Id, role.Name));

        query.Result = new BaseResultList<RolesViewModel>(rolesViewModel, null, true, "Obtido com sucesso");

        return await base.HandleAsync(query, cancellationToken);
    }
}
