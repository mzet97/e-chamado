using EChamado.Server.Application.UseCases.Orders.Queries;
using EChamado.Server.Application.UseCases.Orders.ViewModels;
using EChamado.Server.Common.Api;
using EChamado.Shared.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EChamado.Server.Endpoints.Orders;

/// <summary>
/// Endpoint para busca de orders com Gridify
/// Suporta filtros avançados, ordenação e paginação dinâmica
/// </summary>
public class GridifyOrdersEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/gridify", HandleAsync)
            .WithName("Buscar ordens com Gridify")
            .WithDescription("Busca orders com suporte a filtros, ordenação e paginação dinâmica usando Gridify")
            .Produces<BaseResultList<OrderViewModel>>();

    public static async Task<IResult> HandleAsync(
        [AsParameters] GridifyOrderQuery query,
        [FromServices] IMediator mediator)
    {
        try
        {
            var result = await mediator.Send(query);

            return result.Success
                ? TypedResults.Ok(result)
                : TypedResults.BadRequest(result);
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(new BaseResultList<OrderViewModel>(
                data: new List<OrderViewModel>(),
                pagedResult: PagedResult.Create(1, 10, 0),
                success: false,
                message: $"Erro interno: {ex.Message}"));
        }
    }
}
