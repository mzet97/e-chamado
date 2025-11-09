using EChamado.Server.Application.Common.Messaging;
using EChamado.Server.Application.UseCases.Orders.Commands;
using EChamado.Server.Common.Api;
using EChamado.Shared.Responses;
using Paramore.Brighter;

namespace EChamado.Server.Endpoints.Orders;

public class ChangeStatusOrderEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    => app.MapPost("/{id}/status", HandleAsync)
        .WithName("Alterar status do chamado")
        .WithSummary("Alterar status do chamado")
        .WithDescription("Alterar status do chamado")
        .WithOrder(7)
        .Produces<BaseResult>();

    private static async Task<IResult> HandleAsync(
        IAmACommandProcessor commandProcessor,
        Guid id,
        ChangeStatusRequest request)
    {
        var command = new ChangeStatusOrderCommand(id, request.StatusId);

        var result = await commandProcessor.Send(command);

        if (result.Success)
        {
            return TypedResults.Ok(result);
        }

        return TypedResults.BadRequest(result);
    }
}

public record ChangeStatusRequest(Guid StatusId);
