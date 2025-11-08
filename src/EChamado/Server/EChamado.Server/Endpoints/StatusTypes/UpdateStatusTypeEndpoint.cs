using EChamado.Server.Application.UseCases.StatusTypes.Commands;
using EChamado.Shared.Responses;
using MediatR;

namespace EChamado.Server.Endpoints.StatusTypes;

public class UpdateStatusTypeEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPut("/{id:guid}", HandleAsync)
            .WithName("Atualizar um status de chamado")
            .Produces<BaseResult>();

    private static async Task<IResult> HandleAsync(
        IMediator mediator,
        Guid id,
        UpdateStatusTypeRequest request)
    {
        var command = new UpdateStatusTypeCommand(
            id,
            request.Name,
            request.Description
        );

        var result = await mediator.Send(command);

        if (result.Success)
            return TypedResults.Ok(result);

        return TypedResults.BadRequest(result);
    }
}

public record UpdateStatusTypeRequest(
    string Name,
    string Description
);
