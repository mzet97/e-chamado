using EChamado.Server.Application.Common.Behaviours;
using EChamado.Server.Application.UseCases.Categories.Notifications;
using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace EChamado.Server.Application.UseCases.Categories.Commands;

public class UpdateSubCategoryCommandHandler(
    IUnitOfWork unitOfWork,
    IAmACommandProcessor commandProcessor,
    ILogger<UpdateSubCategoryCommandHandler> logger) :
    RequestHandlerAsync<UpdateSubCategoryCommand>
{
    [RequestLogging(0, HandlerTiming.Before)]
    [RequestValidation(1, HandlerTiming.Before)]
    public override async Task<UpdateSubCategoryCommand> HandleAsync(UpdateSubCategoryCommand command, CancellationToken cancellationToken = default)
    {
        var subCategory = await unitOfWork.SubCategories.GetByIdAsync(command.Id);

        if (subCategory == null)
        {
            logger.LogError("SubCategory {SubCategoryId} not found", command.Id);
            throw new NotFoundException($"SubCategory {command.Id} not found");
        }

        var category = await unitOfWork.Categories.GetByIdAsync(command.CategoryId);

        if (category == null)
        {
            logger.LogError("Category {CategoryId} not found", command.CategoryId);
            throw new NotFoundException($"Category {command.CategoryId} not found");
        }

        subCategory.Update(command.Name, command.Description, command.CategoryId);

        if (!subCategory.IsValid())
        {
            logger.LogError("Validate SubCategory has error");
            throw new ValidationException("Validate SubCategory has error", subCategory.GetErrors());
        }

        await unitOfWork.BeginTransactionAsync();

        await unitOfWork.SubCategories.UpdateAsync(subCategory);

        await unitOfWork.CommitAsync();

        await commandProcessor.PublishAsync(new UpdatedSubCategoryNotification(subCategory.Id, subCategory.Name, subCategory.Description), cancellationToken: cancellationToken);

        logger.LogInformation("SubCategory {SubCategoryId} updated successfully", command.Id);

        command.Result = new BaseResult();
        return await base.HandleAsync(command, cancellationToken);
    }
}
