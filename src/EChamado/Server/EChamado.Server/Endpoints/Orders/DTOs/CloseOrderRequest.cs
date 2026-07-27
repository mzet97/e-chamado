using System.ComponentModel.DataAnnotations;

namespace EChamado.Server.Endpoints.Orders.DTOs;

public class CloseOrderRequest
{
    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    public Guid OrderId { get; set; }

    public int? Evaluation { get; set; }
}
