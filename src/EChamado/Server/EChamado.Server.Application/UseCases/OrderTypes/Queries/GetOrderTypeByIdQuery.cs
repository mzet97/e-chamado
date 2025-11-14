using EChamado.Server.Application.Common.Messaging;
using EChamado.Server.Application.UseCases.OrderTypes.ViewModels;
using EChamado.Shared.Responses;

namespace EChamado.Server.Application.UseCases.OrderTypes.Queries;

public class GetOrderTypeByIdQuery : BrighterRequest<BaseResult<OrderTypeViewModel>>
{
    public Guid OrderTypeId { get; set; }

    public GetOrderTypeByIdQuery()
    {
    }

    public GetOrderTypeByIdQuery(Guid orderTypeId)
    {
        OrderTypeId = orderTypeId;
    }
}
