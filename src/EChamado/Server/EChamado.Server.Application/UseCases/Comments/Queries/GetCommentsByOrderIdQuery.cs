using EChamado.Server.Application.UseCases.Comments.ViewModels;
using EChamado.Shared.Responses;
using MediatR;

namespace EChamado.Server.Application.UseCases.Comments.Queries;

public record GetCommentsByOrderIdQuery(Guid OrderId) : IRequest<BaseResultList<CommentViewModel>>;
