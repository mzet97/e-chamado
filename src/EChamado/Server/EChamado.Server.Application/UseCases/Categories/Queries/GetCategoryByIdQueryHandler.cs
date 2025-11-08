using EChamado.Server.Application.UseCases.Categories.ViewModels;
using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EChamado.Server.Application.UseCases.Categories.Queries;

public class GetCategoryByIdQueryHandler(
    IUnitOfWork unitOfWork,
    ILogger<GetCategoryByIdQueryHandler> logger) :
    IRequestHandler<GetCategoryByIdQuery, BaseResult<CategoryViewModel>>
{
    public async Task<BaseResult<CategoryViewModel>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await unitOfWork.Categories.GetByIdAsync(request.CategoryId, cancellationToken);

        if (category == null)
        {
            logger.LogError("Category {CategoryId} not found", request.CategoryId);
            throw new NotFoundException($"Category {request.CategoryId} not found");
        }

        var viewModel = new CategoryViewModel(
            category.Id,
            category.Name,
            category.Description,
            category.SubCategories.Select(sc => new SubCategoryViewModel(
                sc.Id,
                sc.Name,
                sc.Description,
                sc.CategoryId
            )).ToList()
        );

        logger.LogInformation("Category {CategoryId} retrieved successfully", request.CategoryId);

        return new BaseResult<CategoryViewModel>(viewModel);
    }
}
