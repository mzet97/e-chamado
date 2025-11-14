using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace EChamado.Server.Endpoints.Departments.DTOs;

public class DeleteDepartmentRequest
{
    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    public IEnumerable<Guid> Ids { get; set; } = new List<Guid>();
}
