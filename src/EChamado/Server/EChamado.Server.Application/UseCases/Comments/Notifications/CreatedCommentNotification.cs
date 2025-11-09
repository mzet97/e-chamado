using MediatR;

namespace EChamado.Server.Application.UseCases.Comments.Notifications;

public record CreatedCommentNotification(
    Guid Id,
    string Text,
    Guid OrderId,
    Guid UserId,
    string UserEmail
) : INotification;
