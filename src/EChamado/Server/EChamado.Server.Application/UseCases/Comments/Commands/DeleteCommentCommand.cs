using EChamado.Server.Application.Common.Messaging;
using EChamado.Shared.Responses;

namespace EChamado.Server.Application.UseCases.Comments.Commands;

public class DeleteCommentCommand : BrighterRequest<BaseResult>
{
    public Guid Id { get; set; } = default!;

    public DeleteCommentCommand()
    {
    }

    public DeleteCommentCommand(Guid id)
    {
        Id = id;
    }
}
