using EChamado.Server.Application.UseCases.Categories.ViewModels;
using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EChamado.Server.Application.UseCases.SubCategories.Queries;

public class GetSubCategoryByIdQueryHandler(
    IUnitOfWork unitOfWork,
    ILogger<GetSubCategoryByIdQueryHandler> logger) :
    IRequestHandler<GetSubCategoryByIdQuery, BaseResult<SubCategoryViewModel>>
{
    public async Task<BaseResult<SubCategoryViewModel>> Handle(GetSubCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var subCategory = await unitOfWork.SubCategories.GetByIdAsync(request.SubCategoryId, cancellationToken);

        if (subCategory == null)
        {
            logger.LogError("SubCategory {SubCategoryId} not found", request.SubCategoryId);
            throw new NotFoundException($"SubCategory {request.SubCategoryId} not found");
        }

        var viewModel = new SubCategoryViewModel(
            subCategory.Id,
            subCategory.Name,
            subCategory.Description,
            subCategory.CategoryId
        );

        logger.LogInformation("SubCategory {SubCategoryId} retrieved successfully", request.SubCategoryId);

        return new BaseResult<SubCategoryViewModel>(viewModel);
    }
}
