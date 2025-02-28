using EChamado.Server.Application.UseCases.Roles.ViewModels;
using EChamado.Shared.Responses;
using MediatR;

namespace EChamado.Server.Application.UseCases.Roles.Queries;

public class GetRoleByNameQuery : IRequest<BaseResult<RolesViewModel>>
{
    public string Name { get; set; }
    public GetRoleByNameQuery(string name)
    {
        Name = name;
    }
}
