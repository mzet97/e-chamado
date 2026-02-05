using EChamado.Server.Application.UseCases.Categories.ViewModels;
using EChamado.Server.Application.UseCases.SubCategories.ViewModels;
using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace EChamado.Server.Application.UseCases.Categories.Queries.Handlers;

public class GetCategoryByIdQueryHandler(
    IUnitOfWork unitOfWork,
    ILogger<GetCategoryByIdQueryHandler> logger) :
    RequestHandlerAsync<GetCategoryByIdQuery>
{
    public override async Task<GetCategoryByIdQuery> HandleAsync(GetCategoryByIdQuery query, CancellationToken cancellationToken = default)
    {
        var category = await unitOfWork.Categories.GetByIdAsync(query.CategoryId);

        if (category == null)
        {
            logger.LogError("Category {CategoryId} not found", query.CategoryId);
            throw new NotFoundException($"Category {query.CategoryId} not found");
        }

        var viewModel = new CategoryViewModel(
            category.Id,
            category.Name,
            category.Description,
            category.CreatedAtUtc,
            category.UpdatedAtUtc,
            category.DeletedAtUtc,
            category.IsDeleted,
            category.SubCategories.Select(sc => new SubCategoryViewModel(
                sc.Id,
                sc.Name,
                sc.Description,
                sc.CategoryId,
                sc.CreatedAtUtc,
                sc.UpdatedAtUtc,
                sc.DeletedAtUtc,
                sc.IsDeleted
            )).ToList()
        );

        logger.LogInformation("Category {CategoryId} retrieved successfully", query.CategoryId);

        query.Result = new BaseResult<CategoryViewModel>(viewModel);

        return await base.HandleAsync(query, cancellationToken);
    }
}
