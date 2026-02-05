using EChamado.Server.Application.Common.Behaviours;
using EChamado.Server.Application.UseCases.StatusTypes.Notifications;
using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace EChamado.Server.Application.UseCases.StatusTypes.Commands.Handlers;

public class UpdateStatusStatusTypeCommandHandler(
    IUnitOfWork unitOfWork,
    IAmACommandProcessor commandProcessor,
    ILogger<UpdateStatusStatusTypeCommandHandler> logger) :
    RequestHandlerAsync<UpdateStatusStatusTypeCommand>
{
    [RequestLogging(0, HandlerTiming.Before)]
    [RequestValidation(1, HandlerTiming.Before)]
    public override async Task<UpdateStatusStatusTypeCommand> HandleAsync(
        UpdateStatusStatusTypeCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command == null)
        {
            logger.LogError("UpdateStatusStatusTypeCommand is null");
            throw new ArgumentNullException(nameof(command));
        }

        await unitOfWork.BeginTransactionAsync();

        foreach (var item in command.Items)
        {
            var entity = await unitOfWork
                .StatusTypes
                .GetByIdAsync(item.Id);

            if (entity == null)
            {
                logger.LogError("StatusType not found");
                throw new NotFoundException("StatusType not found");
            }

            await unitOfWork.StatusTypes
                .ActiveOrDisableAsync(item.Id, item.Active);

            await unitOfWork.CommitAsync();

            await commandProcessor.PublishAsync(
                new DisabledStatusTypeNotification(
                    entity.Id,
                    entity.Name,
                    entity.Description), cancellationToken: cancellationToken);
        }

        logger.LogInformation("StatusTypes status updated successfully");

        command.Result = new BaseResult(true, "Status dos StatusTypes atualizados com sucesso");

        return await base.HandleAsync(command, cancellationToken);
    }
}
