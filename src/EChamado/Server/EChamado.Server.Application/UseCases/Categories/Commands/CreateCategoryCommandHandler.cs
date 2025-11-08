using EChamado.Server.Domain.Domains.Categories;
using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EChamado.Server.Application.UseCases.Categories.Commands;

public class CreateCategoryCommandHandler(
    IUnitOfWork unitOfWork,
    ILogger<CreateCategoryCommandHandler> logger) :
    IRequestHandler<CreateCategoryCommand, BaseResult<Guid>>
{
    public async Task<BaseResult<Guid>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var entity = Category.Create(request.Name, request.Description);

        if (!entity.IsValid())
        {
            logger.LogError("Validate Category has error");
            throw new ValidationException("Validate Category has error", entity.GetErrors());
        }

        await unitOfWork.BeginTransactionAsync();

        await unitOfWork.Categories.AddAsync(entity);

        await unitOfWork.CommitAsync();

        logger.LogInformation("Category {CategoryId} created successfully", entity.Id);

        return new BaseResult<Guid>(entity.Id);
    }
}
