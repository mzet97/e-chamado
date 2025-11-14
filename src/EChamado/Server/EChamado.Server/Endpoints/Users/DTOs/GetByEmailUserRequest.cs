using System.ComponentModel.DataAnnotations;

namespace EChamado.Server.Endpoints.Users.DTOs;

public class GetByEmailUserRequest
{
    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    [EmailAddress(ErrorMessage = "O campo {0} deve conter um email válido")]
    public string Email { get; set; } = string.Empty;
}
