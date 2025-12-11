using EChamado.Server.Application.Orders.Commands;
using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Services;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace EChamado.Server.Application.Orders.Handlers;

public sealed class ChangeOrderStatusHandler(
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider,
    ILogger<ChangeOrderStatusHandler> logger) : RequestHandlerAsync<ChangeOrderStatusCommand>
{
    public override async Task<ChangeOrderStatusCommand> HandleAsync(ChangeOrderStatusCommand command, CancellationToken cancellationToken = default)
    {
        var order = await unitOfWork.Orders.GetByIdAsync(command.OrderId);
        if (order is null)
        {
            throw new NotFoundException($"Order {command.OrderId} not found");
        }

        order.ChangeStatus(command.StatusId, dateTimeProvider);
        await unitOfWork.Orders.UpdateAsync(order);

        logger.LogInformation("Order {OrderId} status changed to {StatusId}", order.Id, command.StatusId);
        return await base.HandleAsync(command, cancellationToken);
    }
}
