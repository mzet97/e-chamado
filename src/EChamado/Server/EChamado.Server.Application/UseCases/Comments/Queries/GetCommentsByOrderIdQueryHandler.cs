using EChamado.Server.Application.UseCases.Comments.ViewModels;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using Paramore.Brighter;

namespace EChamado.Server.Application.UseCases.Comments.Queries;

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
            c.CreatedAt
        )).ToList();

        query.Result = new BaseResultList<CommentViewModel>(items, new PagedResult(items.Count, 1, items.Count));

        return await base.HandleAsync(query, cancellationToken);
    }
}
