using EChamado.Server.Application.Common.Behaviours;
using EChamado.Server.Application.UseCases.StatusTypes.Notifications;
using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace EChamado.Server.Application.UseCases.StatusTypes.Commands;

public class DeleteStatusTypeCommandHandler(
    IUnitOfWork unitOfWork,
    IAmACommandProcessor commandProcessor,
    ILogger<DeleteStatusTypeCommandHandler> logger) :
    RequestHandlerAsync<DeleteStatusTypeCommand>
{
    [RequestLogging(0, HandlerTiming.Before)]
    [RequestValidation(1, HandlerTiming.Before)]
    public override async Task<DeleteStatusTypeCommand> HandleAsync(DeleteStatusTypeCommand command, CancellationToken cancellationToken = default)
    {
        var statusType = await unitOfWork.StatusTypes.GetByIdAsync(command.StatusTypeId);

        if (statusType == null)
        {
            logger.LogError("StatusType {StatusTypeId} not found", command.StatusTypeId);
            throw new NotFoundException($"StatusType {command.StatusTypeId} not found");
        }

        await unitOfWork.BeginTransactionAsync();

        await unitOfWork.StatusTypes.RemoveAsync(statusType.Id);

        await unitOfWork.CommitAsync();

        await commandProcessor.PublishAsync(new DeletedStatusTypeNotification(statusType.Id, statusType.Name, statusType.Description), cancellationToken: cancellationToken);

        logger.LogInformation("StatusType {StatusTypeId} deleted successfully", command.StatusTypeId);

        command.Result = new BaseResult();
        return await base.HandleAsync(command, cancellationToken);
    }
}
