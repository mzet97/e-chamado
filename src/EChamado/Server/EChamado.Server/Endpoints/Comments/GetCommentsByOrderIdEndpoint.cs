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
        [FromServices] IAmACommandProcessor commandProcessor,
        Guid orderId)
    {
        var query = new GetCommentsByOrderIdQuery(orderId);
        var result = await commandProcessor.SendWithResultAsync(query);

        if (result.Success)
            return TypedResults.Ok(result);

        return TypedResults.BadRequest(result);
    }
}
