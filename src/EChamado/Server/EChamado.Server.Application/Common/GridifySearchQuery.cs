using Gridify;
using EChamado.Shared.Responses;
using MediatR;

namespace EChamado.Server.Application.Common;

/// <summary>
/// Classe base abstrata para queries que suportam Gridify
/// Fornece funcionalidades de filtro, ordenação e paginação dinâmica
/// </summary>
/// <typeparam name="TResult">Tipo do resultado da query</typeparam>
public abstract class GridifySearchQuery<TResult> : IGridifyQuery, IRequest<BaseResultList<TResult>>
{
    /// <summary>
    /// Filtros no formato Gridify
    /// Exemplos:
    /// - name=*john* (contains)
    /// - age>18 (greater than)
    /// - price>=100,price<=1000 (range)
    /// - isDeleted=false (equals)
    /// </summary>
    public string? Filter { get; set; }

    /// <summary>
    /// Ordenação no formato Gridify
    /// Exemplos:
    /// - name asc
    /// - price desc
    /// - name asc, createdAt desc (múltiplas ordenações)
    /// </summary>
    public string? OrderBy { get; set; }

    /// <summary>
    /// Número da página (começa em 1)
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Tamanho da página (quantidade de itens por página)
    /// </summary>
    public int PageSize { get; set; } = 10;
}
