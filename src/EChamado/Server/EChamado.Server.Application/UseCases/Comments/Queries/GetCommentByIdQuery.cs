using EChamado.Server.Application.Common.Messaging;
using EChamado.Server.Application.UseCases.Comments.ViewModels;
using EChamado.Shared.Responses;

namespace EChamado.Server.Application.UseCases.Comments.Queries;

public class GetCommentByIdQuery : BrighterRequest<BaseResult<CommentViewModel>>
{
    public Guid Id { get; set; }

    public GetCommentByIdQuery()
    {
    }

    public GetCommentByIdQuery(Guid id)
    {
        Id = id;
    }
}
