using EChamado.Server.Application.Common.Messaging;
using EChamado.Shared.Responses;

namespace EChamado.Server.Application.UseCases.Roles.Commands;

public class UpdateRoleCommand : BrighterRequest<BaseResult>
{
    public Guid Id { get; set; } = default!;
    public string Name { get; set; } = string.Empty;

    public UpdateRoleCommand()
    {
    }

    public UpdateRoleCommand(Guid id, string name)
    {
        Id = id;
        Name = name;
    }
}
