using System.ComponentModel.DataAnnotations;

namespace EChamado.Server.Endpoints.Categories.DTOs;

/// <summary>
/// DTO para criação de categoria
/// </summary>
public class CreateCategoryRequestDto
{
    /// <summary>
    /// Nome da categoria
    /// </summary>
    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    [StringLength(100, ErrorMessage = "O campo {0} deve ter entre {2} e {1} caracteres", MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Descrição da categoria
    /// </summary>
    [StringLength(500, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres")]
    public string? Description { get; set; }
}

/// <summary>
/// DTO para atualização de categoria
/// </summary>
public class UpdateCategoryRequestDto
{
    /// <summary>
    /// ID da categoria
    /// </summary>
    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Nome da categoria
    /// </summary>
    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    [StringLength(100, ErrorMessage = "O campo {0} deve ter entre {2} e {1} caracteres", MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Descrição da categoria
    /// </summary>
    [StringLength(500, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres")]
    public string? Description { get; set; }
}

/// <summary>
/// DTO para parâmetros de busca de categorias
/// </summary>
public class SearchCategoriesParametersDto
{
    /// <summary>
    /// Nome da categoria para filtro
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Descrição da categoria para filtro
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Índice da página
    /// </summary>
    public int PageIndex { get; set; } = 0;

    /// <summary>
    /// Tamanho da página
    /// </summary>
    public int PageSize { get; set; } = 10;
}