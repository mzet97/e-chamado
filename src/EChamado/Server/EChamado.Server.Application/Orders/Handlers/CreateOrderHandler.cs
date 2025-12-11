using EChamado.Server.Application.Orders.Commands;
using EChamado.Server.Domain.Domains.Orders;
using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Services;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace EChamado.Server.Application.Orders.Handlers;

public sealed class CreateOrderHandler(
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider,
    ILogger<CreateOrderHandler> logger) : RequestHandlerAsync<CreateOrderCommand>
{
    public override async Task<CreateOrderCommand> HandleAsync(CreateOrderCommand command, CancellationToken cancellationToken = default)
    {
        var status = await unitOfWork.StatusTypes.SearchAsync(
            x => x.Name.ToLower() == "aberto" || x.Name.ToLower() == "open",
            null,
            10,
            1);

        var statusId = status.Data.FirstOrDefault()?.Id;
        if (statusId is null || statusId == Guid.Empty)
        {
            throw new NotFoundException("Default status not found");
        }

        var order = Order.Create(
            command.Title,
            command.Description,
            command.RequestingUserEmail,
            command.ResponsibleUserEmail,
            command.RequestingUserId,
            command.ResponsibleUserId,
            command.CategoryId,
            command.DepartmentId,
            command.TypeId,
            statusId.Value,
            command.SubCategoryId,
            command.DueDate,
            dateTimeProvider);

        if (!order.IsValid())
        {
            throw new ValidationException("Order validation failed", order.Errors);
        }

        await unitOfWork.Orders.AddAsync(order);
        logger.LogInformation("Order {OrderId} created via CQRS pipeline", order.Id);

        return await base.HandleAsync(command, cancellationToken);
    }
}
