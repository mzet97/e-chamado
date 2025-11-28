using EChamado.Server.Application.Common;
using EChamado.Server.Application.UseCases.Categories.ViewModels;

namespace EChamado.Server.Application.UseCases.Categories.Queries;

/// <summary>
/// Query para busca de categories com Gridify
/// Suporta filtros avançados, ordenação e paginação usando sintaxe Gridify
/// </summary>
public class GridifyCategoryQuery : GridifySearchQuery<CategoryViewModel>
{
    /// <summary>
    /// Filtro por ID da category
    /// </summary>
    public Guid? Id { get; set; }

    /// <summary>
    /// Filtro por nome da category
    /// Suporta operadores Gridify: name=*hardware*, name^=Soft, name$=ware
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Filtro por descrição da category
    /// Suporta operadores Gridify: description=*problema*, description^=Categoria
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
