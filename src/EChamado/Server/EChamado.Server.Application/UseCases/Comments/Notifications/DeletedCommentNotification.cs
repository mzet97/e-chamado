using MediatR;

namespace EChamado.Server.Application.UseCases.Comments.Notifications;

public record DeletedCommentNotification(
    Guid Id,
    Guid OrderId
) : INotification;
