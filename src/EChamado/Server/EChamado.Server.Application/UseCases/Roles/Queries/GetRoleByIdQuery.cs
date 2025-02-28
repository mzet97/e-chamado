using EChamado.Server.Application.UseCases.Roles.ViewModels;
using EChamado.Shared.Responses;
using MediatR;

namespace EChamado.Server.Application.UseCases.Roles.Queries;

public class GetRoleByIdQuery : IRequest<BaseResult<RolesViewModel>>
{
    public Guid Id { get; set; }
    public GetRoleByIdQuery(Guid id)
    {
        Id = id;
    }
}
