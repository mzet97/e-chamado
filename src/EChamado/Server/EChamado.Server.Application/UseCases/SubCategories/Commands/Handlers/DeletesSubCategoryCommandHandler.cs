using EChamado.Server.Application.Common.Behaviours;
using EChamado.Server.Application.UseCases.SubCategories.Notifications;
using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace EChamado.Server.Application.UseCases.SubCategories.Commands.Handlers;

public class DeletesSubCategoryCommandHandler(
    IUnitOfWork unitOfWork,
    IAmACommandProcessor commandProcessor,
    ILogger<DeletesSubCategoryCommandHandler> logger) :
    RequestHandlerAsync<DeletesSubCategoryCommand>
{
    [RequestLogging(0, HandlerTiming.Before)]
    [RequestValidation(1, HandlerTiming.Before)]
    public override async Task<DeletesSubCategoryCommand> HandleAsync(
        DeletesSubCategoryCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command == null)
        {
            logger.LogError("DeletesSubCategoryCommand is null");
            throw new ArgumentNullException(nameof(command));
        }

        await unitOfWork.BeginTransactionAsync();

        foreach (var id in command.Ids)
        {
            var entity = await unitOfWork
                .SubCategories
                .GetByIdAsync(id);

            if (entity == null)
            {
                logger.LogError("SubCategory not found");
                throw new NotFoundException("SubCategory not found");
            }

            await unitOfWork.SubCategories
                .RemoveAsync(id);

            await unitOfWork.CommitAsync();

            await commandProcessor.PublishAsync(
                new DeletedSubCategoryNotification(
                    entity.Id,
                    entity.CategoryId,
                    entity.Name,
                    entity.Description), cancellationToken: cancellationToken);
        }

        logger.LogInformation("SubCategories deleted successfully");

        command.Result = new BaseResult(true, "SubCategories deletadas com sucesso");

        return await base.HandleAsync(command, cancellationToken);
    }
}
