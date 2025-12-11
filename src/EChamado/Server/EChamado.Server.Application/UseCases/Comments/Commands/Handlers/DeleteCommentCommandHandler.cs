using EChamado.Server.Application.Common.Behaviours;
using EChamado.Server.Application.UseCases.Comments.Notifications;
using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace EChamado.Server.Application.UseCases.Comments.Commands.Handlers;

public class DeleteCommentCommandHandler(
    IUnitOfWork unitOfWork,
    IAmACommandProcessor commandProcessor,
    ILogger<DeleteCommentCommandHandler> logger) :
    RequestHandlerAsync<DeleteCommentCommand>
{
    [RequestLogging(0, HandlerTiming.Before)]
    [RequestValidation(1, HandlerTiming.Before)]
    public override async Task<DeleteCommentCommand> HandleAsync(DeleteCommentCommand command, CancellationToken cancellationToken = default)
    {
        var comment = await unitOfWork.Comments.GetByIdAsync(command.Id);

        if (comment == null)
        {
            logger.LogError("Comment {CommentId} not found", command.Id);
            throw new NotFoundException($"Comment {command.Id} not found");
        }

        await unitOfWork.BeginTransactionAsync();

        await unitOfWork.Comments.RemoveAsync(comment.Id);

        await unitOfWork.CommitAsync();

        await commandProcessor.PublishAsync(new DeletedCommentNotification(
            comment.Id,
            comment.OrderId,
            comment.UserId,
            comment.UserEmail,
            comment.Text), cancellationToken: cancellationToken);

        logger.LogInformation("Comment {CommentId} deleted successfully", command.Id);

        command.Result = new BaseResult();
        return await base.HandleAsync(command, cancellationToken);
    }
}
