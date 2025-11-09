using EChamado.Server.Application.Common.Messaging;
using EChamado.Shared.Responses;

namespace EChamado.Server.Application.UseCases.Orders.Commands;

public class ChangeStatusOrderCommand : BrighterRequest<BaseResult>
{
    public Guid OrderId { get; set; } = default!;
    public Guid StatusId { get; set; } = default!;

    public ChangeStatusOrderCommand()
    {
    }

    public ChangeStatusOrderCommand(Guid orderId, Guid statusId)
    {
        OrderId = orderId;
        StatusId = statusId;
    }
}
