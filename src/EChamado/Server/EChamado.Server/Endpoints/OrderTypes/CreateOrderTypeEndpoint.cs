using EChamado.Server.Application.UseCases.OrderTypes.Commands;
using EChamado.Shared.Responses;
using MediatR;

namespace EChamado.Server.Endpoints.OrderTypes;

public class CreateOrderTypeEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPost("/", HandleAsync)
            .WithName("Criar um novo tipo de chamado")
            .Produces<BaseResult<Guid>>();

    private static async Task<IResult> HandleAsync(
        IMediator mediator,
        CreateOrderTypeRequest request)
    {
        var command = new CreateOrderTypeCommand(
            request.Name,
            request.Description
        );

        var result = await mediator.Send(command);

        if (result.Success)
            return TypedResults.Ok(result);

        return TypedResults.BadRequest(result);
    }
}

public record CreateOrderTypeRequest(
    string Name,
    string Description
);
