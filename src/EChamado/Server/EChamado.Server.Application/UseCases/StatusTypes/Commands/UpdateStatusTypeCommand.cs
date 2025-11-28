using EChamado.Server.Application.Common.Messaging;
using EChamado.Shared.Responses;

namespace EChamado.Server.Application.UseCases.StatusTypes.Commands;

public class UpdateStatusTypeCommand : BrighterRequest<BaseResult>
{
    public Guid Id { get; set; } = default!;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public UpdateStatusTypeCommand()
    {
    }

    public UpdateStatusTypeCommand(Guid id, string name, string description)
    {
        Id = id;
        Name = name;
        Description = description;
    }
}
