using System.ComponentModel.DataAnnotations;

namespace EChamado.Server.Endpoints.Users.DTOs;

public class GetByIdUserRequest
{
    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    public Guid Id { get; set; }
}
