using EChamado.Server.Application.UseCases.OrderTypes.ViewModels;
using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EChamado.Server.Application.UseCases.OrderTypes.Queries;

public class GetOrderTypeByIdQueryHandler(
    IUnitOfWork unitOfWork,
    ILogger<GetOrderTypeByIdQueryHandler> logger) :
    IRequestHandler<GetOrderTypeByIdQuery, BaseResult<OrderTypeViewModel>>
{
    public async Task<BaseResult<OrderTypeViewModel>> Handle(GetOrderTypeByIdQuery request, CancellationToken cancellationToken)
    {
        var orderType = await unitOfWork.OrderTypes.GetByIdAsync(request.OrderTypeId, cancellationToken);

        if (orderType == null)
        {
            logger.LogError("OrderType {OrderTypeId} not found", request.OrderTypeId);
            throw new NotFoundException($"OrderType {request.OrderTypeId} not found");
        }

        var viewModel = new OrderTypeViewModel(
            orderType.Id,
            orderType.Name,
            orderType.Description
        );

        logger.LogInformation("OrderType {OrderTypeId} retrieved successfully", request.OrderTypeId);

        return new BaseResult<OrderTypeViewModel>(viewModel);
    }
}
