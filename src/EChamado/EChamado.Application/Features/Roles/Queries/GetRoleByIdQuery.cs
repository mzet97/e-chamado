using EChamado.Application.Features.Roles.ViewModels;
using EChamado.Core.Responses;
using MediatR;

namespace EChamado.Application.Features.Roles.Queries;

public class GetRoleByIdQuery : IRequest<BaseResult<RolesViewModel>>
{
    public Guid Id { get; set; }
    public GetRoleByIdQuery(Guid id)
    {
        Id = id;
    }
}
