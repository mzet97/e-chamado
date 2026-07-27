using System.ComponentModel.DataAnnotations;

namespace EChamado.Server.Endpoints.Roles.DTOs;

/// <summary>
/// DTO para criação de role
/// </summary>
public class CreateRoleRequestDto
{
    /// <summary>
    /// Nome da role a ser criada
    /// </summary>
    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    [StringLength(100, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres")]
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// DTO para atualização de role
/// </summary>
public class UpdateRoleRequestDto
{
    /// <summary>
    /// ID da role a ser atualizada
    /// </summary>
    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Nome da role
    /// </summary>
    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    [StringLength(100, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres")]
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// DTO para busca de roles
/// </summary>
public class SearchRolesParametersDto
{
    /// <summary>
    /// Nome da role para filtro (opcional)
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Número da página para paginação
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "A página deve ser maior que zero")]
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Tamanho da página para paginação
    /// </summary>
    [Range(1, 100, ErrorMessage = "O tamanho da página deve estar entre 1 e 100")]
    public int PageSize { get; set; } = 10;
}

/// <summary>
/// DTO para exclusão de role
/// </summary>
public class DeleteRoleRequestDto
{
    /// <summary>
    /// ID da role a ser excluída
    /// </summary>
    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    public string Id { get; set; } = string.Empty;
}