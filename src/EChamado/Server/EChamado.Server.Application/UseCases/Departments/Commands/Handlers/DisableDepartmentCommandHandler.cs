using EChamado.Server.Application.UseCases.Departments.Notifications;
using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EChamado.Server.Application.UseCases.Departments.Commands.Handlers;

public class DisableDepartmentCommandHandler(IUnitOfWork unitOfWork,
    IMediator mediator,
    ILogger<DisableDepartmentCommandHandler> logger) :
    IRequestHandler<DisableDepartmentCommand, BaseResult>
{
    public async Task<BaseResult> Handle(DisableDepartmentCommand request, CancellationToken cancellationToken)
{
    if (request == null)
    {
        logger.LogError("DisableDepartmentCommand is null");
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
        .DisableAsync(request.Id);

    await unitOfWork.CommitAsync();

    await mediator.Publish(
        new DeletedDepartmentNotification(
            entity.Id,
            entity.Name,
            entity.Description));

    return new BaseResult(true, "Desativado com sucesso");
}
}
