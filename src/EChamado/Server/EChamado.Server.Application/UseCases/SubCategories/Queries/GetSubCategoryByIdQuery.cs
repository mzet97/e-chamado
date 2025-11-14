using EChamado.Server.Application.Common.Messaging;
using EChamado.Server.Application.UseCases.Categories.ViewModels;
using EChamado.Shared.Responses;

namespace EChamado.Server.Application.UseCases.SubCategories.Queries;

public class GetSubCategoryByIdQuery : BrighterRequest<BaseResult<SubCategoryViewModel>>
{
    public Guid SubCategoryId { get; set; }

    public GetSubCategoryByIdQuery()
    {
    }

    public GetSubCategoryByIdQuery(Guid subCategoryId)
    {
        SubCategoryId = subCategoryId;
    }
}
