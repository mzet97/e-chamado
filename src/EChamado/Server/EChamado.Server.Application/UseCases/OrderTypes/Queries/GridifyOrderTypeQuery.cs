using EChamado.Server.Application.Common;
using EChamado.Server.Application.UseCases.OrderTypes.ViewModels;

namespace EChamado.Server.Application.UseCases.OrderTypes.Queries;

/// <summary>
/// Query para busca de order types com Gridify
/// Suporta filtros avançados, ordenação e paginação usando sintaxe Gridify
/// </summary>
public class GridifyOrderTypeQuery : GridifySearchQuery<OrderTypeViewModel>
{
    /// <summary>
    /// Filtro por ID do order type
    /// </summary>
    public Guid? Id { get; set; }

    /// <summary>
    /// Filtro por nome do order type
    /// Suporta operadores Gridify: name=*incidente*, name^=Req, name$=Mudança
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Filtro por descrição do order type
    /// Suporta operadores Gridify: description=*urgente*, description^=Tipo
    /// </summary>
    public string? Description { get; set; }
}
