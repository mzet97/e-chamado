using EChamado.Server.Application.Common;
using EChamado.Server.Application.UseCases.SubCategories.ViewModels;

namespace EChamado.Server.Application.UseCases.SubCategories.Queries;

/// <summary>
/// Query para busca de sub-categories com Gridify
/// Suporta filtros avançados, ordenação e paginação usando sintaxe Gridify
/// </summary>
public class GridifySubCategoryQuery : GridifySearchQuery<SubCategoryViewModel>
{
    /// <summary>
    /// Filtro por ID da sub-category
    /// </summary>
    public Guid? Id { get; set; }

    /// <summary>
    /// Filtro por nome da sub-category
    /// Suporta operadores Gridify: name=*TI*, name^=Suporte, name$=Vendas
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Filtro por descrição da sub-category
    /// Suporta operadores Gridify: description=*atendimento*, description^=Dep
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Filtro por ID da categoria
    /// </summary>
    public Guid? CategoryId { get; set; }

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
