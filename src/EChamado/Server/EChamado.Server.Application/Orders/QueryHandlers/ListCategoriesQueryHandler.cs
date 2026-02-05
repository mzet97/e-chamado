using EChamado.Server.Application.Orders.Queries;
using EChamado.Server.Application.UseCases.Categories.ViewModels;
using EChamado.Server.Application.UseCases.SubCategories.ViewModels;
using EChamado.Server.Domain.Domains.Orders.Entities;
using EChamado.Server.Domain.Repositories;
using Paramore.Darker;

namespace EChamado.Server.Application.Orders.QueryHandlers;

public sealed class ListCategoriesQueryHandler : QueryHandlerAsync<ListCategoriesQuery, IEnumerable<CategoryViewModel>>
{
    private readonly IUnitOfWork _unitOfWork;

    public ListCategoriesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public override async Task<IEnumerable<CategoryViewModel>> ExecuteAsync(ListCategoriesQuery query, CancellationToken cancellationToken = default)
    {
        var categories = await _unitOfWork.Categories.GetAllAsync();
        return categories.Select(category => new CategoryViewModel(
            category.Id,
            category.Name,
            category.Description,
            category.CreatedAtUtc,
            category.UpdatedAtUtc,
            category.DeletedAtUtc,
            category.IsDeleted,
            (category.SubCategories ?? Enumerable.Empty<SubCategory>())
                .Select(subCategory => new SubCategoryViewModel(
                    subCategory.Id,
                    subCategory.Name,
                    subCategory.Description,
                    subCategory.CategoryId,
                    subCategory.CreatedAtUtc,
                    subCategory.UpdatedAtUtc,
                    subCategory.DeletedAtUtc,
                    subCategory.IsDeleted))
                .ToList()));
    }
}
