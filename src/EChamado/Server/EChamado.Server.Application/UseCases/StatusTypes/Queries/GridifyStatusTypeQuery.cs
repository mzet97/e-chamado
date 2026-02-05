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

    /// <summary>
    /// Filtro por data de criação
    /// Suporta operadores: createdAt>2024-01-01, createdAt<2024-12-31
    /// </summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// Filtro por data de atualização
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Filtro por status de deleção
    /// </summary>
    public bool? IsDeleted { get; set; }
}
