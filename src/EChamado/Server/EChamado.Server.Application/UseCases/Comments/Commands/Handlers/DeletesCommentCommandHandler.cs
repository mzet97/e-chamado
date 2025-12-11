using EChamado.Server.Application.Common.Behaviours;
using EChamado.Server.Application.UseCases.Comments.Notifications;
using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace EChamado.Server.Application.UseCases.Comments.Commands.Handlers;

public class DeletesCommentCommandHandler(
    IUnitOfWork unitOfWork,
    IAmACommandProcessor commandProcessor,
    ILogger<DeletesCommentCommandHandler> logger) :
    RequestHandlerAsync<DeletesCommentCommand>
{
    [RequestLogging(0, HandlerTiming.Before)]
    [RequestValidation(1, HandlerTiming.Before)]
    public override async Task<DeletesCommentCommand> HandleAsync(
        DeletesCommentCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command == null)
        {
            logger.LogError("DeletesCommentCommand is null");
            throw new ArgumentNullException(nameof(command));
        }

        await unitOfWork.BeginTransactionAsync();

        foreach (var id in command.Ids)
        {
            var entity = await unitOfWork
                .Comments
                .GetByIdAsync(id);

            if (entity == null)
            {
                logger.LogError("Comment not found");
                throw new NotFoundException("Comment not found");
            }

            await unitOfWork.Comments
                .RemoveAsync(id);

            await unitOfWork.CommitAsync();

            await commandProcessor.PublishAsync(
                new DeletedCommentNotification(
                    entity.Id,
                    entity.OrderId,
                    entity.UserId,
                    entity.UserEmail,
                    entity.Text), cancellationToken: cancellationToken);
        }

        logger.LogInformation("Comments deleted successfully");

        command.Result = new BaseResult(true, "Comments deletados com sucesso");

        return await base.HandleAsync(command, cancellationToken);
    }
}
