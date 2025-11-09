using EChamado.Server.Application.Common.Messaging;
using EChamado.Server.Application.UseCases.OrderTypes.Commands;
using EChamado.Shared.Responses;
using Paramore.Brighter;

namespace EChamado.Server.Endpoints.OrderTypes;

public class UpdateOrderTypeEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPut("/{id:guid}", HandleAsync)
            .WithName("Atualizar um tipo de chamado")
            .Produces<BaseResult>();

    private static async Task<IResult> HandleAsync(
        IAmACommandProcessor commandProcessor,
        Guid id,
        UpdateOrderTypeRequest request)
    {
        var command = new UpdateOrderTypeCommand(
            id,
            request.Name,
            request.Description
        );

        var result = await commandProcessor.Send(command);

        if (result.Success)
            return TypedResults.Ok(result);

        return TypedResults.BadRequest(result);
    }
}

public record UpdateOrderTypeRequest(
    string Name,
    string Description
);
