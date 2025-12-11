using EChamado.Server.Application.UseCases.SubCategories.ViewModels;
using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using Paramore.Brighter;
using Microsoft.Extensions.Logging;

namespace EChamado.Server.Application.UseCases.SubCategories.Queries.Handlers;

public class GetSubCategoryByIdQueryHandler(
    IUnitOfWork unitOfWork,
    ILogger<GetSubCategoryByIdQueryHandler> logger) :
    RequestHandlerAsync<GetSubCategoryByIdQuery>
{
    public override async Task<GetSubCategoryByIdQuery> HandleAsync(GetSubCategoryByIdQuery query, CancellationToken cancellationToken = default)
    {
        var subCategory = await unitOfWork.SubCategories.GetByIdAsync(query.Id);

        if (subCategory == null)
        {
            logger.LogError("SubCategory {SubCategoryId} not found", query.Id);
            throw new NotFoundException($"SubCategory {query.Id} not found");
        }

        var viewModel = new SubCategoryViewModel(
            subCategory.Id,
            subCategory.Name,
            subCategory.Description,
            subCategory.CategoryId,
            subCategory.CreatedAtUtc,
            subCategory.UpdatedAtUtc,
            subCategory.DeletedAtUtc,
            subCategory.IsDeleted
        );

        logger.LogInformation("SubCategory {SubCategoryId} retrieved successfully", query.Id);

        query.Result = new BaseResult<SubCategoryViewModel>(viewModel);

        return await base.HandleAsync(query, cancellationToken);
    }
}
