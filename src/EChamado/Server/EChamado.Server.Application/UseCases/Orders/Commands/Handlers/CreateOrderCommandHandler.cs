using EChamado.Server.Application.Common.Behaviours;
using EChamado.Server.Domain.Domains.Orders;
using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using EChamado.Shared.Services;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace EChamado.Server.Application.UseCases.Orders.Commands.Handlers;

public class CreateOrderCommandHandler(
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider,
    ILogger<CreateOrderCommandHandler> logger) :
    RequestHandlerAsync<CreateOrderCommand>
{
    [RequestLogging(0, HandlerTiming.Before)]
    [RequestValidation(1, HandlerTiming.Before)]
    public override async Task<CreateOrderCommand> HandleAsync(CreateOrderCommand command, CancellationToken cancellationToken = default)
    {
        // Busca status padrão "Aberto" ou primeiro status disponível
        var defaultStatus = await unitOfWork.StatusTypes.SearchAsync(
            x => x.Name.ToLower() == "aberto" || x.Name.ToLower() == "open",
            null,
            10,
            1);

        var statusId = defaultStatus.Data.FirstOrDefault()?.Id;

        if (statusId == null || statusId == Guid.Empty)
        {
            logger.LogError("No default status found");
            throw new NotFoundException("No default status found. Please create a status first.");
        }

        // Busca o usuário responsável padrão ou usa o mesmo usuário solicitante
        var responsibleUserId = command.RequestingUserId;
        var responsibleUserEmail = command.RequestingUserEmail;

        // Valida CategoryId e DepartmentId obrigatórios (FKs do chamado)
        var categoryId = command.CategoryId ?? Guid.Empty;
        var departmentId = command.DepartmentId ?? Guid.Empty;

        if (categoryId == Guid.Empty)
        {
            logger.LogError("Category is required to create an order");
            throw new ValidationException("Category is required to create an order",
                new[] { "CategoryId é obrigatório." });
        }

        if (departmentId == Guid.Empty)
        {
            logger.LogError("Department is required to create an order");
            throw new ValidationException("Department is required to create an order",
                new[] { "DepartmentId é obrigatório." });
        }

        // Confirma que a categoria e o departamento informados existem
        if (!await unitOfWork.Categories.ExistsAsync(c => c.Id == categoryId))
        {
            logger.LogError("Category {CategoryId} not found", categoryId);
            throw new NotFoundException($"Category {categoryId} not found");
        }

        if (!await unitOfWork.Departments.ExistsAsync(d => d.Id == departmentId))
        {
            logger.LogError("Department {DepartmentId} not found", departmentId);
            throw new NotFoundException($"Department {departmentId} not found");
        }

        var order = Order.Create(
            command.Title,
            command.Description,
            command.RequestingUserEmail,
            responsibleUserEmail,
            command.RequestingUserId,
            responsibleUserId,
            categoryId,
            departmentId,
            command.TypeId,
            statusId.Value,
            command.SubCategoryId,
            command.DueDate,
            dateTimeProvider
        );

        if (!order.IsValid())
        {
            logger.LogError("Validate Order has error");
            throw new ValidationException("Validate Order has error", order.Errors);
        }

        await unitOfWork.BeginTransactionAsync();

        await unitOfWork.Orders.AddAsync(order);

        await unitOfWork.CommitAsync();

        logger.LogInformation("Order {OrderId} created successfully", order.Id);

        command.Result = new BaseResult<Guid>(order.Id);
        return await base.HandleAsync(command, cancellationToken);
    }
}
