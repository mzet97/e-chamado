using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EChamado.Server.Application.UseCases.Categories.Commands;

public class DeleteSubCategoryCommandHandler(
    IUnitOfWork unitOfWork,
    ILogger<DeleteSubCategoryCommandHandler> logger) :
    IRequestHandler<DeleteSubCategoryCommand, BaseResult>
{
    public async Task<BaseResult> Handle(DeleteSubCategoryCommand request, CancellationToken cancellationToken)
    {
        var subCategory = await unitOfWork.SubCategories.GetByIdAsync(request.SubCategoryId, cancellationToken);

        if (subCategory == null)
        {
            logger.LogError("SubCategory {SubCategoryId} not found", request.SubCategoryId);
            throw new NotFoundException($"SubCategory {request.SubCategoryId} not found");
        }

        await unitOfWork.BeginTransactionAsync();

        await unitOfWork.SubCategories.DeleteAsync(subCategory, cancellationToken);

        await unitOfWork.CommitAsync();

        logger.LogInformation("SubCategory {SubCategoryId} deleted successfully", request.SubCategoryId);

        return new BaseResult();
    }
}
