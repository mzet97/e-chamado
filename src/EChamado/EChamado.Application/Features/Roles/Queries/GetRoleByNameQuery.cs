using EChamado.Application.Features.Roles.ViewModels;
using EChamado.Core.Responses;
using MediatR;

namespace EChamado.Application.Features.Roles.Queries;

public class GetRoleByNameQuery : IRequest<BaseResult<RolesViewModel>>
{
    public string Name { get; set; }
    public GetRoleByNameQuery(string name)
    {
        Name = name;
    }
}
