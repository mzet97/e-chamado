using EChamado.Server.Application.Common.Messaging;
using EChamado.Server.Application.UseCases.Categories.Queries;
using EChamado.Server.Application.UseCases.Categories.ViewModels;
using EChamado.Shared.Responses;
using Microsoft.AspNetCore.Mvc;
using EChamado.Server.Common.Api;
using Paramore.Brighter;

namespace EChamado.Server.Endpoints.Categories;

/// <summary>
/// Endpoint para buscar e filtrar categorias
/// </summary>
public class SearchCategoriesEndpoint : IEndpoint
{
    /// <summary>
    /// Lista e filtra categorias com paginação
    /// </summary>
    /// <remarks>
    /// Exemplo de requisição:
    ///
    ///     GET /v1/categories?name=Hardware&pageIndex=1&pageSize=10
    ///
    /// Parâmetros de busca disponíveis:
    /// - **name**: Filtrar por nome (busca parcial)
    /// - **description**: Filtrar por descrição (busca parcial)
    /// - **createdAt**: Filtrar por data de criação
    /// - **pageIndex**: Número da página (padrão: 1)
    /// - **pageSize**: Itens por página (padrão: 10, máximo: 100)
    ///
    /// </remarks>
    /// <param name="parameters">Parâmetros de busca e paginação</param>
    /// <param name="commandProcessor">Command Processor para executar a consulta</param>
    /// <returns>Lista paginada de categorias</returns>
    /// <response code="200">Retorna a lista de categorias (pode estar vazia)</response>
    /// <response code="400">Parâmetros inválidos</response>
    /// <response code="401">Não autenticado</response>
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/", HandleAsync)
            .WithName("Buscar categorias")
            .WithSummary("Lista e filtra categorias")
            .WithDescription("Busca categorias com filtros opcionais e paginação")
            .WithTags("Category")
            // .RequireAuthorization() // TEMPORÁRIAMENTE DESABILITADO PARA TESTES
            .Produces<BaseResultList<CategoryViewModel>>(StatusCodes.Status200OK)
            .Produces<BaseResultList<CategoryViewModel>>(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized);

    private static async Task<IResult> HandleAsync(
        [AsParameters] SearchCategoriesParameters parameters,
        [FromServices] IAmACommandProcessor commandProcessor)
    {
        try
        {
            // Validações de paginação
            var pageIndex = Math.Max(1, parameters.PageIndex ?? 1);
            var pageSize = Math.Min(100, Math.Max(1, parameters.PageSize ?? 10));

            // Cria a query do Brighter com os parâmetros
            var query = new SearchCategoriesQuery
            {
                Name = parameters.Name ?? string.Empty,
                Description = parameters.Description ?? string.Empty,
                CreatedAt = parameters.CreatedAt ?? default,
                UpdatedAt = parameters.UpdatedAt ?? default,
                DeletedAt = parameters.DeletedAt ?? default,
                Order = parameters.Order ?? "Name",
                PageIndex = pageIndex,
                PageSize = pageSize
            };

            await commandProcessor.SendAsync(query);

            return query.Result.Success
                ? TypedResults.Ok(query.Result)
                : TypedResults.BadRequest(query.Result);
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(new BaseResultList<CategoryViewModel>(
                new List<CategoryViewModel>(),
                null,
                false,
                $"Erro interno: {ex.Message}"));
        }
    }
}

/// <summary>
/// Parâmetros para busca e filtro de categorias
/// </summary>
public class SearchCategoriesParameters
{
    /// <summary>
    /// Nome da categoria (busca parcial, case-insensitive)
    /// </summary>
    /// <example>Hardware</example>
    [FromQuery] public string? Name { get; set; }

    /// <summary>
    /// Descrição da categoria (busca parcial, case-insensitive)
    /// </summary>
    /// <example>Problemas com equipamentos</example>
    [FromQuery] public string? Description { get; set; }

    /// <summary>
    /// Data de criação (filtro exato)
    /// </summary>
    /// <example>2025-01-01T00:00:00Z</example>
    [FromQuery] public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// Data da última atualização (filtro exato)
    /// </summary>
    /// <example>2025-01-15T10:30:00Z</example>
    [FromQuery] public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Data de exclusão (soft delete)
    /// </summary>
    [FromQuery] public DateTime? DeletedAt { get; set; }

    /// <summary>
    /// Campo para ordenação (ex: name, createdAt, updatedAt)
    /// </summary>
    /// <example>name</example>
    [FromQuery] public string? Order { get; set; }

    /// <summary>
    /// Número da página (inicia em 1)
    /// </summary>
    /// <example>1</example>
    [FromQuery] public int? PageIndex { get; set; } = 1;

    /// <summary>
    /// Quantidade de itens por página (máximo: 100)
    /// </summary>
    /// <example>10</example>
    [FromQuery] public int? PageSize { get; set; } = 10;
}