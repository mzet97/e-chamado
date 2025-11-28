using System.ComponentModel.DataAnnotations;

namespace EChamado.Server.Endpoints.Orders.DTOs;

public class CreateOrderRequest
{
    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    [StringLength(200, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    [StringLength(2000, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    public Guid TypeId { get; set; }

    public Guid? CategoryId { get; set; }
    public Guid? SubCategoryId { get; set; }
    public Guid? DepartmentId { get; set; }
    public DateTime? DueDate { get; set; }

    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    public Guid RequestingUserId { get; set; }

    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    [EmailAddress(ErrorMessage = "O campo {0} deve conter um email válido")]
    public string RequestingUserEmail { get; set; } = string.Empty;
}
