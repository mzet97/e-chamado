using EChamado.Server.Application.Common.Messaging;
using EChamado.Server.Application.UseCases.StatusTypes.Commands;
using EChamado.Shared.Responses;
using Paramore.Brighter;

namespace EChamado.Server.Endpoints.StatusTypes;

public class DeleteStatusTypeEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapDelete("/{id:guid}", HandleAsync)
            .WithName("Deletar um status de chamado")
            .Produces<BaseResult>();

    private static async Task<IResult> HandleAsync(
        IAmACommandProcessor commandProcessor,
        Guid id)
    {
        var command = new DeleteStatusTypeCommand(id);
        var result = await commandProcessor.Send(command);

        if (result.Success)
            return TypedResults.Ok(result);

        return TypedResults.BadRequest(result);
    }
}
