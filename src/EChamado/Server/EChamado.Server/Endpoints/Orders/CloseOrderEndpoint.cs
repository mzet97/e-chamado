using EChamado.Server.Application.UseCases.Orders.Commands;
using EChamado.Server.Common.Api;
using EChamado.Shared.Responses;
using MediatR;

namespace EChamado.Server.Endpoints.Orders;

public class CloseOrderEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    => app.MapPost("/{id}/close", HandleAsync)
        .WithName("Fechar chamado")
        .WithSummary("Fechar chamado")
        .WithDescription("Fechar chamado")
        .WithOrder(6)
        .Produces<BaseResult>();

    private static async Task<IResult> HandleAsync(
        IMediator mediator,
        Guid id,
        CloseOrderRequest request)
    {
        var command = new CloseOrderCommand(id, request.Evaluation);

        var result = await mediator.Send(command);

        if (result.Success)
        {
            return TypedResults.Ok(result);
        }

        return TypedResults.BadRequest(result);
    }
}

public record CloseOrderRequest(int? Evaluation);
