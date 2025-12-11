using EChamado.Server.Application.Common.Messaging;
using EChamado.Server.Application.UseCases.Roles.ViewModels;
using EChamado.Shared.Responses;

namespace EChamado.Server.Application.UseCases.Roles.Queries;

public class GetRoleByIdQuery : BrighterRequest<BaseResult<RolesViewModel>>
{
    public Guid Id { get; set; }

    public GetRoleByIdQuery()
    {
    }

    public GetRoleByIdQuery(Guid id)
    {
        Id = id;
    }
}
