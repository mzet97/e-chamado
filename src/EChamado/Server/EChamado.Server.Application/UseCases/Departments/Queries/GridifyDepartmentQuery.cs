using EChamado.Server.Application.Common;
using EChamado.Server.Application.UseCases.Departments.ViewModels;

namespace EChamado.Server.Application.UseCases.Departments.Queries;

/// <summary>
/// Query para busca de departments com Gridify
/// Suporta filtros avançados, ordenação e paginação usando sintaxe Gridify
/// </summary>
public class GridifyDepartmentQuery : GridifySearchQuery<DepartmentViewModel>
{
    /// <summary>
    /// Filtro por ID do department
    /// </summary>
    public Guid? Id { get; set; }

    /// <summary>
    /// Filtro por nome do department
    /// Suporta operadores Gridify: name=*TI*, name^=Suporte, name$=Vendas
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Filtro por descrição do department
    /// Suporta operadores Gridify: description=*atendimento*, description^=Dep
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
