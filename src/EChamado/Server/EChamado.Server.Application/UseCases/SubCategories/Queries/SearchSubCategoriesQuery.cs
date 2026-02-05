using EChamado.Server.Application.Common.Messaging;
using EChamado.Server.Application.UseCases.SubCategories.ViewModels;
using EChamado.Shared.Responses;

namespace EChamado.Server.Application.UseCases.SubCategories.Queries;

public class SearchSubCategoriesQuery : BrighterRequest<BaseResultList<SubCategoryViewModel>>
{
    // BaseSearch properties
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? Order { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    // Specific properties
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid? CategoryId { get; set; }
}
