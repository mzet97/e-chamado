using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EChamado.Server.Application.UseCases.Categories.Commands;

public class UpdateCategoryCommandHandler(
    IUnitOfWork unitOfWork,
    ILogger<UpdateCategoryCommandHandler> logger) :
    IRequestHandler<UpdateCategoryCommand, BaseResult>
{
    public async Task<BaseResult> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await unitOfWork.Categories.GetByIdAsync(request.Id, cancellationToken);

        if (category == null)
        {
            logger.LogError("Category {CategoryId} not found", request.Id);
            throw new NotFoundException($"Category {request.Id} not found");
        }

        category.Update(request.Name, request.Description);

        if (!category.IsValid())
        {
            logger.LogError("Validate Category has error");
            throw new ValidationException("Validate Category has error", category.GetErrors());
        }

        await unitOfWork.BeginTransactionAsync();

        await unitOfWork.Categories.UpdateAsync(category, cancellationToken);

        await unitOfWork.CommitAsync();

        logger.LogInformation("Category {CategoryId} updated successfully", request.Id);

        return new BaseResult();
    }
}
