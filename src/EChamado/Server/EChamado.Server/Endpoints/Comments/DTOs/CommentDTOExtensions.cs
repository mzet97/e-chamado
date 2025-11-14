using EChamado.Server.Application.UseCases.Comments.Commands;

namespace EChamado.Server.Endpoints.Comments.DTOs;

public static class CommentDTOExtensions
{
    public static CreateCommentCommand ToCommand(this CreateCommentRequest request)
    {
        return new CreateCommentCommand(
            request.Description,
            request.OrderId,
            request.UserId,
            request.UserEmail
        );
    }

    public static DeleteCommentCommand ToCommand(this DeleteCommentRequest request)
    {
        return new DeleteCommentCommand(request.Id);
    }
}
