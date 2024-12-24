using EChamado.Application.Features.Departments.Notifications;
using EChamado.Core.Exceptions;
using EChamado.Core.Repositories;
using EChamado.Core.Responses;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EChamado.Application.Features.Departments.Commands.Handlers;

public class DeleteDepartmentCommandHandler(IUnitOfWork unitOfWork,
    IMediator mediator,
    ILogger<DeleteDepartmentCommandHandler> logger) :
    IRequestHandler<DeleteDepartmentCommand, BaseResult>
{
    public async Task<BaseResult> Handle(
        DeleteDepartmentCommand request, 
        CancellationToken cancellationToken)
    {
        if (request == null)
        {
            logger.LogError("DeleteDepartmentCommand is null");
            throw new ArgumentNullException(nameof(request));
        }

        var entity = await unitOfWork
            .Departments
            .GetByIdAsync(request.Id);

        if (entity == null)
        {
            logger.LogError("Department not found");
            throw new NotFoundException("Department not found");
        }

        await unitOfWork.BeginTransactionAsync();

        await unitOfWork.Departments
            .RemoveAsync(request.Id);

        await unitOfWork.CommitAsync();

        await mediator.Publish(
            new DeletedDepartmentNotification(
                entity.Id,
                entity.Name,
                entity.Description));

        return new BaseResult(true, "Deletado com sucesso");
    }
}
