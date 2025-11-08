using EChamado.Server.Application.UseCases.Categories.ViewModels;
using EChamado.Shared.Responses;
using EChamado.Shared.ViewModels;
using MediatR;

namespace EChamado.Server.Application.UseCases.Categories.Queries;

public class SearchCategoriesQuery : BaseSearch, IRequest<BaseResultList<CategoryViewModel>>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
