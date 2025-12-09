using EChamado.Server.Application.Common.Behaviours;
using EChamado.Server.Application.UseCases.Categories.Notifications;
using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using EChamado.Shared.Services;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace EChamado.Server.Application.UseCases.Categories.Commands;

public class UpdateCategoryCommandHandler(
    IUnitOfWork unitOfWork,
    IAmACommandProcessor commandProcessor,
    IDateTimeProvider dateTimeProvider,
    ILogger<UpdateCategoryCommandHandler> logger) :
    RequestHandlerAsync<UpdateCategoryCommand>
{
    [RequestLogging(0, HandlerTiming.Before)]
    [RequestValidation(1, HandlerTiming.Before)]
    public override async Task<UpdateCategoryCommand> HandleAsync(UpdateCategoryCommand command, CancellationToken cancellationToken = default)
    {
        var category = await unitOfWork.Categories.GetByIdAsync(command.Id);

        if (category == null)
        {
            logger.LogError("Category {CategoryId} not found", command.Id);
            throw new NotFoundException($"Category {command.Id} not found");
        }

        category.Update(command.Name, command.Description, dateTimeProvider);

        if (!category.IsValid())
        {
            logger.LogError("Validate Category has error");
            throw new ValidationException("Validate Category has error", category.Errors);
        }

        await unitOfWork.BeginTransactionAsync();

        await unitOfWork.Categories.UpdateAsync(category);

        await unitOfWork.CommitAsync();

        await commandProcessor.PublishAsync(new UpdatedCategoryNotification(category.Id, category.Name, category.Description), cancellationToken: cancellationToken);

        logger.LogInformation("Category {CategoryId} updated successfully", command.Id);

        command.Result = new BaseResult();
        return await base.HandleAsync(command, cancellationToken);
    }
}
