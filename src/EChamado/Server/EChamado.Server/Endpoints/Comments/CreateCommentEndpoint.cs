using EChamado.Server.Application.Common.Messaging;
using EChamado.Server.Application.UseCases.Comments.Commands;
using EChamado.Shared.Responses;
using Paramore.Brighter;

namespace EChamado.Server.Endpoints.Comments;

public class CreateCommentEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPost("/{orderId:guid}/comments", HandleAsync)
            .WithName("Criar um novo comentário em um chamado")
            .Produces<BaseResult<Guid>>();

    private static async Task<IResult> HandleAsync(
        IAmACommandProcessor commandProcessor,
        Guid orderId,
        CreateCommentRequest request)
    {
        var command = new CreateCommentCommand(
            request.Text,
            orderId,
            request.UserId,
            request.UserEmail
        );

        var result = await commandProcessor.Send(command);

        if (result.Success)
            return TypedResults.Ok(result);

        return TypedResults.BadRequest(result);
    }
}

public record CreateCommentRequest(
    string Text,
    Guid UserId,
    string UserEmail
);
