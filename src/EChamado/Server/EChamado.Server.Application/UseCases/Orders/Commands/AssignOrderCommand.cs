using EChamado.Server.Application.Common.Messaging;
using EChamado.Shared.Responses;

namespace EChamado.Server.Application.UseCases.Orders.Commands;

public class AssignOrderCommand : BrighterRequest<BaseResult>
{
    public Guid OrderId { get; set; } = default!;
    public Guid AssignedToUserId { get; set; } = default!;

    public AssignOrderCommand()
    {
    }

    public AssignOrderCommand(Guid orderId, Guid assignedToUserId)
    {
        OrderId = orderId;
        AssignedToUserId = assignedToUserId;
    }
}
