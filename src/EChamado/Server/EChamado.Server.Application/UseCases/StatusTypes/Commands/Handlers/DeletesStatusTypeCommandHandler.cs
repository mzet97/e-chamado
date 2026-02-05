using EChamado.Server.Application.Common.Behaviours;
using EChamado.Server.Application.UseCases.StatusTypes.Notifications;
using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace EChamado.Server.Application.UseCases.StatusTypes.Commands.Handlers;

public class DeletesStatusTypeCommandHandler(
    IUnitOfWork unitOfWork,
    IAmACommandProcessor commandProcessor,
    ILogger<DeletesStatusTypeCommandHandler> logger) :
    RequestHandlerAsync<DeletesStatusTypeCommand>
{
    [RequestLogging(0, HandlerTiming.Before)]
    [RequestValidation(1, HandlerTiming.Before)]
    public override async Task<DeletesStatusTypeCommand> HandleAsync(
        DeletesStatusTypeCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command == null)
        {
            logger.LogError("DeletesStatusTypeCommand is null");
            throw new ArgumentNullException(nameof(command));
        }

        await unitOfWork.BeginTransactionAsync();

        foreach (var id in command.Ids)
        {
            var entity = await unitOfWork
                .StatusTypes
                .GetByIdAsync(id);

            if (entity == null)
            {
                logger.LogError("StatusType not found");
                throw new NotFoundException("StatusType not found");
            }

            await unitOfWork.StatusTypes
                .RemoveAsync(id);

            await unitOfWork.CommitAsync();

            await commandProcessor.PublishAsync(
                new DeletedStatusTypeNotification(
                    entity.Id,
                    entity.Name,
                    entity.Description), cancellationToken: cancellationToken);
        }

        logger.LogInformation("StatusTypes deleted successfully");

        command.Result = new BaseResult(true, "StatusTypes deletados com sucesso");

        return await base.HandleAsync(command, cancellationToken);
    }
}
