using EChamado.Server.Application.Common;
using EChamado.Server.Application.UseCases.StatusTypes.ViewModels;

namespace EChamado.Server.Application.UseCases.StatusTypes.Queries;

/// <summary>
/// Query para busca de status types com Gridify
/// Suporta filtros avançados, ordenação e paginação usando sintaxe Gridify
/// </summary>
public class GridifyStatusTypeQuery : GridifySearchQuery<StatusTypeViewModel>
{
    /// <summary>
    /// Filtro por ID do status type
    /// </summary>
    public Guid? Id { get; set; }

    /// <summary>
    /// Filtro por nome do status type
    /// Suporta operadores Gridify: name=*aberto*, name^=Em, name$=Fechado
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Filtro por descrição do status type
    /// Suporta operadores Gridify: description=*andamento*, description^=Status
    /// </summary>
    public string? Description { get; set; }
}
