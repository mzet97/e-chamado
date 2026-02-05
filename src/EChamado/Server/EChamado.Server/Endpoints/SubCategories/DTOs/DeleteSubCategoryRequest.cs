using System.ComponentModel.DataAnnotations;

namespace EChamado.Server.Endpoints.SubCategories.DTOs;

public class DeleteSubCategoryRequest
{
    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    public Guid Id { get; set; }
}
