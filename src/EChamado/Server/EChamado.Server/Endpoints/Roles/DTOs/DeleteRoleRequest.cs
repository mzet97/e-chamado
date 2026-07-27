using System.ComponentModel.DataAnnotations;

namespace EChamado.Server.Endpoints.Roles.DTOs;

public class DeleteRoleRequest
{
    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    public Guid Id { get; set; }
}
