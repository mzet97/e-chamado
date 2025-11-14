using EChamado.Server.Application.UseCases.OrderTypes.ViewModels;
using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using Paramore.Brighter;
using Microsoft.Extensions.Logging;

namespace EChamado.Server.Application.UseCases.OrderTypes.Queries;

public class GetOrderTypeByIdQueryHandler(
    IUnitOfWork unitOfWork,
    ILogger<GetOrderTypeByIdQueryHandler> logger) :
    RequestHandlerAsync<GetOrderTypeByIdQuery>
{
    public override async Task<GetOrderTypeByIdQuery> HandleAsync(GetOrderTypeByIdQuery query, CancellationToken cancellationToken = default)
    {
        var orderType = await unitOfWork.OrderTypes.GetByIdAsync(query.OrderTypeId);

        if (orderType == null)
        {
            logger.LogError("OrderType {OrderTypeId} not found", query.OrderTypeId);
            throw new NotFoundException($"OrderType {query.OrderTypeId} not found");
        }

        var viewModel = new OrderTypeViewModel(
            orderType.Id,
            orderType.Name,
            orderType.Description
        );

        logger.LogInformation("OrderType {OrderTypeId} retrieved successfully", query.OrderTypeId);

        query.Result = new BaseResult<OrderTypeViewModel>(viewModel);

        return await base.HandleAsync(query, cancellationToken);
    }
}
