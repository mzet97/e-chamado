using System.ComponentModel.DataAnnotations;

namespace EChamado.Server.Endpoints.Comments.DTOs;

public class DeleteCommentRequest
{
    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    public Guid Id { get; set; }
}
