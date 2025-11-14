using System.ComponentModel.DataAnnotations;

namespace EChamado.Server.Endpoints.Departments.DTOs;

/// <summary>
/// DTO para criação de departamento
/// </summary>
public class CreateDepartmentRequestDto
{
    /// <summary>
    /// Nome do departamento
    /// </summary>
    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    [StringLength(100, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Descrição do departamento
    /// </summary>
    [StringLength(500, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres")]
    public string? Description { get; set; }
}

/// <summary>
/// DTO para atualização de departamento
/// </summary>
public class UpdateDepartmentRequestDto
{
    /// <summary>
    /// ID do departamento a ser atualizado
    /// </summary>
    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Nome do departamento
    /// </summary>
    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    [StringLength(100, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Descrição do departamento
    /// </summary>
    [StringLength(500, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres")]
    public string? Description { get; set; }
}

/// <summary>
/// DTO para busca de departamentos
/// </summary>
public class SearchDepartmentsParametersDto
{
    /// <summary>
    /// Nome do departamento para filtro (opcional)
    /// </summary>
    [StringLength(100, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres")]
    public string? Name { get; set; }
    
    /// <summary>
    /// Descrição do departamento para filtro (opcional)
    /// </summary>
    [StringLength(500, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres")]
    public string? Description { get; set; }
    
    /// <summary>
    /// Número da página para paginação
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "A página deve ser maior que zero")]
    public int PageIndex { get; set; } = 1;
    
    /// <summary>
    /// Tamanho da página para paginação
    /// </summary>
    [Range(1, 100, ErrorMessage = "O tamanho da página deve estar entre 1 e 100")]
    public int PageSize { get; set; } = 10;
}

/// <summary>
/// DTO para atualização de status de departamentos
/// </summary>
public class UpdateStatusDepartmentRequestDto
{
    /// <summary>
    /// Lista de IDs dos departamentos para atualizar status
    /// </summary>
    [Required(ErrorMessage = "A lista de {0} é obrigatória")]
    public List<string> Ids { get; set; } = new();
    
    /// <summary>
    /// Status ativo/inativo
    /// </summary>
    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    public bool Active { get; set; }
}