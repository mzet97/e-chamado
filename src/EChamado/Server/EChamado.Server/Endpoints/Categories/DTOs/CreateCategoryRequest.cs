using System.ComponentModel.DataAnnotations;

namespace EChamado.Server.Endpoints.Categories.DTOs;

public class CreateCategoryRequest
{
    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    [StringLength(100, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres")]
    public string? Description { get; set; }
}
