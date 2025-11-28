using System.ComponentModel.DataAnnotations;

namespace EChamado.Server.Endpoints.Departments.DTOs;

public class DeleteDepartmentRequest
{
    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    public Guid Id { get; set; }
}
