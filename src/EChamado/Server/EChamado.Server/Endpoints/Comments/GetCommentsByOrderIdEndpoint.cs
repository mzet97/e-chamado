using EChamado.Server.Application.Common.Messaging;
using EChamado.Server.Application.UseCases.Comments.Queries;
using EChamado.Server.Application.UseCases.Comments.ViewModels;
using EChamado.Shared.Responses;
using Paramore.Brighter;
using EChamado.Server.Common.Api;
using Microsoft.AspNetCore.Mvc;

namespace EChamado.Server.Endpoints.Comments;

public class GetCommentsByOrderIdEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/{orderId:guid}/comments", HandleAsync)
            .WithName("Obter comentários de um chamado")
            .Produces<BaseResultList<CommentViewModel>>();

    private static async Task<IResult> HandleAsync(
        Guid orderId,
        [FromServices] IAmACommandProcessor commandProcessor)
    {
        try
        {
            var query = new GetCommentsByOrderIdQuery(orderId);
            await commandProcessor.SendAsync(query);

            return query.Result.Success
                ? TypedResults.Ok(query.Result)
                : TypedResults.BadRequest(query.Result);
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(new BaseResultList<CommentViewModel>(
                new List<CommentViewModel>(),
                null,
                false,
                $"Erro interno: {ex.Message}"));
        }
    }
}
