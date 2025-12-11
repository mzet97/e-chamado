using EChamado.Server.Application.Common.Behaviours;
using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using EChamado.Shared.Services;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace EChamado.Server.Application.UseCases.Orders.Commands.Handlers;

public class ChangeStatusOrderCommandHandler(
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider,
    ILogger<ChangeStatusOrderCommandHandler> logger) :
    RequestHandlerAsync<ChangeStatusOrderCommand>
{
    [RequestLogging(0, HandlerTiming.Before)]
    [RequestValidation(1, HandlerTiming.Before)]
    public override async Task<ChangeStatusOrderCommand> HandleAsync(ChangeStatusOrderCommand command, CancellationToken cancellationToken = default)
    {
        var order = await unitOfWork.Orders.GetByIdAsync(command.OrderId);

        if (order == null)
        {
            logger.LogError("Order {OrderId} not found", command.OrderId);
            throw new NotFoundException($"Order {command.OrderId} not found");
        }

        var status = await unitOfWork.StatusTypes.GetByIdAsync(command.StatusId);

        if (status == null)
        {
            logger.LogError("Status {StatusId} not found", command.StatusId);
            throw new NotFoundException($"Status {command.StatusId} not found");
        }

        order.ChangeStatus(command.StatusId, dateTimeProvider);

        if (!order.IsValid())
        {
            logger.LogError("Validate Order has error");
            throw new ValidationException("Validate Order has error", order.Errors);
        }

        await unitOfWork.BeginTransactionAsync();

        await unitOfWork.Orders.UpdateAsync(order);

        await unitOfWork.CommitAsync();

        logger.LogInformation("Order {OrderId} status changed to {StatusId}", command.OrderId, command.StatusId);

        command.Result = new BaseResult();
        return await base.HandleAsync(command, cancellationToken);
    }
}

