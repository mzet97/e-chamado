using EChamado.Server.Application.Common.Behaviours;
using EChamado.Server.Application.UseCases.Comments.Notifications;
using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace EChamado.Server.Application.UseCases.Comments.Commands.Handlers;

public class DisableCommentCommandHandler(
    IUnitOfWork unitOfWork,
    IAmACommandProcessor commandProcessor,
    ILogger<DisableCommentCommandHandler> logger) :
    RequestHandlerAsync<DisableCommentCommand>
{
    [RequestLogging(0, HandlerTiming.Before)]
    [RequestValidation(1, HandlerTiming.Before)]
    public override async Task<DisableCommentCommand> HandleAsync(
        DisableCommentCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command == null)
        {
            logger.LogError("DisableCommentCommand is null");
            throw new ArgumentNullException(nameof(command));
        }

        var entity = await unitOfWork
            .Comments
            .GetByIdAsync(command.Id);

        if (entity == null)
        {
            logger.LogError("Comment not found");
            throw new NotFoundException("Comment not found");
        }

        await unitOfWork.BeginTransactionAsync();

        await unitOfWork.Comments
            .DisableAsync(command.Id);

        await unitOfWork.CommitAsync();

        await commandProcessor.PublishAsync(
            new DisabledCommentNotification(
                entity.Id,
                entity.OrderId,
                entity.UserId,
                entity.UserEmail,
                entity.Text), cancellationToken: cancellationToken);

        logger.LogInformation("Comment {CommentId} disabled successfully", command.Id);

        command.Result = new BaseResult(true, "Comment desativado com sucesso");

        return await base.HandleAsync(command, cancellationToken);
    }
}
