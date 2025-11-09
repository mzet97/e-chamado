using EChamado.Server.Domain.Domains.Orders.Entities;
using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using MediatR;
using Microsoft.Extensions.Logging;
using EChamado.Server.Application.UseCases.Categories.Notifications;

namespace EChamado.Server.Application.UseCases.Categories.Commands;

public class CreateSubCategoryCommandHandler(
    IUnitOfWork unitOfWork,
    IMediator mediator,
    ILogger<CreateSubCategoryCommandHandler> logger) :
    IRequestHandler<CreateSubCategoryCommand, BaseResult<Guid>>
{
    public async Task<BaseResult<Guid>> Handle(CreateSubCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await unitOfWork.Categories.GetByIdAsync(request.CategoryId, cancellationToken);

        if (category == null)
        {
            logger.LogError("Category {CategoryId} not found", request.CategoryId);
            throw new NotFoundException($"Category {request.CategoryId} not found");
        }

        var entity = SubCategory.Create(request.Name, request.Description, request.CategoryId);

        if (!entity.IsValid())
        {
            logger.LogError("Validate SubCategory has error");
            throw new ValidationException("Validate SubCategory has error", entity.GetErrors());
        }

        await unitOfWork.BeginTransactionAsync();

        await unitOfWork.SubCategories.AddAsync(entity);

        await unitOfWork.CommitAsync();

        await mediator.Publish(new CreatedSubCategoryNotification(entity.Id, entity.Name, entity.Description, entity.CategoryId));

        logger.LogInformation("SubCategory {SubCategoryId} created successfully for Category {CategoryId}",
            entity.Id, request.CategoryId);

        return new BaseResult<Guid>(entity.Id);
    }
}
