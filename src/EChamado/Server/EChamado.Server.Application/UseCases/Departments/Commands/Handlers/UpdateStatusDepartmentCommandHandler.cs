using EChamado.Server.Application.UseCases.Departments.Notifications;
using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EChamado.Server.Application.UseCases.Departments.Commands.Handlers;

public class UpdateStatusDepartmentCommandHandler(IUnitOfWork unitOfWork,
    IMediator mediator,
    ILogger<UpdateStatusDepartmentCommandHandler> logger) :
    IRequestHandler<UpdateStatusDepartmentCommand, BaseResult>
{
    public async Task<BaseResult> Handle(UpdateStatusDepartmentCommand request, CancellationToken cancellationToken)
{
    if (request == null)
    {
        logger.LogError("DeleteDepartmentCommand is null");
        throw new ArgumentNullException(nameof(request));
    }

    await unitOfWork.BeginTransactionAsync();

    foreach (var item in request.Items)
    {
        var entity = await unitOfWork
            .Departments
            .GetByIdAsync(item.Id);

        if (entity == null)
        {
            logger.LogError("Department not found");
            throw new NotFoundException("Department not found");
        }

        await unitOfWork.Departments
            .ActiveOrDisableAsync(item.Id, item.Active);

        await unitOfWork.CommitAsync();

        await mediator.Publish(
            new DisabledDepartmentNotification(
                entity.Id,
                entity.Name,
                entity.Description));
    }


    return new BaseResult(true, "Deletado com sucesso");
}
}
