using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using MediatR;
using Microsoft.Extensions.Logging;

using EChamado.Server.Application.UseCases.Categories.Notifications;

namespace EChamado.Server.Application.UseCases.Categories.Commands;

public class UpdateSubCategoryCommandHandler(
    IUnitOfWork unitOfWork,
    IMediator mediator,
    ILogger<UpdateSubCategoryCommandHandler> logger) :
    IRequestHandler<UpdateSubCategoryCommand, BaseResult>
{
    public async Task<BaseResult> Handle(UpdateSubCategoryCommand request, CancellationToken cancellationToken)
    {
        var subCategory = await unitOfWork.SubCategories.GetByIdAsync(request.Id, cancellationToken);

        if (subCategory == null)
        {
            logger.LogError("SubCategory {SubCategoryId} not found", request.Id);
            throw new NotFoundException($"SubCategory {request.Id} not found");
        }

        subCategory.Update(request.Name, request.Description);

        if (!subCategory.IsValid())
        {
            logger.LogError("Validate SubCategory has error");
            throw new ValidationException("Validate SubCategory has error", subCategory.GetErrors());
        }

        await unitOfWork.BeginTransactionAsync();

        await unitOfWork.SubCategories.UpdateAsync(subCategory, cancellationToken);

        await unitOfWork.CommitAsync();

        await mediator.Publish(new UpdatedSubCategoryNotification(subCategory.Id, subCategory.Name, subCategory.Description));

        logger.LogInformation("SubCategory {SubCategoryId} updated successfully", request.Id);

        return new BaseResult();
    }
}
