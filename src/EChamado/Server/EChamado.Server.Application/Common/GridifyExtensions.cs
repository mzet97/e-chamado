using Gridify;
using Microsoft.EntityFrameworkCore;
using EChamado.Shared.Responses;

namespace EChamado.Server.Application.Common;

/// <summary>
/// Extension methods para aplicar Gridify em queries do Entity Framework
/// </summary>
public static class GridifyExtensions
{
    /// <summary>
    /// Aplica filtros, ordenação e paginação usando Gridify de forma completa
    /// </summary>
    /// <typeparam name="T">Tipo da entidade</typeparam>
    /// <param name="query">Query IQueryable</param>
    /// <param name="gridifyQuery">Parâmetros do Gridify</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado paginado com metadados</returns>
    public static async Task<BaseResultList<T>> ApplyGridifyAsync<T>(
        this IQueryable<T> query,
        IGridifyQuery gridifyQuery,
        CancellationToken cancellationToken = default) where T : class
    {
        // 1. Aplica filtros do Gridify
        var filteredQuery = query.ApplyFiltering(gridifyQuery);

        // 2. Aplica ordenação do Gridify
        var orderedQuery = filteredQuery.ApplyOrdering(gridifyQuery);

        // 3. Conta o total de registros APÓS aplicar filtros (importante para performance)
        var totalCount = await orderedQuery.CountAsync(cancellationToken);

        // 4. Calcula informações de paginação
        var pageSize = gridifyQuery.PageSize;
        var currentPage = gridifyQuery.Page;

        // 5. Cria o PagedResult com os metadados
        var pagedResult = PagedResult.Create(currentPage, pageSize, totalCount);

        // 6. Aplica paginação (skip/take)
        var pagedQuery = orderedQuery.ApplyPaging(gridifyQuery);

        // 7. Materializa a query (executa no banco de dados)
        var items = await pagedQuery.ToListAsync(cancellationToken);

        // 8. Retorna o resultado com os dados e metadados de paginação
        return new BaseResultList<T>(items, pagedResult);
    }

    /// <summary>
    /// Aplica apenas filtros e ordenação sem paginação
    /// Útil quando precisa do resultado completo filtrado
    /// </summary>
    /// <typeparam name="T">Tipo da entidade</typeparam>
    /// <param name="query">Query IQueryable</param>
    /// <param name="gridifyQuery">Parâmetros do Gridify</param>
    /// <returns>Query filtrada e ordenada (ainda não materializada)</returns>
    public static IQueryable<T> ApplyGridifyFiltering<T>(
        this IQueryable<T> query,
        IGridifyQuery gridifyQuery) where T : class
    {
        return query
            .ApplyFiltering(gridifyQuery)
            .ApplyOrdering(gridifyQuery);
    }
}
