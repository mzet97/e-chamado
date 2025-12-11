using EChamado.Server.Application.Common.Behaviours;
using EChamado.Server.Application.UseCases.SubCategories.Notifications;
using EChamado.Server.Domain.Domains.Orders.Entities;
using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using EChamado.Shared.Services;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace EChamado.Server.Application.UseCases.SubCategories.Commands.Handlers;

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
        var entity = SubCategory.Create(command.Name, command.Description, command.CategoryId, dateTimeProvider);

        if (!entity.IsValid())
        {
            logger.LogError("Validate SubCategory has error");
            throw new ValidationException("Validate SubCategory has error", entity.Errors);
        }

        await unitOfWork.BeginTransactionAsync();

        await unitOfWork.SubCategories.AddAsync(entity);

        await unitOfWork.CommitAsync();

        await commandProcessor.PublishAsync(new CreatedSubCategoryNotification(entity.Id, entity.CategoryId, entity.Name, entity.Description), cancellationToken: cancellationToken);

        logger.LogInformation("SubCategory {SubCategoryId} created successfully", entity.Id);

        command.Result = new BaseResult<Guid>(entity.Id);
        return await base.HandleAsync(command, cancellationToken);
    }
}
