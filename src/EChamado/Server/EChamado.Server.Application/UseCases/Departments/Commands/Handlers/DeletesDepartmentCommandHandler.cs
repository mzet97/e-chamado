using EChamado.Server.Application.UseCases.Departments.Notifications;
using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EChamado.Server.Application.UseCases.Departments.Commands.Handlers;

public class DeletesDepartmentCommandHandler(IUnitOfWork unitOfWork,
    IMediator mediator,
    ILogger<DeletesDepartmentCommandHandler> logger) :
    IRequestHandler<DeletesDepartmentCommand, BaseResult>
{
    public async Task<BaseResult> Handle(DeletesDepartmentCommand request, CancellationToken cancellationToken)
{
    if (request == null)
    {
        logger.LogError("DeleteDepartmentCommand is null");
        throw new ArgumentNullException(nameof(request));
    }

    await unitOfWork.BeginTransactionAsync();

    foreach (var id in request.Ids)
    {
        var entity = await unitOfWork
            .Departments
            .GetByIdAsync(id);

        if (entity == null)
        {
            logger.LogError("Department not found");
            throw new NotFoundException("Department not found");
        }

        await unitOfWork.Departments
            .RemoveAsync(id);

        await unitOfWork.CommitAsync();

        await mediator.Publish(
            new DeletedDepartmentNotification(
                entity.Id,
                entity.Name,
                entity.Description));
    }


    return new BaseResult(true, "Deletado com sucesso");
}
}
