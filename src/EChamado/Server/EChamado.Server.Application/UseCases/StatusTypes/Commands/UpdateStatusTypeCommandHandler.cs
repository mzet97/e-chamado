using EChamado.Server.Application.Common.Behaviours;
using EChamado.Server.Application.UseCases.StatusTypes.Notifications;
using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace EChamado.Server.Application.UseCases.StatusTypes.Commands;

public class UpdateStatusTypeCommandHandler(
    IUnitOfWork unitOfWork,
    IAmACommandProcessor commandProcessor,
    ILogger<UpdateStatusTypeCommandHandler> logger) :
    RequestHandlerAsync<UpdateStatusTypeCommand>
{
    [RequestLogging(0, HandlerTiming.Before)]
    [RequestValidation(1, HandlerTiming.Before)]
    public override async Task<UpdateStatusTypeCommand> HandleAsync(UpdateStatusTypeCommand command, CancellationToken cancellationToken = default)
    {
        var statusType = await unitOfWork.StatusTypes.GetByIdAsync(command.Id);

        if (statusType == null)
        {
            logger.LogError("StatusType {StatusTypeId} not found", command.Id);
            throw new NotFoundException($"StatusType {command.Id} not found");
        }

        statusType.Update(command.Name, command.Description);

        if (!statusType.IsValid())
        {
            logger.LogError("Validate StatusType has error");
            throw new ValidationException("Validate StatusType has error", statusType.GetErrors());
        }

        await unitOfWork.BeginTransactionAsync();

        await unitOfWork.StatusTypes.UpdateAsync(statusType);

        await unitOfWork.CommitAsync();

        await commandProcessor.PublishAsync(new UpdatedStatusTypeNotification(statusType.Id, statusType.Name, statusType.Description), cancellationToken: cancellationToken);

        logger.LogInformation("StatusType {StatusTypeId} updated successfully", command.Id);

        command.Result = new BaseResult();
        return await base.HandleAsync(command, cancellationToken);
    }
}
