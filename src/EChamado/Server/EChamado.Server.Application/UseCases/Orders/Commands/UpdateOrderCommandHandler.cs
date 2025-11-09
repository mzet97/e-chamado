using EChamado.Server.Application.Common.Behaviours;
using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace EChamado.Server.Application.UseCases.Orders.Commands;

public class UpdateOrderCommandHandler(
    IUnitOfWork unitOfWork,
    ILogger<UpdateOrderCommandHandler> logger) :
    RequestHandlerAsync<UpdateOrderCommand>
{
    [RequestLogging(0, HandlerTiming.Before)]
    [RequestValidation(1, HandlerTiming.Before)]
    public override async Task<UpdateOrderCommand> HandleAsync(UpdateOrderCommand command, CancellationToken cancellationToken = default)
    {
        var order = await unitOfWork.Orders.GetByIdAsync(command.Id, cancellationToken);

        if (order == null)
        {
            logger.LogError("Order {OrderId} not found", command.Id);
            throw new NotFoundException($"Order {command.Id} not found");
        }

        order.Update(
            command.Title,
            command.Description,
            command.CategoryId,
            command.SubCategoryId,
            command.DepartmentId,
            command.DueDate
        );

        if (!order.IsValid())
        {
            logger.LogError("Validate Order has error");
            throw new ValidationException("Validate Order has error", order.GetErrors());
        }

        await unitOfWork.BeginTransactionAsync();

        await unitOfWork.Orders.UpdateAsync(order, cancellationToken);

        await unitOfWork.CommitAsync();

        logger.LogInformation("Order {OrderId} updated successfully", command.Id);

        command.Result = new BaseResult();
        return await base.HandleAsync(command, cancellationToken);
    }
}
