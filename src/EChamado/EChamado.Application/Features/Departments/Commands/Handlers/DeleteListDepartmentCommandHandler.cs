using EChamado.Application.Features.Departments.Notifications;
using EChamado.Core.Exceptions;
using EChamado.Core.Repositories;
using EChamado.Core.Responses;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EChamado.Application.Features.Departments.Commands.Handlers;

public class DeleteListDepartmentCommandHandler(IUnitOfWork unitOfWork,
    IMediator mediator,
    ILogger<DeleteListDepartmentCommandHandler> logger) :
    IRequestHandler<DeleteListDepartmentCommand, BaseResult>
{
    public async Task<BaseResult> Handle(DeleteListDepartmentCommand request, CancellationToken cancellationToken)
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
