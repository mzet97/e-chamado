using EChamado.Server.Application.UseCases.Departments.Notifications;
using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EChamado.Server.Application.UseCases.Departments.Commands.Handlers;

public class UpdateDepartmentCommandHandler(IUnitOfWork unitOfWork,
    IMediator mediator,
    ILogger<UpdateDepartmentCommandHandler> logger) : 
    IRequestHandler<UpdateDepartmentCommand, BaseResult>
{
    public async Task<BaseResult> Handle(
        UpdateDepartmentCommand request, 
        CancellationToken cancellationToken)
    {
        var entityDb = await unitOfWork
            .Departments
            .GetByIdAsync(request.Id);

        if(entityDb == null)
        {
            logger.LogError("Department not found");
            throw new NotFoundException("Department not found");
        }

        entityDb.Update(request.Name, request.Description);

        if (!entityDb.IsValid())
        {
            logger.LogError("Validate Department has error");
            throw new ValidationException(
                "Validate Department has error", 
                entityDb.GetErrors());
        }

        await unitOfWork
            .Departments
            .UpdateAsync(entityDb);

        await unitOfWork.BeginTransactionAsync();

        await unitOfWork.CommitAsync();

        await mediator.Publish(
            new UpdatedDepartmentNotification(
            entityDb.Id,
            entityDb.Name,
            entityDb.Description));

        return new BaseResult(true, "Atualizado com sucesso");
    }
}
