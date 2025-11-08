using EChamado.Server.Application.UseCases.Orders.Commands;
using EChamado.Server.Common.Api;
using EChamado.Shared.Responses;
using MediatR;

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
        IMediator mediator,
        Guid id,
        ChangeStatusRequest request)
    {
        var command = new ChangeStatusOrderCommand(id, request.StatusId);

        var result = await mediator.Send(command);

        if (result.Success)
        {
            return TypedResults.Ok(result);
        }

        return TypedResults.BadRequest(result);
    }
}

public record ChangeStatusRequest(Guid StatusId);
