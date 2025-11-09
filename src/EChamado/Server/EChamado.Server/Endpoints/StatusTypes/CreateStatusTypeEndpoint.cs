using EChamado.Server.Application.Common.Messaging;
using EChamado.Server.Application.UseCases.StatusTypes.Commands;
using EChamado.Shared.Responses;
using Paramore.Brighter;

namespace EChamado.Server.Endpoints.StatusTypes;

public class CreateStatusTypeEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPost("/", HandleAsync)
            .WithName("Criar um novo status de chamado")
            .Produces<BaseResult<Guid>>();

    private static async Task<IResult> HandleAsync(
        IAmACommandProcessor commandProcessor,
        CreateStatusTypeRequest request)
    {
        var command = new CreateStatusTypeCommand(
            request.Name,
            request.Description
        );

        var result = await commandProcessor.Send(command);

        if (result.Success)
            return TypedResults.Ok(result);

        return TypedResults.BadRequest(result);
    }
}

public record CreateStatusTypeRequest(
    string Name,
    string Description
);
