using EChamado.Server.Application.Common.Messaging;
using EChamado.Server.Application.UseCases.Categories.ViewModels;
using EChamado.Shared.Responses;

namespace EChamado.Server.Application.UseCases.Categories.Queries;

public class GetCategoryByIdQuery : BrighterRequest<BaseResult<CategoryViewModel>>
{
    public Guid CategoryId { get; set; }

    public GetCategoryByIdQuery()
    {
    }

    public GetCategoryByIdQuery(Guid categoryId)
    {
        CategoryId = categoryId;
    }
}
