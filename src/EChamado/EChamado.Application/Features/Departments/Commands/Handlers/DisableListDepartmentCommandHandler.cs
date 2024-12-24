using EChamado.Application.Features.Departments.Notifications;
using EChamado.Core.Exceptions;
using EChamado.Core.Repositories;
using EChamado.Core.Responses;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EChamado.Application.Features.Departments.Commands.Handlers;

public class DisableListDepartmentCommandHandler(IUnitOfWork unitOfWork,
    IMediator mediator,
    ILogger<DisableListDepartmentCommandHandler> logger) :
    IRequestHandler<DisableListDepartmentCommand, BaseResult>
{
    public async Task<BaseResult> Handle(DisableListDepartmentCommand request, CancellationToken cancellationToken)
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
                .DisableAsync(id);

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
