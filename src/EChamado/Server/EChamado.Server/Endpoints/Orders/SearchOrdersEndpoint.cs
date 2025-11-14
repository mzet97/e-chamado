using EChamado.Server.Application.Common.Messaging;
using EChamado.Server.Application.UseCases.Orders.Queries;
using EChamado.Server.Application.UseCases.Orders.ViewModels;
using EChamado.Server.Endpoints.Orders.DTOs;
using EChamado.Server.Common.Api;
using EChamado.Shared.Responses;
using Paramore.Brighter;
using Microsoft.AspNetCore.Mvc;

namespace EChamado.Server.Endpoints.Orders;

public class SearchOrdersEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/", HandleAsync)
            .WithName("Buscar ordens")
            .Produces<BaseResultList<OrderListViewModel>>();

    public static async Task<IResult> HandleAsync(
        [AsParameters] SearchOrdersRequest request,
        [FromServices] IAmACommandProcessor commandProcessor)
    {
        try
        {
            var query = request.ToQuery();
            await commandProcessor.SendAsync(query);

            return query.Result.Success
                ? TypedResults.Ok(query.Result)
                : TypedResults.BadRequest(query.Result);
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(new BaseResultList<OrderListViewModel>(
                data: new List<OrderListViewModel>(),
                pagedResult: new PagedResult { CurrentPage = 0, PageCount = 0, PageSize = 10, RowCount = 0 },
                success: false,
                message: $"Erro interno: {ex.Message}"));
        }
    }
}
