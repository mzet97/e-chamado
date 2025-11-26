using System.ComponentModel.DataAnnotations;

namespace EChamado.Server.Endpoints.Orders.DTOs;

public class SearchOrdersRequest
{
    [StringLength(200, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres")]
    public string? Title { get; set; }

    [StringLength(2000, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres")]
    public string? Description { get; set; }

    public Guid? StatusTypeId { get; set; }
    public Guid? TypeId { get; set; }
    public Guid? CategoryId { get; set; }
    public Guid? SubCategoryId { get; set; }
    public Guid? DepartmentId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? AssignedToUserId { get; set; }
    public string? CreatedByUserId { get; set; }
    public bool? IsOverdue { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "A página deve ser maior que zero")]
    public int PageIndex { get; set; } = 1;

    [Range(1, 100, ErrorMessage = "O tamanho da página deve estar entre 1 e 100")]
    public int PageSize { get; set; } = 10;
}
