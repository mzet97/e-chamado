using EChamado.Server.Application.UseCases.Categories.Notifications;
using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EChamado.Server.Application.UseCases.Categories.Commands;

public class DeleteCategoryCommandHandler(
    IUnitOfWork unitOfWork,
    IMediator mediator,
    ILogger<DeleteCategoryCommandHandler> logger) :
    IRequestHandler<DeleteCategoryCommand, BaseResult>
{
    public async Task<BaseResult> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await unitOfWork.Categories.GetByIdAsync(request.CategoryId, cancellationToken);

        if (category == null)
        {
            logger.LogError("Category {CategoryId} not found", request.CategoryId);
            throw new NotFoundException($"Category {request.CategoryId} not found");
        }

        await unitOfWork.BeginTransactionAsync();

        await unitOfWork.Categories.DeleteAsync(category, cancellationToken);

        await unitOfWork.CommitAsync();

        await mediator.Publish(new DeletedCategoryNotification(category.Id, category.Name, category.Description));

        logger.LogInformation("Category {CategoryId} deleted successfully", request.CategoryId);

        return new BaseResult();
    }
}
