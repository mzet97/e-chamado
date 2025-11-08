using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EChamado.Server.Application.UseCases.Orders.Commands;

public class CloseOrderCommandHandler(
    IUnitOfWork unitOfWork,
    ILogger<CloseOrderCommandHandler> logger) :
    IRequestHandler<CloseOrderCommand, BaseResult>
{
    public async Task<BaseResult> Handle(CloseOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await unitOfWork.Orders.GetByIdAsync(request.OrderId, cancellationToken);

        if (order == null)
        {
            logger.LogError("Order {OrderId} not found", request.OrderId);
            throw new NotFoundException($"Order {request.OrderId} not found");
        }

        if (order.ClosingDate.HasValue)
        {
            logger.LogWarning("Order {OrderId} is already closed", request.OrderId);
            throw new ValidationException("Order is already closed");
        }

        order.Close(request.Evaluation ?? 0);

        if (!order.IsValid())
        {
            logger.LogError("Validate Order has error");
            throw new ValidationException("Validate Order has error", order.GetErrors());
        }

        await unitOfWork.BeginTransactionAsync();

        await unitOfWork.Orders.UpdateAsync(order, cancellationToken);

        await unitOfWork.CommitAsync();

        logger.LogInformation("Order {OrderId} closed successfully with evaluation {Evaluation}",
            request.OrderId, request.Evaluation);

        return new BaseResult();
    }
}
