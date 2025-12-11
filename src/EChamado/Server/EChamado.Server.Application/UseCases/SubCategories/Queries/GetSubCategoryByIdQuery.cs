using EChamado.Server.Application.Common.Messaging;
using EChamado.Server.Application.UseCases.SubCategories.ViewModels;
using EChamado.Shared.Responses;

namespace EChamado.Server.Application.UseCases.SubCategories.Queries;

public class GetSubCategoryByIdQuery : BrighterRequest<BaseResult<SubCategoryViewModel>>
{
    public Guid Id { get; set; }

    public GetSubCategoryByIdQuery()
    {
    }

    public GetSubCategoryByIdQuery(Guid id)
    {
        Id = id;
    }
}
