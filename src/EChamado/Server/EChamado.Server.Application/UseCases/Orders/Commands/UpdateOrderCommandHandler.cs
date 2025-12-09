using EChamado.Server.Application.Common.Behaviours;
using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using EChamado.Shared.Services;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace EChamado.Server.Application.UseCases.Orders.Commands;

public class UpdateOrderCommandHandler(
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider,
    ILogger<UpdateOrderCommandHandler> logger) :
    RequestHandlerAsync<UpdateOrderCommand>
{
    [RequestLogging(0, HandlerTiming.Before)]
    [RequestValidation(1, HandlerTiming.Before)]
    public override async Task<UpdateOrderCommand> HandleAsync(UpdateOrderCommand command, CancellationToken cancellationToken = default)
    {
        var order = await unitOfWork.Orders.GetByIdAsync(command.Id);

        if (order == null)
        {
            logger.LogError("Order {OrderId} not found", command.Id);
            throw new NotFoundException($"Order {command.Id} not found");
        }

        order.Update(
            command.Title,
            command.Description,
            order.RequestingUserEmail,
            order.RequestingUserId,
            order.ResponsibleUserId,
            command.CategoryId ?? order.CategoryId,
            command.DepartmentId ?? order.DepartmentId,
            command.TypeId,
            order.StatusId,
            command.SubCategoryId,
            command.DueDate,
            dateTimeProvider
        );

        if (!order.IsValid())
        {
            logger.LogError("Validate Order has error");
            throw new ValidationException("Validate Order has error", order.Errors);
        }

        await unitOfWork.BeginTransactionAsync();

        await unitOfWork.Orders.UpdateAsync(order);

        await unitOfWork.CommitAsync();

        logger.LogInformation("Order {OrderId} updated successfully", command.Id);

        command.Result = new BaseResult();
        return await base.HandleAsync(command, cancellationToken);
    }
}

