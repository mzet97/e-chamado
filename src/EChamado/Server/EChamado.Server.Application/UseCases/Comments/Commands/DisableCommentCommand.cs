using EChamado.Server.Application.Common.Messaging;
using EChamado.Shared.Responses;

namespace EChamado.Server.Application.UseCases.Comments.Commands;

public class DisableCommentCommand : BrighterRequest<BaseResult>
{
    public Guid Id { get; set; } = default!;

    public DisableCommentCommand()
    {
    }

    public DisableCommentCommand(Guid id)
    {
        Id = id;
    }
}
