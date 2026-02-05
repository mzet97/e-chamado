using System.ComponentModel.DataAnnotations;

namespace EChamado.Server.Endpoints.Comments.DTOs;

public class CreateCommentRequest
{
    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    public Guid OrderId { get; set; }

    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    [StringLength(2000, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    public Guid UserId { get; set; }

    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    [EmailAddress(ErrorMessage = "O campo {0} deve conter um email válido")]
    public string UserEmail { get; set; } = string.Empty;
}
