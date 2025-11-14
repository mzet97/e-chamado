using EChamado.Server.Application.UseCases.Comments.Commands;
using EChamado.Server.Application.UseCases.Orders.Queries;
using EChamado.Server.Endpoints.Comments.DTOs;

namespace EChamado.Server.Endpoints.Comments.Extensions;

/// <summary>
/// Extensões para mapeamento entre DTOs e comandos/queries do Comments
/// </summary>
public static class CommentsDTOExtensions
{
    /// <summary>
    /// Converte CreateCommentRequestDto para CreateCommentCommand
    /// </summary>
    public static CreateCommentCommand ToCommand(this CreateCommentRequestDto requestDto, Guid orderId)
    {
        return new CreateCommentCommand
        {
            OrderId = orderId,
            Text = requestDto.Text,
            UserId = requestDto.UserId,
            UserEmail = requestDto.UserEmail ?? string.Empty
        };
    }
}