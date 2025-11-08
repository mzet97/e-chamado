using EChamado.Server.Application.UseCases.Categories.ViewModels;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EChamado.Server.Application.UseCases.Categories.Queries;

public class GetAllCategoriesQueryHandler(
    IUnitOfWork unitOfWork,
    ILogger<GetAllCategoriesQueryHandler> logger) :
    IRequestHandler<GetAllCategoriesQuery, BaseResultList<CategoryViewModel>>
{
    public async Task<BaseResultList<CategoryViewModel>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await unitOfWork.Categories.GetAllAsync(cancellationToken);

        if (!string.IsNullOrEmpty(request.SearchText))
        {
            categories = categories.Where(c =>
                c.Name.Contains(request.SearchText, StringComparison.OrdinalIgnoreCase) ||
                c.Description.Contains(request.SearchText, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        var totalCount = categories.Count;

        var items = categories
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(c => new CategoryViewModel(
                c.Id,
                c.Name,
                c.Description,
                c.SubCategories.Select(sc => new SubCategoryViewModel(
                    sc.Id,
                    sc.Name,
                    sc.Description,
                    sc.CategoryId
                )).ToList()
            ))
            .ToList();

        logger.LogInformation("Get all categories returned {Count} results", items.Count);

        return new BaseResultList<CategoryViewModel>(items, totalCount, request.PageNumber, request.PageSize);
    }
}
