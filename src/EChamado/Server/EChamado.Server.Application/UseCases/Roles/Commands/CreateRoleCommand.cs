using EChamado.Server.Application.Common.Messaging;
using EChamado.Shared.Responses;

namespace EChamado.Server.Application.UseCases.Roles.Commands;

public class CreateRoleCommand : BrighterRequest<BaseResult<Guid>>
{
    public string Name { get; set; } = string.Empty;

    public CreateRoleCommand()
    {
    }

    public CreateRoleCommand(string name)
    {
        Name = name;
    }
}
