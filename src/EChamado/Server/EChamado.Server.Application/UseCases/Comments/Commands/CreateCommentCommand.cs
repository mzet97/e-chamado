using EChamado.Shared.Responses;
using MediatR;

namespace EChamado.Server.Application.UseCases.Comments.Commands;

public record CreateCommentCommand(
    string Text,
    Guid OrderId,
    Guid UserId,
    string UserEmail
) : IRequest<BaseResult<Guid>>;
