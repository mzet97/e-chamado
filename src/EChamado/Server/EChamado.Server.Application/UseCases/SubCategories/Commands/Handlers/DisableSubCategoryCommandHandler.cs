using EChamado.Server.Application.Common.Behaviours;
using EChamado.Server.Application.UseCases.SubCategories.Notifications;
using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace EChamado.Server.Application.UseCases.SubCategories.Commands.Handlers;

public class DisableSubCategoryCommandHandler(
    IUnitOfWork unitOfWork,
    IAmACommandProcessor commandProcessor,
    ILogger<DisableSubCategoryCommandHandler> logger) :
    RequestHandlerAsync<DisableSubCategoryCommand>
{
    [RequestLogging(0, HandlerTiming.Before)]
    [RequestValidation(1, HandlerTiming.Before)]
    public override async Task<DisableSubCategoryCommand> HandleAsync(
        DisableSubCategoryCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command == null)
        {
            logger.LogError("DisableSubCategoryCommand is null");
            throw new ArgumentNullException(nameof(command));
        }

        var entity = await unitOfWork
            .SubCategories
            .GetByIdAsync(command.Id);

        if (entity == null)
        {
            logger.LogError("SubCategory not found");
            throw new NotFoundException("SubCategory not found");
        }

        await unitOfWork.BeginTransactionAsync();

        await unitOfWork.SubCategories
            .DisableAsync(command.Id);

        await unitOfWork.CommitAsync();

        await commandProcessor.PublishAsync(
            new DisabledSubCategoryNotification(
                entity.Id,
                entity.CategoryId,
                entity.Name,
                entity.Description), cancellationToken: cancellationToken);

        logger.LogInformation("SubCategory {SubCategoryId} disabled successfully", command.Id);

        command.Result = new BaseResult(true, "SubCategory desativada com sucesso");

        return await base.HandleAsync(command, cancellationToken);
    }
}
