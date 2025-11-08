using EChamado.Server.Application.UseCases.OrderTypes.Commands;
using EChamado.Shared.Responses;
using MediatR;

namespace EChamado.Server.Endpoints.OrderTypes;

public class DeleteOrderTypeEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapDelete("/{id:guid}", HandleAsync)
            .WithName("Deletar um tipo de chamado")
            .Produces<BaseResult>();

    private static async Task<IResult> HandleAsync(
        IMediator mediator,
        Guid id)
    {
        var command = new DeleteOrderTypeCommand(id);
        var result = await mediator.Send(command);

        if (result.Success)
            return TypedResults.Ok(result);

        return TypedResults.BadRequest(result);
    }
}
