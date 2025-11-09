namespace EChamado.Client.Models;

public record CommentResponse(
    Guid Id,
    string Text,
    Guid OrderId,
    Guid UserId,
    string UserEmail,
    DateTime CreatedAt
);

public record CreateCommentRequest(
    string Text,
    Guid UserId,
    string UserEmail
);
