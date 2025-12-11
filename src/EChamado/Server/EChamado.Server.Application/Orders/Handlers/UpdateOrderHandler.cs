using EChamado.Server.Application.Orders.Commands;
using EChamado.Server.Domain.Domains.Orders;
using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Services;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace EChamado.Server.Application.Orders.Handlers;

public sealed class UpdateOrderHandler(
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider,
    ILogger<UpdateOrderHandler> logger) : RequestHandlerAsync<UpdateOrderCommand>
{
    public override async Task<UpdateOrderCommand> HandleAsync(UpdateOrderCommand command, CancellationToken cancellationToken = default)
    {
        var order = await unitOfWork.Orders.GetByIdAsync(command.OrderId);
        if (order is null)
        {
            throw new NotFoundException($"Order {command.OrderId} not found");
        }

        order.Update(
            command.Title,
            command.Description,
            command.RequestingUserEmail,
            command.RequestingUserId,
            command.ResponsibleUserId,
            command.CategoryId,
            command.DepartmentId,
            command.TypeId,
            order.StatusId,
            command.SubCategoryId,
            command.DueDate,
            dateTimeProvider);

        await unitOfWork.Orders.UpdateAsync(order);
        logger.LogInformation("Order {OrderId} updated via CQRS pipeline", order.Id);

        return await base.HandleAsync(command, cancellationToken);
    }
}
