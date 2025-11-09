using EChamado.Server.Application.UseCases.Comments.Queries;
using EChamado.Server.Application.UseCases.Comments.ViewModels;
using EChamado.Shared.Responses;
using MediatR;

namespace EChamado.Server.Endpoints.Comments;

public class GetCommentsByOrderIdEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/{orderId:guid}/comments", HandleAsync)
            .WithName("Obter comentários de um chamado")
            .Produces<BaseResultList<CommentViewModel>>();

    private static async Task<IResult> HandleAsync(
        IMediator mediator,
        Guid orderId)
    {
        var query = new GetCommentsByOrderIdQuery(orderId);
        var result = await mediator.Send(query);

        if (result.Success)
            return TypedResults.Ok(result);

        return TypedResults.BadRequest(result);
    }
}
