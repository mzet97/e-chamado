using EChamado.Server.Application.Common.Messaging;
using EChamado.Shared.Responses;

namespace EChamado.Server.Application.UseCases.Comments.Commands;

public class DeletesCommentCommand : BrighterRequest<BaseResult>
{
    public IEnumerable<Guid> Ids { get; set; } = default!;

    public DeletesCommentCommand()
    {
    }

    public DeletesCommentCommand(IEnumerable<Guid> ids)
    {
        Ids = ids;
    }
}
