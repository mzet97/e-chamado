using EChamado.Server.Application.Common.Messaging;
using EChamado.Server.Application.UseCases.Orders.ViewModels;
using EChamado.Shared.Responses;

namespace EChamado.Server.Application.UseCases.Orders.Queries;

public class GetOrderByIdQuery : BrighterRequest<BaseResult<OrderViewModel>>
{
    public Guid OrderId { get; set; }

    public GetOrderByIdQuery()
    {
    }

    public GetOrderByIdQuery(Guid orderId)
    {
        OrderId = orderId;
    }
}
