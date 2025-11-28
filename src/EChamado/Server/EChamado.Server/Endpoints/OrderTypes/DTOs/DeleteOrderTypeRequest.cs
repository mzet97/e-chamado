using System.ComponentModel.DataAnnotations;

namespace EChamado.Server.Endpoints.OrderTypes.DTOs;

public class DeleteOrderTypeRequest
{
    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    public Guid Id { get; set; }
}
