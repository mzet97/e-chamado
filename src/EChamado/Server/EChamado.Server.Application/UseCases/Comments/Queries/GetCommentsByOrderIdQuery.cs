using EChamado.Server.Application.Common.Messaging;
using EChamado.Server.Application.UseCases.Comments.ViewModels;
using EChamado.Shared.Responses;

namespace EChamado.Server.Application.UseCases.Comments.Queries;

public class GetCommentsByOrderIdQuery : BrighterRequest<BaseResultList<CommentViewModel>>
{
    public Guid OrderId { get; set; }

    public GetCommentsByOrderIdQuery()
    {
    }

    public GetCommentsByOrderIdQuery(Guid orderId)
    {
        OrderId = orderId;
    }
}
