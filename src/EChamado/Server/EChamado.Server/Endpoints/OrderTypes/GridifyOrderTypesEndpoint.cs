using EChamado.Server.Application.UseCases.OrderTypes.Queries;
using EChamado.Server.Application.UseCases.OrderTypes.ViewModels;
using EChamado.Server.Common.Api;
using EChamado.Shared.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EChamado.Server.Endpoints.OrderTypes;

/// <summary>
/// Endpoint para busca de order types com Gridify
/// Suporta filtros avançados, ordenação e paginação dinâmica
/// </summary>
public class GridifyOrderTypesEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/gridify", HandleAsync)
            .WithName("Buscar tipos de ordem com Gridify")
            .WithDescription("Busca order types com suporte a filtros, ordenação e paginação dinâmica usando Gridify")
            .Produces<BaseResultList<OrderTypeViewModel>>();

    public static async Task<IResult> HandleAsync(
        [AsParameters] GridifyOrderTypeQuery query,
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
            return TypedResults.BadRequest(new BaseResultList<OrderTypeViewModel>(
                data: new List<OrderTypeViewModel>(),
                pagedResult: PagedResult.Create(1, 10, 0),
                success: false,
                message: $"Erro interno: {ex.Message}"));
        }
    }
}
