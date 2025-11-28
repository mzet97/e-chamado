using EChamado.Server.Application.Common.Messaging;
using EChamado.Shared.Responses;

namespace EChamado.Server.Application.UseCases.OrderTypes.Commands;

public class DeleteOrderTypeCommand : BrighterRequest<BaseResult>
{
    public Guid OrderTypeId { get; set; } = default!;

    public DeleteOrderTypeCommand()
    {
    }

    public DeleteOrderTypeCommand(Guid orderTypeId)
    {
        OrderTypeId = orderTypeId;
    }
}
