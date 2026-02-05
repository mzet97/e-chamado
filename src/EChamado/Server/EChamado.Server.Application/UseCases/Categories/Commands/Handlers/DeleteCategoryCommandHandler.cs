using EChamado.Server.Application.Common.Behaviours;
using EChamado.Server.Application.UseCases.Categories.Notifications;
using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace EChamado.Server.Application.UseCases.Categories.Commands.Handlers;

public class DeleteCategoryCommandHandler(
    IUnitOfWork unitOfWork,
    IAmACommandProcessor commandProcessor,
    ILogger<DeleteCategoryCommandHandler> logger) :
    RequestHandlerAsync<DeleteCategoryCommand>
{
    [RequestLogging(0, HandlerTiming.Before)]
    [RequestValidation(1, HandlerTiming.Before)]
    public override async Task<DeleteCategoryCommand> HandleAsync(DeleteCategoryCommand command, CancellationToken cancellationToken = default)
    {
        var category = await unitOfWork.Categories.GetByIdAsync(command.CategoryId);

        if (category == null)
        {
            logger.LogError("Category {CategoryId} not found", command.CategoryId);
            throw new NotFoundException($"Category {command.CategoryId} not found");
        }

        await unitOfWork.BeginTransactionAsync();

        await unitOfWork.Categories.RemoveAsync(category.Id);

        await unitOfWork.CommitAsync();

        await commandProcessor.PublishAsync(new DeletedCategoryNotification(category.Id, category.Name, category.Description), cancellationToken: cancellationToken);

        logger.LogInformation("Category {CategoryId} deleted successfully", command.CategoryId);

        command.Result = new BaseResult();
        return await base.HandleAsync(command, cancellationToken);
    }
}
