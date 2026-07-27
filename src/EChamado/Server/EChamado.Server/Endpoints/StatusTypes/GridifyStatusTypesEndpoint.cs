using EChamado.Server.Application.UseCases.StatusTypes.Queries;
using EChamado.Server.Application.UseCases.StatusTypes.ViewModels;
using EChamado.Server.Common.Api;
using EChamado.Shared.Responses;
using Paramore.Darker;
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
        [FromServices] IQueryProcessor queryProcessor)
    {
        try
        {
            var result = await queryProcessor.ExecuteAsync(query);

            return result.Success
                ? TypedResults.Ok(result)
                : TypedResults.BadRequest(result);
        }
        catch (Exception)
        {
            return TypedResults.BadRequest(new BaseResultList<StatusTypeViewModel>(
                data: new List<StatusTypeViewModel>(),
                pagedResult: PagedResult.Create(1, 10, 0),
                success: false,
                message: "Erro ao processar a solicitacao."));
        }
    }
}
