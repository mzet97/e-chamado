using EChamado.Server.Application.Common.Behaviours;
using EChamado.Server.Application.UseCases.Comments.Notifications;
using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace EChamado.Server.Application.UseCases.Comments.Commands.Handlers;

public class UpdateStatusCommentCommandHandler(
    IUnitOfWork unitOfWork,
    IAmACommandProcessor commandProcessor,
    ILogger<UpdateStatusCommentCommandHandler> logger) :
    RequestHandlerAsync<UpdateStatusCommentCommand>
{
    [RequestLogging(0, HandlerTiming.Before)]
    [RequestValidation(1, HandlerTiming.Before)]
    public override async Task<UpdateStatusCommentCommand> HandleAsync(
        UpdateStatusCommentCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command == null)
        {
            logger.LogError("UpdateStatusCommentCommand is null");
            throw new ArgumentNullException(nameof(command));
        }

        await unitOfWork.BeginTransactionAsync();

        foreach (var item in command.Items)
        {
            var entity = await unitOfWork
                .Comments
                .GetByIdAsync(item.Id);

            if (entity == null)
            {
                logger.LogError("Comment not found");
                throw new NotFoundException("Comment not found");
            }

            await unitOfWork.Comments
                .ActiveOrDisableAsync(item.Id, item.Active);

            await unitOfWork.CommitAsync();

            await commandProcessor.PublishAsync(
                new DisabledCommentNotification(
                    entity.Id,
                    entity.OrderId,
                    entity.UserId,
                    entity.UserEmail,
                    entity.Text), cancellationToken: cancellationToken);
        }

        logger.LogInformation("Comments status updated successfully");

        command.Result = new BaseResult(true, "Status dos Comments atualizados com sucesso");

        return await base.HandleAsync(command, cancellationToken);
    }
}
