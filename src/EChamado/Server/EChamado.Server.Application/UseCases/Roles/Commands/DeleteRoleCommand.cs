using EChamado.Shared.Responses;
using MediatR;

namespace EChamado.Server.Application.UseCases.Roles.Commands;

public class DeleteRoleCommand : IRequest<BaseResult>
{
    public DeleteRoleCommand(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; set; }
}
