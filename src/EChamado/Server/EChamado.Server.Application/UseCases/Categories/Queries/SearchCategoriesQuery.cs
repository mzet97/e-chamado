using EChamado.Server.Application.Common.Messaging;
using EChamado.Server.Application.UseCases.Categories.ViewModels;
using EChamado.Shared.Responses;
using EChamado.Shared.ViewModels;

namespace EChamado.Server.Application.UseCases.Categories.Queries;

public class SearchCategoriesQuery : BaseSearch, BrighterRequest<BaseResultList<CategoryViewModel>>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
