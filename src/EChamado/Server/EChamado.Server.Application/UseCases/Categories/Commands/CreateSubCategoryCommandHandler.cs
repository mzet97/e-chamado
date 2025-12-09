using EChamado.Server.Application.Common.Behaviours;
using EChamado.Server.Application.UseCases.Categories.Notifications;
using EChamado.Server.Domain.Domains.Orders.Entities;
using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using EChamado.Shared.Services;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace EChamado.Server.Application.UseCases.Categories.Commands;

public class CreateSubCategoryCommandHandler(
    IUnitOfWork unitOfWork,
    IAmACommandProcessor commandProcessor,
    IDateTimeProvider dateTimeProvider,
    ILogger<CreateSubCategoryCommandHandler> logger) :
    RequestHandlerAsync<CreateSubCategoryCommand>
{
    [RequestLogging(0, HandlerTiming.Before)]
    [RequestValidation(1, HandlerTiming.Before)]
    public override async Task<CreateSubCategoryCommand> HandleAsync(CreateSubCategoryCommand command, CancellationToken cancellationToken = default)
    {
        var category = await unitOfWork.Categories.GetByIdAsync(command.CategoryId);

        if (category == null)
        {
            logger.LogError("Category {CategoryId} not found", command.CategoryId);
            throw new NotFoundException($"Category {command.CategoryId} not found");
        }

        var entity = SubCategory.Create(command.Name, command.Description, command.CategoryId, dateTimeProvider);

        if (!entity.IsValid())
        {
            logger.LogError("Validate SubCategory has error");
            throw new ValidationException("Validate SubCategory has error", entity.Errors);
        }

        await unitOfWork.BeginTransactionAsync();

        await unitOfWork.SubCategories.AddAsync(entity);

        await unitOfWork.CommitAsync();

        await commandProcessor.PublishAsync(new CreatedSubCategoryNotification(entity.Id, entity.Name, entity.Description, entity.CategoryId), cancellationToken: cancellationToken);

        logger.LogInformation("SubCategory {SubCategoryId} created successfully for Category {CategoryId}",
            entity.Id, command.CategoryId);

        command.Result = new BaseResult<Guid>(entity.Id);
        return await base.HandleAsync(command, cancellationToken);
    }
}
