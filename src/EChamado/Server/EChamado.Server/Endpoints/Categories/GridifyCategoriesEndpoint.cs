using EChamado.Server.Application.UseCases.Categories.Queries;
using EChamado.Server.Application.UseCases.Categories.ViewModels;
using EChamado.Server.Common.Api;
using EChamado.Shared.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EChamado.Server.Endpoints.Categories;

/// <summary>
/// Endpoint para busca de categories com Gridify
/// Suporta filtros avançados, ordenação e paginação dinâmica
/// </summary>
public class GridifyCategoriesEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/gridify", HandleAsync)
            .WithName("Buscar categorias com Gridify")
            .WithDescription("Busca categories com suporte a filtros, ordenação e paginação dinâmica usando Gridify")
            .Produces<BaseResultList<CategoryViewModel>>();

    public static async Task<IResult> HandleAsync(
        [AsParameters] GridifyCategoryQuery query,
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
            return TypedResults.BadRequest(new BaseResultList<CategoryViewModel>(
                data: new List<CategoryViewModel>(),
                pagedResult: PagedResult.Create(1, 10, 0),
                success: false,
                message: $"Erro interno: {ex.Message}"));
        }
    }
}
