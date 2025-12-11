using EChamado.Server.Application.Common.Behaviours;
using EChamado.Server.Application.UseCases.StatusTypes.Notifications;
using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace EChamado.Server.Application.UseCases.StatusTypes.Commands.Handlers;

public class DisableStatusTypeCommandHandler(
    IUnitOfWork unitOfWork,
    IAmACommandProcessor commandProcessor,
    ILogger<DisableStatusTypeCommandHandler> logger) :
    RequestHandlerAsync<DisableStatusTypeCommand>
{
    [RequestLogging(0, HandlerTiming.Before)]
    [RequestValidation(1, HandlerTiming.Before)]
    public override async Task<DisableStatusTypeCommand> HandleAsync(
        DisableStatusTypeCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command == null)
        {
            logger.LogError("DisableStatusTypeCommand is null");
            throw new ArgumentNullException(nameof(command));
        }

        var entity = await unitOfWork
            .StatusTypes
            .GetByIdAsync(command.Id);

        if (entity == null)
        {
            logger.LogError("StatusType not found");
            throw new NotFoundException("StatusType not found");
        }

        await unitOfWork.BeginTransactionAsync();

        await unitOfWork.StatusTypes
            .DisableAsync(command.Id);

        await unitOfWork.CommitAsync();

        await commandProcessor.PublishAsync(
            new DisabledStatusTypeNotification(
                entity.Id,
                entity.Name,
                entity.Description), cancellationToken: cancellationToken);

        logger.LogInformation("StatusType {StatusTypeId} disabled successfully", command.Id);

        command.Result = new BaseResult(true, "StatusType desativado com sucesso");

        return await base.HandleAsync(command, cancellationToken);
    }
}
