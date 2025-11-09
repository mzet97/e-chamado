using EChamado.Shared.Responses;
using MediatR;

namespace EChamado.Server.Application.UseCases.Comments.Commands;

public record DeleteCommentCommand(Guid CommentId) : IRequest<BaseResult>;
