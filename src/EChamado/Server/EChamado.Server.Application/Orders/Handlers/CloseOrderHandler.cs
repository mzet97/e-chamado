using EChamado.Server.Application.Orders.Commands;
using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Services;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace EChamado.Server.Application.Orders.Handlers;

public sealed class CloseOrderHandler(
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider,
    ILogger<CloseOrderHandler> logger) : RequestHandlerAsync<CloseOrderCommand>
{
    public override async Task<CloseOrderCommand> HandleAsync(CloseOrderCommand command, CancellationToken cancellationToken = default)
    {
        var order = await unitOfWork.Orders.GetByIdAsync(command.OrderId);
        if (order is null)
        {
            throw new NotFoundException($"Order {command.OrderId} not found");
        }

        order.Close(command.Evaluation, dateTimeProvider);
        await unitOfWork.Orders.UpdateAsync(order);

        logger.LogInformation("Order {OrderId} closed with evaluation {Evaluation}", order.Id, command.Evaluation);
        return await base.HandleAsync(command, cancellationToken);
    }
}
