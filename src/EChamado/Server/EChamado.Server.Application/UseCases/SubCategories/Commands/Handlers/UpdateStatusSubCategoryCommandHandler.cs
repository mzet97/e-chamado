using EChamado.Server.Application.Common.Behaviours;
using EChamado.Server.Application.UseCases.SubCategories.Notifications;
using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace EChamado.Server.Application.UseCases.SubCategories.Commands.Handlers;

public class UpdateStatusSubCategoryCommandHandler(
    IUnitOfWork unitOfWork,
    IAmACommandProcessor commandProcessor,
    ILogger<UpdateStatusSubCategoryCommandHandler> logger) :
    RequestHandlerAsync<UpdateStatusSubCategoryCommand>
{
    [RequestLogging(0, HandlerTiming.Before)]
    [RequestValidation(1, HandlerTiming.Before)]
    public override async Task<UpdateStatusSubCategoryCommand> HandleAsync(
        UpdateStatusSubCategoryCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command == null)
        {
            logger.LogError("UpdateStatusSubCategoryCommand is null");
            throw new ArgumentNullException(nameof(command));
        }

        await unitOfWork.BeginTransactionAsync();

        foreach (var item in command.Items)
        {
            var entity = await unitOfWork
                .SubCategories
                .GetByIdAsync(item.Id);

            if (entity == null)
            {
                logger.LogError("SubCategory not found");
                throw new NotFoundException("SubCategory not found");
            }

            await unitOfWork.SubCategories
                .ActiveOrDisableAsync(item.Id, item.Active);

            await unitOfWork.CommitAsync();

            await commandProcessor.PublishAsync(
                new DisabledSubCategoryNotification(
                    entity.Id,
                    entity.CategoryId,
                    entity.Name,
                    entity.Description), cancellationToken: cancellationToken);
        }

        logger.LogInformation("SubCategories status updated successfully");

        command.Result = new BaseResult(true, "Status das SubCategories atualizados com sucesso");

        return await base.HandleAsync(command, cancellationToken);
    }
}
