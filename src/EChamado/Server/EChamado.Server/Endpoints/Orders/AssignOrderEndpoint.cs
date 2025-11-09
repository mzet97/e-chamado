using EChamado.Server.Application.Common.Messaging;
using EChamado.Server.Application.UseCases.Orders.Commands;
using EChamado.Server.Common.Api;
using EChamado.Shared.Responses;
using Paramore.Brighter;

namespace EChamado.Server.Endpoints.Orders;

public class AssignOrderEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    => app.MapPost("/{id}/assign", HandleAsync)
        .WithName("Atribuir chamado a usuário")
        .WithSummary("Atribuir chamado a usuário")
        .WithDescription("Atribuir chamado a usuário")
        .WithOrder(5)
        .Produces<BaseResult>();

    private static async Task<IResult> HandleAsync(
        IAmACommandProcessor commandProcessor,
        Guid id,
        AssignOrderRequest request)
    {
        var command = new AssignOrderCommand(id, request.AssignedToUserId);

        var result = await commandProcessor.Send(command);

        if (result.Success)
        {
            return TypedResults.Ok(result);
        }

        return TypedResults.BadRequest(result);
    }
}

public record AssignOrderRequest(Guid AssignedToUserId);
