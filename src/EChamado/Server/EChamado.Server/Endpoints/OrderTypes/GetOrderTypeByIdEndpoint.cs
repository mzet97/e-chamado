using EChamado.Server.Application.UseCases.OrderTypes.Queries;
using EChamado.Server.Application.UseCases.OrderTypes.ViewModels;
using EChamado.Shared.Responses;
using MediatR;

namespace EChamado.Server.Endpoints.OrderTypes;

public class GetOrderTypeByIdEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/{id:guid}", HandleAsync)
            .WithName("Obter tipo de chamado por ID")
            .Produces<BaseResult<OrderTypeViewModel>>();

    private static async Task<IResult> HandleAsync(
        IMediator mediator,
        Guid id)
    {
        var query = new GetOrderTypeByIdQuery(id);
        var result = await mediator.Send(query);

        if (result.Success)
            return TypedResults.Ok(result);

        return TypedResults.BadRequest(result);
    }
}
