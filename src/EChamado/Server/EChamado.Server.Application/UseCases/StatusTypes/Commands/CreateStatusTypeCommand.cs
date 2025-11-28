using EChamado.Server.Application.Common.Messaging;
using EChamado.Shared.Responses;

namespace EChamado.Server.Application.UseCases.StatusTypes.Commands;

public class CreateStatusTypeCommand : BrighterRequest<BaseResult<Guid>>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public CreateStatusTypeCommand()
    {
    }

    public CreateStatusTypeCommand(string name, string description)
    {
        Name = name;
        Description = description;
    }
}
