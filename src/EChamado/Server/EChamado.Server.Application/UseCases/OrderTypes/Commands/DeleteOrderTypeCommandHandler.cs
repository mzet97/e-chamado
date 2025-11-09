using EChamado.Server.Application.Common.Behaviours;
using EChamado.Server.Application.UseCases.OrderTypes.Notifications;
using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace EChamado.Server.Application.UseCases.OrderTypes.Commands;

public class DeleteOrderTypeCommandHandler(
    IUnitOfWork unitOfWork,
    IAmACommandProcessor commandProcessor,
    ILogger<DeleteOrderTypeCommandHandler> logger) :
    RequestHandlerAsync<DeleteOrderTypeCommand>
{
    [RequestLogging(0, HandlerTiming.Before)]
    [RequestValidation(1, HandlerTiming.Before)]
    public override async Task<DeleteOrderTypeCommand> HandleAsync(DeleteOrderTypeCommand command, CancellationToken cancellationToken = default)
    {
        var orderType = await unitOfWork.OrderTypes.GetByIdAsync(command.OrderTypeId, cancellationToken);

        if (orderType == null)
        {
            logger.LogError("OrderType {OrderTypeId} not found", command.OrderTypeId);
            throw new NotFoundException($"OrderType {command.OrderTypeId} not found");
        }

        await unitOfWork.BeginTransactionAsync();

        await unitOfWork.OrderTypes.DeleteAsync(orderType, cancellationToken);

        await unitOfWork.CommitAsync();

        await commandProcessor.PublishAsync(new DeletedOrderTypeNotification(orderType.Id, orderType.Name, orderType.Description), cancellationToken: cancellationToken);

        logger.LogInformation("OrderType {OrderTypeId} deleted successfully", command.OrderTypeId);

        command.Result = new BaseResult();
        return await base.HandleAsync(command, cancellationToken);
    }
}
