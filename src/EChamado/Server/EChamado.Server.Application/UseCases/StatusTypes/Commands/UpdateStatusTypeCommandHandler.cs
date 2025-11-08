using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EChamado.Server.Application.UseCases.StatusTypes.Commands;

public class UpdateStatusTypeCommandHandler(
    IUnitOfWork unitOfWork,
    ILogger<UpdateStatusTypeCommandHandler> logger) :
    IRequestHandler<UpdateStatusTypeCommand, BaseResult>
{
    public async Task<BaseResult> Handle(UpdateStatusTypeCommand request, CancellationToken cancellationToken)
    {
        var statusType = await unitOfWork.StatusTypes.GetByIdAsync(request.Id, cancellationToken);

        if (statusType == null)
        {
            logger.LogError("StatusType {StatusTypeId} not found", request.Id);
            throw new NotFoundException($"StatusType {request.Id} not found");
        }

        statusType.Update(request.Name, request.Description);

        if (!statusType.IsValid())
        {
            logger.LogError("Validate StatusType has error");
            throw new ValidationException("Validate StatusType has error", statusType.GetErrors());
        }

        await unitOfWork.BeginTransactionAsync();

        await unitOfWork.StatusTypes.UpdateAsync(statusType, cancellationToken);

        await unitOfWork.CommitAsync();

        logger.LogInformation("StatusType {StatusTypeId} updated successfully", request.Id);

        return new BaseResult();
    }
}
