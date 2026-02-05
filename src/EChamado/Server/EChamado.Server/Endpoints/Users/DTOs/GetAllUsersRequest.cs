using System.ComponentModel.DataAnnotations;

namespace EChamado.Server.Endpoints.Users.DTOs;

public class GetAllUsersRequest
{
    [StringLength(255, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres")]
    public string? Email { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "A página deve ser maior que zero")]
    public int PageIndex { get; set; } = 1;

    [Range(1, 100, ErrorMessage = "O tamanho da página deve estar entre 1 e 100")]
    public int PageSize { get; set; } = 10;
}
