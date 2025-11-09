using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using MediatR;
using Microsoft.Extensions.Logging;
using EChamado.Server.Application.UseCases.Comments.Notifications;

namespace EChamado.Server.Application.UseCases.Comments.Commands;

public class DeleteCommentCommandHandler(
    IUnitOfWork unitOfWork,
    IMediator mediator,
    ILogger<DeleteCommentCommandHandler> logger) :
    IRequestHandler<DeleteCommentCommand, BaseResult>
{
    public async Task<BaseResult> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
    {
        var comment = await unitOfWork.Comments.GetByIdAsync(request.CommentId, cancellationToken);

        if (comment == null)
        {
            logger.LogError("Comment {CommentId} not found", request.CommentId);
            throw new NotFoundException($"Comment {request.CommentId} not found");
        }

        await unitOfWork.BeginTransactionAsync();

        await unitOfWork.Comments.DeleteAsync(comment, cancellationToken);

        await unitOfWork.CommitAsync();

        await mediator.Publish(new DeletedCommentNotification(
            comment.Id,
            comment.OrderId), cancellationToken);

        logger.LogInformation("Comment {CommentId} deleted successfully", request.CommentId);

        return new BaseResult();
    }
}
