using EChamado.Server.Domain.Entities;
using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EChamado.Server.Application.UseCases.StatusTypes.Commands;

public class CreateStatusTypeCommandHandler(
    IUnitOfWork unitOfWork,
    ILogger<CreateStatusTypeCommandHandler> logger) :
    IRequestHandler<CreateStatusTypeCommand, BaseResult<Guid>>
{
    public async Task<BaseResult<Guid>> Handle(CreateStatusTypeCommand request, CancellationToken cancellationToken)
    {
        var entity = StatusType.Create(request.Name, request.Description);

        if (!entity.IsValid())
        {
            logger.LogError("Validate StatusType has error");
            throw new ValidationException("Validate StatusType has error", entity.GetErrors());
        }

        await unitOfWork.BeginTransactionAsync();

        await unitOfWork.StatusTypes.AddAsync(entity);

        await unitOfWork.CommitAsync();

        logger.LogInformation("StatusType {StatusTypeId} created successfully", entity.Id);

        return new BaseResult<Guid>(entity.Id);
    }
}
