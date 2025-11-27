using EChamado.Server.Application.UseCases.Departments.Queries;
using EChamado.Server.Application.UseCases.Departments.ViewModels;
using EChamado.Server.Common.Api;
using EChamado.Shared.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EChamado.Server.Endpoints.Departments;

/// <summary>
/// Endpoint para busca de departments com Gridify
/// Suporta filtros avançados, ordenação e paginação dinâmica
/// </summary>
public class GridifyDepartmentsEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/gridify", HandleAsync)
            .WithName("Buscar departamentos com Gridify")
            .WithDescription("Busca departments com suporte a filtros, ordenação e paginação dinâmica usando Gridify")
            .Produces<BaseResultList<DepartmentViewModel>>();

    public static async Task<IResult> HandleAsync(
        [AsParameters] GridifyDepartmentQuery query,
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
            return TypedResults.BadRequest(new BaseResultList<DepartmentViewModel>(
                data: new List<DepartmentViewModel>(),
                pagedResult: PagedResult.Create(1, 10, 0),
                success: false,
                message: $"Erro interno: {ex.Message}"));
        }
    }
}
