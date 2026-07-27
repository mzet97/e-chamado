using System.ComponentModel.DataAnnotations;

namespace EChamado.Server.Endpoints.StatusTypes.DTOs;

public class DeleteStatusTypeRequest
{
    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    public Guid Id { get; set; }
}
