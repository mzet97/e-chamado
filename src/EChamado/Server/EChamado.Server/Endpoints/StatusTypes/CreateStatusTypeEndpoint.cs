using EChamado.Server.Application.UseCases.StatusTypes.Commands;
using EChamado.Shared.Responses;
using MediatR;

namespace EChamado.Server.Endpoints.StatusTypes;

public class CreateStatusTypeEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPost("/", HandleAsync)
            .WithName("Criar um novo status de chamado")
            .Produces<BaseResult<Guid>>();

    private static async Task<IResult> HandleAsync(
        IMediator mediator,
        CreateStatusTypeRequest request)
    {
        var command = new CreateStatusTypeCommand(
            request.Name,
            request.Description
        );

        var result = await mediator.Send(command);

        if (result.Success)
            return TypedResults.Ok(result);

        return TypedResults.BadRequest(result);
    }
}

public record CreateStatusTypeRequest(
    string Name,
    string Description
);
