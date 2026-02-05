using EChamado.Server.Application.Common.Messaging;
using EChamado.Shared.Responses;

namespace EChamado.Server.Application.UseCases.OrderTypes.Commands;

public class UpdateOrderTypeCommand : BrighterRequest<BaseResult>
{
    public Guid Id { get; set; } = default!;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public UpdateOrderTypeCommand()
    {
    }

    public UpdateOrderTypeCommand(Guid id, string name, string description)
    {
        Id = id;
        Name = name;
        Description = description;
    }
}
