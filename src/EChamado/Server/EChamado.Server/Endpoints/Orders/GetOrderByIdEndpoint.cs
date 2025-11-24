using EChamado.Server.Application.Common.Messaging;
using EChamado.Server.Application.UseCases.Orders.Queries;
using EChamado.Server.Application.UseCases.Orders.ViewModels;
using EChamado.Server.Common.Api;
using EChamado.Shared.Responses;
using Paramore.Brighter;
using Microsoft.AspNetCore.Mvc;

namespace EChamado.Server.Endpoints.Orders;

public class GetOrderByIdEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/{id:guid}", HandleAsync)
            .WithName("Buscar ordem por ID")
            .Produces<BaseResult<OrderViewModel>>();

    public static async Task<IResult> HandleAsync(
        Guid id,
        [FromServices] IAmACommandProcessor commandProcessor)
    {
        try
        {
            var query = new GetOrderByIdQuery(id);
            await commandProcessor.SendAsync(query);

            return query.Result.Success
                ? TypedResults.Ok(query.Result)
                : TypedResults.NotFound(query.Result);
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(new BaseResult<OrderViewModel>(
                data: null,
                success: false,
                message: $"Erro interno: {ex.Message}"));
        }
    }
}
