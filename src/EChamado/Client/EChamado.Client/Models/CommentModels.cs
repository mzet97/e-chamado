namespace EChamado.Client.Models;

public record CommentResponse(
    Guid Id,
    string Text,
    Guid OrderId,
    Guid UserId,
    string UserEmail,
    bool IsInternal,
    DateTime CreatedAt
);

public record CreateCommentRequest(
    Guid OrderId,
    string Description,
    Guid UserId,
    string UserEmail,
    bool IsInternal = false
);
