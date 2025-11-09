using EChamado.Server.Application.Common.Messaging;
using EChamado.Server.Application.UseCases.OrderTypes.Queries;
using EChamado.Server.Application.UseCases.OrderTypes.ViewModels;
using EChamado.Shared.Responses;
using Paramore.Brighter;

namespace EChamado.Server.Endpoints.OrderTypes;

public class GetOrderTypeByIdEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/{id:guid}", HandleAsync)
            .WithName("Obter tipo de chamado por ID")
            .Produces<BaseResult<OrderTypeViewModel>>();

    private static async Task<IResult> HandleAsync(
        IAmACommandProcessor commandProcessor,
        Guid id)
    {
        var query = new GetOrderTypeByIdQuery(id);
        var result = await commandProcessor.Send(query);

        if (result.Success)
            return TypedResults.Ok(result);

        return TypedResults.BadRequest(result);
    }
}
