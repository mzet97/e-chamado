using EChamado.Server.Application.UseCases.Comments.Commands;
using EChamado.Shared.Responses;
using MediatR;

namespace EChamado.Server.Endpoints.Comments;

public class DeleteCommentEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapDelete("/comments/{id:guid}", HandleAsync)
            .WithName("Deletar um comentário")
            .Produces<BaseResult>();

    private static async Task<IResult> HandleAsync(
        IMediator mediator,
        Guid id)
    {
        var command = new DeleteCommentCommand(id);
        var result = await mediator.Send(command);

        if (result.Success)
            return TypedResults.Ok(result);

        return TypedResults.BadRequest(result);
    }
}
