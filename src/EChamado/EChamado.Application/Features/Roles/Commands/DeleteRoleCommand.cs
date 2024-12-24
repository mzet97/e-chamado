using EChamado.Core.Responses;
using MediatR;

namespace EChamado.Application.Features.Roles.Commands;

public class DeleteRoleCommand : IRequest<BaseResult>
{
    public DeleteRoleCommand(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; set; }
}
