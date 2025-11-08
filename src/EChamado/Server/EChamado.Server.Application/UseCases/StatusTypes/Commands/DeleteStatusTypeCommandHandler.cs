using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using MediatR;
using Microsoft.Extensions.Logging;

using EChamado.Server.Application.UseCases.StatusTypes.Notifications;

namespace EChamado.Server.Application.UseCases.StatusTypes.Commands;

public class DeleteStatusTypeCommandHandler(
    IUnitOfWork unitOfWork,
    IMediator mediator,
    ILogger<DeleteStatusTypeCommandHandler> logger) :
    IRequestHandler<DeleteStatusTypeCommand, BaseResult>
{
    public async Task<BaseResult> Handle(DeleteStatusTypeCommand request, CancellationToken cancellationToken)
    {
        var statusType = await unitOfWork.StatusTypes.GetByIdAsync(request.StatusTypeId, cancellationToken);

        if (statusType == null)
        {
            logger.LogError("StatusType {StatusTypeId} not found", request.StatusTypeId);
            throw new NotFoundException($"StatusType {request.StatusTypeId} not found");
        }

        await unitOfWork.BeginTransactionAsync();

        await unitOfWork.StatusTypes.DeleteAsync(statusType, cancellationToken);

        await unitOfWork.CommitAsync();

        await mediator.Publish(new DeletedStatusTypeNotification(statusType.Id, statusType.Name, statusType.Description));

        logger.LogInformation("StatusType {StatusTypeId} deleted successfully", request.StatusTypeId);

        return new BaseResult();
    }
}
