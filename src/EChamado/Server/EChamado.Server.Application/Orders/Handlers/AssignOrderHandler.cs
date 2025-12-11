using EChamado.Server.Application.Orders.Commands;
using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Services;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace EChamado.Server.Application.Orders.Handlers;

public sealed class AssignOrderHandler(
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider,
    ILogger<AssignOrderHandler> logger) : RequestHandlerAsync<AssignOrderCommand>
{
    public override async Task<AssignOrderCommand> HandleAsync(AssignOrderCommand command, CancellationToken cancellationToken = default)
    {
        var order = await unitOfWork.Orders.GetByIdAsync(command.OrderId);
        if (order is null)
        {
            throw new NotFoundException($"Order {command.OrderId} not found");
        }

        order.AssignTo(command.UserId, command.UserEmail, dateTimeProvider);
        await unitOfWork.Orders.UpdateAsync(order);

        logger.LogInformation("Order {OrderId} assigned to {UserId}", order.Id, command.UserId);
        return await base.HandleAsync(command, cancellationToken);
    }
}
