using EChamado.Application.Features.Departments.Notifications;
using EChamado.Core.Domains.Orders.ValueObjects;
using EChamado.Core.Domains.Orders.ValueObjects.Validations;
using EChamado.Core.Exceptions;
using EChamado.Core.Repositories;
using EChamado.Core.Responses;
using EChamado.Core.Validations;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EChamado.Application.Features.Departments.Commands.Handlers;

public class UpdateDepartmentCommandHandler(IUnitOfWork unitOfWork,
    IMediator mediator,
    ILogger<UpdateDepartmentCommandHandler> logger) : 
    IRequestHandler<UpdateDepartmentCommand, BaseResult>
{
    public async Task<BaseResult> Handle(
        UpdateDepartmentCommand request, 
        CancellationToken cancellationToken)
    {
        var entity = new Department
        {
            Name = request.Name,
            Description = request.Description
        };

        if (!Validator.Validate(new DepartmentValidation(), entity))
        {
            logger.LogError("Validate Department has error");
            throw new ValidationException("Validate Department has error");
        }

        var entityDb = await unitOfWork
            .Departments
            .GetByIdAsync(request.Id);

        if(entityDb == null)
        {
            logger.LogError("Department not found");
            throw new NotFoundException("Department not found");
        }

        entityDb.Name = entity.Name;
        entityDb.Description = entity.Description;

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
