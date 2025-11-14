using EChamado.Server.Application.Common.Messaging;
using EChamado.Shared.Responses;

namespace EChamado.Server.Application.UseCases.Comments.Commands;

public class CreateCommentCommand : BrighterRequest<BaseResult<Guid>>
{
    public string Text { get; set; } = string.Empty;
    public Guid OrderId { get; set; } = default!;
    public Guid UserId { get; set; } = default!;
    public string UserEmail { get; set; } = string.Empty;

    public CreateCommentCommand()
    {
    }

    public CreateCommentCommand(string text, Guid orderId, Guid userId, string userEmail)
    {
        Text = text;
        OrderId = orderId;
        UserId = userId;
        UserEmail = userEmail;
    }
}
