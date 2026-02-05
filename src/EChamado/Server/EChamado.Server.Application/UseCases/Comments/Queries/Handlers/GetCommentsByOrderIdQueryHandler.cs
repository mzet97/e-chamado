using EChamado.Server.Application.UseCases.Comments.ViewModels;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using Paramore.Brighter;

namespace EChamado.Server.Application.UseCases.Comments.Queries.Handlers;

public class GetCommentsByOrderIdQueryHandler(IUnitOfWork unitOfWork) :
    RequestHandlerAsync<GetCommentsByOrderIdQuery>
{
    public override async Task<GetCommentsByOrderIdQuery> HandleAsync(
        GetCommentsByOrderIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var comments = await unitOfWork.Comments.GetByOrderIdAsync(query.OrderId, cancellationToken);

        var items = comments.Select(c => new CommentViewModel(
            c.Id,
            c.Text,
            c.OrderId,
            c.UserId,
            c.UserEmail,
            c.CreatedAtUtc,
            c.UpdatedAtUtc,
            c.DeletedAtUtc,
            c.IsDeleted
        )).ToList();

        var pagedResult = PagedResult.Create(1, items.Count, items.Count);
        query.Result = new BaseResultList<CommentViewModel>(items, pagedResult);

        return await base.HandleAsync(query, cancellationToken);
    }
}
