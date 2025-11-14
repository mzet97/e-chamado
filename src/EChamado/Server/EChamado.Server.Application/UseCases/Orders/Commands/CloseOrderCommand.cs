using EChamado.Server.Application.Common.Messaging;
using EChamado.Shared.Responses;

namespace EChamado.Server.Application.UseCases.Orders.Commands;

public class CloseOrderCommand : BrighterRequest<BaseResult>
{
    public Guid OrderId { get; set; } = default!;
    public int? Evaluation { get; set; }

    public CloseOrderCommand()
    {
    }

    public CloseOrderCommand(Guid orderId, int? evaluation)
    {
        OrderId = orderId;
        Evaluation = evaluation;
    }
}
