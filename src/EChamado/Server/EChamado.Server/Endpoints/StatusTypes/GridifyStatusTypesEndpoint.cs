using EChamado.Server.Application.UseCases.StatusTypes.Queries;
using EChamado.Server.Application.UseCases.StatusTypes.ViewModels;
using EChamado.Server.Common.Api;
using EChamado.Shared.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EChamado.Server.Endpoints.StatusTypes;

/// <summary>
/// Endpoint para busca de status types com Gridify
/// Suporta filtros avançados, ordenação e paginação dinâmica
/// </summary>
public class GridifyStatusTypesEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/gridify", HandleAsync)
            .WithName("Buscar tipos de status com Gridify")
            .WithDescription("Busca status types com suporte a filtros, ordenação e paginação dinâmica usando Gridify")
            .Produces<BaseResultList<StatusTypeViewModel>>();

    public static async Task<IResult> HandleAsync(
        [AsParameters] GridifyStatusTypeQuery query,
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
            return TypedResults.BadRequest(new BaseResultList<StatusTypeViewModel>(
                data: new List<StatusTypeViewModel>(),
                pagedResult: PagedResult.Create(1, 10, 0),
                success: false,
                message: $"Erro interno: {ex.Message}"));
        }
    }
}
