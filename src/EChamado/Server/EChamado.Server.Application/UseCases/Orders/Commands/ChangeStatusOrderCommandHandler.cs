using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EChamado.Server.Application.UseCases.Orders.Commands;

public class ChangeStatusOrderCommandHandler(
    IUnitOfWork unitOfWork,
    ILogger<ChangeStatusOrderCommandHandler> logger) :
    IRequestHandler<ChangeStatusOrderCommand, BaseResult>
{
    public async Task<BaseResult> Handle(ChangeStatusOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await unitOfWork.Orders.GetByIdAsync(request.OrderId, cancellationToken);

        if (order == null)
        {
            logger.LogError("Order {OrderId} not found", request.OrderId);
            throw new NotFoundException($"Order {request.OrderId} not found");
        }

        var status = await unitOfWork.StatusTypes.GetByIdAsync(request.StatusId, cancellationToken);

        if (status == null)
        {
            logger.LogError("Status {StatusId} not found", request.StatusId);
            throw new NotFoundException($"Status {request.StatusId} not found");
        }

        order.ChangeStatus(request.StatusId);

        if (!order.IsValid())
        {
            logger.LogError("Validate Order has error");
            throw new ValidationException("Validate Order has error", order.GetErrors());
        }

        await unitOfWork.BeginTransactionAsync();

        await unitOfWork.Orders.UpdateAsync(order, cancellationToken);

        await unitOfWork.CommitAsync();

        logger.LogInformation("Order {OrderId} status changed to {StatusId}", request.OrderId, request.StatusId);

        return new BaseResult();
    }
}
