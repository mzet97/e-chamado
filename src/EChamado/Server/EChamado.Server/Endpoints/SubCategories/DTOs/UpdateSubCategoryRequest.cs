using System.ComponentModel.DataAnnotations;

namespace EChamado.Server.Endpoints.SubCategories.DTOs;

public class UpdateSubCategoryRequest
{
    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    public Guid CategoryId { get; set; }

    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    [StringLength(100, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres")]
    public string? Description { get; set; }
}
