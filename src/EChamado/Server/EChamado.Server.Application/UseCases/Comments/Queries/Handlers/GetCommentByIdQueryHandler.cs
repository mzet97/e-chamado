using EChamado.Server.Application.UseCases.Comments.ViewModels;
using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using Paramore.Brighter;
using Microsoft.Extensions.Logging;

namespace EChamado.Server.Application.UseCases.Comments.Queries.Handlers;

public class GetCommentByIdQueryHandler(
    IUnitOfWork unitOfWork,
    ILogger<GetCommentByIdQueryHandler> logger) :
    RequestHandlerAsync<GetCommentByIdQuery>
{
    public override async Task<GetCommentByIdQuery> HandleAsync(GetCommentByIdQuery query, CancellationToken cancellationToken = default)
    {
        var comment = await unitOfWork.Comments.GetByIdAsync(query.Id);

        if (comment == null)
        {
            logger.LogError("Comment {CommentId} not found", query.Id);
            throw new NotFoundException($"Comment {query.Id} not found");
        }

        var viewModel = new CommentViewModel(
            comment.Id,
            comment.Text,
            comment.OrderId,
            comment.UserId,
            comment.UserEmail,
            comment.CreatedAtUtc,
            comment.UpdatedAtUtc,
            comment.DeletedAtUtc,
            comment.IsDeleted
        );

        logger.LogInformation("Comment {CommentId} retrieved successfully", query.Id);

        query.Result = new BaseResult<CommentViewModel>(viewModel);

        return await base.HandleAsync(query, cancellationToken);
    }
}
