using EChamado.Server.Application.UseCases.StatusTypes.Commands;
using EChamado.Shared.Responses;
using MediatR;

namespace EChamado.Server.Endpoints.StatusTypes;

public class DeleteStatusTypeEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapDelete("/{id:guid}", HandleAsync)
            .WithName("Deletar um status de chamado")
            .Produces<BaseResult>();

    private static async Task<IResult> HandleAsync(
        IMediator mediator,
        Guid id)
    {
        var command = new DeleteStatusTypeCommand(id);
        var result = await mediator.Send(command);

        if (result.Success)
            return TypedResults.Ok(result);

        return TypedResults.BadRequest(result);
    }
}
