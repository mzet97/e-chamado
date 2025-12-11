using EChamado.Server.Application.Common.Behaviours;
using EChamado.Server.Application.UseCases.SubCategories.Notifications;
using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using EChamado.Shared.Services;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace EChamado.Server.Application.UseCases.SubCategories.Commands.Handlers;

public class UpdateSubCategoryCommandHandler(
    IUnitOfWork unitOfWork,
    IAmACommandProcessor commandProcessor,
    IDateTimeProvider dateTimeProvider,
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

        subCategory.Update(command.Name, command.Description, command.CategoryId, dateTimeProvider);

        if (!subCategory.IsValid())
        {
            logger.LogError("Validate SubCategory has error");
            throw new ValidationException("Validate SubCategory has error", subCategory.Errors);
        }

        await unitOfWork.BeginTransactionAsync();

        await unitOfWork.SubCategories.UpdateAsync(subCategory);

        await unitOfWork.CommitAsync();

        await commandProcessor.PublishAsync(new UpdatedSubCategoryNotification(subCategory.Id, subCategory.CategoryId, subCategory.Name, subCategory.Description), cancellationToken: cancellationToken);

        logger.LogInformation("SubCategory {SubCategoryId} updated successfully", command.Id);

        command.Result = new BaseResult();
        return await base.HandleAsync(command, cancellationToken);
    }
}
