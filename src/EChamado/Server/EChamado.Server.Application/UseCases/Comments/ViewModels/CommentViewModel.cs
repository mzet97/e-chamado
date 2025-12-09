namespace EChamado.Server.Application.UseCases.Comments.ViewModels;

public record CommentViewModel(
    Guid Id,
    string Text,
    Guid OrderId,
    Guid UserId,
    string UserEmail,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc
);
