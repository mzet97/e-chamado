using EChamado.Server.Application.Common.Messaging;
using EChamado.Shared.Responses;

namespace EChamado.Server.Application.UseCases.Roles.Commands;

public class DeleteRoleCommand : BrighterRequest<BaseResult>
{
    public Guid Id { get; set; } = default!;

    public DeleteRoleCommand()
    {
    }

    public DeleteRoleCommand(Guid id)
    {
        Id = id;
    }
}
