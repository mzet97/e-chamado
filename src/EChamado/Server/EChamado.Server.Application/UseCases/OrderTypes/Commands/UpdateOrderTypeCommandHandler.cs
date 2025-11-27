using EChamado.Server.Application.Common.Behaviours;
using EChamado.Server.Application.UseCases.OrderTypes.Notifications;
using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using EChamado.Shared.Services;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace EChamado.Server.Application.UseCases.OrderTypes.Commands;

public class UpdateOrderTypeCommandHandler(
    IUnitOfWork unitOfWork,
    IAmACommandProcessor commandProcessor,
    IDateTimeProvider dateTimeProvider,
    ILogger<UpdateOrderTypeCommandHandler> logger) :
    RequestHandlerAsync<UpdateOrderTypeCommand>
{
    [RequestLogging(0, HandlerTiming.Before)]
    [RequestValidation(1, HandlerTiming.Before)]
    public override async Task<UpdateOrderTypeCommand> HandleAsync(UpdateOrderTypeCommand command, CancellationToken cancellationToken = default)
    {
        var orderType = await unitOfWork.OrderTypes.GetByIdAsync(command.Id);

        if (orderType == null)
        {
            logger.LogError("OrderType {OrderTypeId} not found", command.Id);
            throw new NotFoundException($"OrderType {command.Id} not found");
        }

        orderType.Update(command.Name, command.Description, dateTimeProvider);

        if (!orderType.IsValid())
        {
            logger.LogError("Validate OrderType has error");
            throw new ValidationException("Validate OrderType has error", orderType.GetErrors());
        }

        await unitOfWork.BeginTransactionAsync();

        await unitOfWork.OrderTypes.UpdateAsync(orderType);

        await unitOfWork.CommitAsync();

        await commandProcessor.PublishAsync(new UpdatedOrderTypeNotification(orderType.Id, orderType.Name, orderType.Description), cancellationToken: cancellationToken);

        logger.LogInformation("OrderType {OrderTypeId} updated successfully", command.Id);

        command.Result = new BaseResult();
        return await base.HandleAsync(command, cancellationToken);
    }
}
