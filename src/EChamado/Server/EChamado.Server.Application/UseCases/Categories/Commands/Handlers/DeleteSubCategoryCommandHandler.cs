using EChamado.Server.Application.Common.Behaviours;
using EChamado.Server.Application.UseCases.Categories.Notifications;
using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace EChamado.Server.Application.UseCases.Categories.Commands.Handlers;

public class DeleteSubCategoryCommandHandler(
    IUnitOfWork unitOfWork,
    IAmACommandProcessor commandProcessor,
    ILogger<DeleteSubCategoryCommandHandler> logger) :
    RequestHandlerAsync<DeleteSubCategoryCommand>
{
    [RequestLogging(0, HandlerTiming.Before)]
    [RequestValidation(1, HandlerTiming.Before)]
    public override async Task<DeleteSubCategoryCommand> HandleAsync(DeleteSubCategoryCommand command, CancellationToken cancellationToken = default)
    {
        var subCategory = await unitOfWork.SubCategories.GetByIdAsync(command.SubCategoryId);

        if (subCategory == null)
        {
            logger.LogError("SubCategory {SubCategoryId} not found", command.SubCategoryId);
            throw new NotFoundException($"SubCategory {command.SubCategoryId} not found");
        }

        await unitOfWork.BeginTransactionAsync();

        await unitOfWork.SubCategories.RemoveAsync(subCategory.Id);

        await unitOfWork.CommitAsync();

        await commandProcessor.PublishAsync(new DeletedSubCategoryNotification(subCategory.Id, subCategory.Name, subCategory.Description), cancellationToken: cancellationToken);

        logger.LogInformation("SubCategory {SubCategoryId} deleted successfully", command.SubCategoryId);

        command.Result = new BaseResult();
        return await base.HandleAsync(command, cancellationToken);
    }
}
