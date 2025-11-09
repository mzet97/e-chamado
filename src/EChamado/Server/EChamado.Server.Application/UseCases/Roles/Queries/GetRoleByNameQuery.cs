using EChamado.Server.Application.Common.Messaging;
using EChamado.Server.Application.UseCases.Roles.ViewModels;
using EChamado.Shared.Responses;

namespace EChamado.Server.Application.UseCases.Roles.Queries;

public class GetRoleByNameQuery : BrighterRequest<BaseResult<RolesViewModel>>
{
    public string Name { get; set; } = string.Empty;

    public GetRoleByNameQuery()
    {
    }

    public GetRoleByNameQuery(string name)
    {
        Name = name;
    }
}
