using EChamado.Server.Application.Common.Messaging;
using EChamado.Shared.Responses;

namespace EChamado.Server.Application.UseCases.OrderTypes.Commands;

public class CreateOrderTypeCommand : BrighterRequest<BaseResult<Guid>>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public CreateOrderTypeCommand()
    {
    }

    public CreateOrderTypeCommand(string name, string description)
    {
        Name = name;
        Description = description;
    }
}
