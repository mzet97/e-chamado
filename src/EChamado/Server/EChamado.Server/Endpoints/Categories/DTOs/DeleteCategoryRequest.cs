using System.ComponentModel.DataAnnotations;

namespace EChamado.Server.Endpoints.Categories.DTOs;

public class DeleteCategoryRequest
{
    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    public Guid Id { get; set; }
}
