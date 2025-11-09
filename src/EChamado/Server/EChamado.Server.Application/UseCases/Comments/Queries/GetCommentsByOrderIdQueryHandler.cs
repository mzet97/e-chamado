using EChamado.Server.Application.UseCases.Comments.ViewModels;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using MediatR;

namespace EChamado.Server.Application.UseCases.Comments.Queries;

public class GetCommentsByOrderIdQueryHandler(IUnitOfWork unitOfWork) :
    IRequestHandler<GetCommentsByOrderIdQuery, BaseResultList<CommentViewModel>>
{
    public async Task<BaseResultList<CommentViewModel>> Handle(
        GetCommentsByOrderIdQuery request,
        CancellationToken cancellationToken)
    {
        var comments = await unitOfWork.Comments.GetByOrderIdAsync(request.OrderId, cancellationToken);

        var items = comments.Select(c => new CommentViewModel(
            c.Id,
            c.Text,
            c.OrderId,
            c.UserId,
            c.UserEmail,
            c.CreatedAt
        )).ToList();

        return new BaseResultList<CommentViewModel>(items, new PagedResult(items.Count, 1, items.Count));
    }
}
