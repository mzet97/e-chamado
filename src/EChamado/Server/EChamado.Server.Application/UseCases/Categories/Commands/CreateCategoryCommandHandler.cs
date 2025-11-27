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

public class CreateCategoryCommandHandler(
    IUnitOfWork unitOfWork,
    IAmACommandProcessor commandProcessor,
    IDateTimeProvider dateTimeProvider,
    ILogger<CreateCategoryCommandHandler> logger) :
    RequestHandlerAsync<CreateCategoryCommand>
{
    [RequestLogging(0, HandlerTiming.Before)]
    [RequestValidation(1, HandlerTiming.Before)]
    public override async Task<CreateCategoryCommand> HandleAsync(CreateCategoryCommand command, CancellationToken cancellationToken = default)
    {
        var entity = Category.Create(command.Name, command.Description, dateTimeProvider);

        if (!entity.IsValid())
        {
            logger.LogError("Validate Category has error");
            throw new ValidationException("Validate Category has error", entity.GetErrors());
        }

        await unitOfWork.BeginTransactionAsync();

        await unitOfWork.Categories.AddAsync(entity);

        await unitOfWork.CommitAsync();

        await commandProcessor.PublishAsync(new CreatedCategoryNotification(entity.Id, entity.Name, entity.Description), cancellationToken: cancellationToken);

        logger.LogInformation("Category {CategoryId} created successfully", entity.Id);

        command.Result = new BaseResult<Guid>(entity.Id);

        return await base.HandleAsync(command, cancellationToken);
    }
}
