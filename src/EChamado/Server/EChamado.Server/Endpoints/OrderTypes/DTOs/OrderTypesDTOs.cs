using System.ComponentModel.DataAnnotations;

namespace EChamado.Server.Endpoints.OrderTypes.DTOs;

/// <summary>
/// DTO para criação de tipo de chamado
/// </summary>
public class CreateOrderTypeRequestDto
{
    /// <summary>
    /// Nome do tipo de chamado
    /// </summary>
    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    [StringLength(100, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Descrição do tipo de chamado
    /// </summary>
    [StringLength(500, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres")]
    public string? Description { get; set; }
}

/// <summary>
/// DTO para atualização de tipo de chamado
/// </summary>
public class UpdateOrderTypeRequestDto
{
    /// <summary>
    /// Nome do tipo de chamado
    /// </summary>
    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    [StringLength(100, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Descrição do tipo de chamado
    /// </summary>
    [StringLength(500, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres")]
    public string? Description { get; set; }
}

/// <summary>
/// DTO para parâmetros de busca de tipos de chamados
/// </summary>
public class SearchOrderTypesParametersDto
{
    /// <summary>
    /// Nome do tipo para filtragem
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Descrição do tipo para filtragem
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Página para paginação (padrão: 1)
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "O campo {0} deve ser maior que zero")]
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// Tamanho da página para paginação (padrão: 10)
    /// </summary>
    [Range(1, 100, ErrorMessage = "O campo {0} deve estar entre {1} e {2}")]
    public int PageSize { get; set; } = 10;
}