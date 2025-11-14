using System.ComponentModel.DataAnnotations;

namespace EChamado.Server.Endpoints.Orders.DTOs;

public class ChangeStatusOrderRequest
{
    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    public Guid OrderId { get; set; }

    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    public Guid StatusTypeId { get; set; }
}
