using EChamado.Server.Application.UseCases.Departments.Notifications;
using EChamado.Server.Domain.Domains.Orders.ValueObjects;
using EChamado.Server.Domain.Domains.Orders.ValueObjects.Validations;
using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Server.Domain.Validations;
using EChamado.Shared.Responses;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EChamado.Server.Application.UseCases.Departments.Commands.Handlers;

public class CreateDepartmentCommandHandler(
    IUnitOfWork unitOfWork, 
    IMediator mediator,
    ILogger<CreateDepartmentCommandHandler> logger) : IRequestHandler<CreateDepartmentCommand, BaseResult<Guid>>
{
    public async Task<BaseResult<Guid>> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
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

        await unitOfWork.BeginTransactionAsync();

        await unitOfWork.Departments.AddAsync(entity);

        await unitOfWork.CommitAsync();

        await mediator.Publish(new CreatedDepartmentNotification(entity.Id, entity.Name, entity.Description));

        return new BaseResult<Guid>(entity.Id);
    }
}
