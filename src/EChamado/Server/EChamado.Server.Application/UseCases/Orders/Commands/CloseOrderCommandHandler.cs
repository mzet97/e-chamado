using EChamado.Server.Application.Common.Behaviours;
using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace EChamado.Server.Application.UseCases.Orders.Commands;

public class CloseOrderCommandHandler(
    IUnitOfWork unitOfWork,
    ILogger<CloseOrderCommandHandler> logger) :
    RequestHandlerAsync<CloseOrderCommand>
{
    [RequestLogging(0, HandlerTiming.Before)]
    [RequestValidation(1, HandlerTiming.Before)]
    public override async Task<CloseOrderCommand> HandleAsync(CloseOrderCommand command, CancellationToken cancellationToken = default)
    {
        var order = await unitOfWork.Orders.GetByIdAsync(command.OrderId, cancellationToken);

        if (order == null)
        {
            logger.LogError("Order {OrderId} not found", command.OrderId);
            throw new NotFoundException($"Order {command.OrderId} not found");
        }

        if (order.ClosingDate.HasValue)
        {
            logger.LogWarning("Order {OrderId} is already closed", command.OrderId);
            throw new ValidationException("Order is already closed");
        }

        order.Close(command.Evaluation ?? 0);

        if (!order.IsValid())
        {
            logger.LogError("Validate Order has error");
            throw new ValidationException("Validate Order has error", order.GetErrors());
        }

        await unitOfWork.BeginTransactionAsync();

        await unitOfWork.Orders.UpdateAsync(order, cancellationToken);

        await unitOfWork.CommitAsync();

        logger.LogInformation("Order {OrderId} closed successfully with evaluation {Evaluation}",
            command.OrderId, command.Evaluation);

        command.Result = new BaseResult();
        return await base.HandleAsync(command, cancellationToken);
    }
}
