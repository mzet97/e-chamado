using EChamado.Server.Application.Common.Messaging;
using EChamado.Server.Application.UseCases.Categories.ViewModels;
using EChamado.Shared.Responses;
using EChamado.Shared.ViewModels;

namespace EChamado.Server.Application.UseCases.SubCategories.Queries;

public class SearchSubCategoriesQuery : BaseSearch, BrighterRequest<BaseResultList<SubCategoryViewModel>>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid? CategoryId { get; set; }
}
