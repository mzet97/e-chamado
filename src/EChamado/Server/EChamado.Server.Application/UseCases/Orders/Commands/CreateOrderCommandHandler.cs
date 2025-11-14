using EChamado.Server.Application.Common.Behaviours;
using EChamado.Server.Domain.Domains.Orders;
using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace EChamado.Server.Application.UseCases.Orders.Commands;

public class CreateOrderCommandHandler(
    IUnitOfWork unitOfWork,
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

        // Se CategoryId não foi fornecido, busca uma categoria padrão ou cria uma
        var categoryId = command.CategoryId ?? Guid.Empty;
        var departmentId = command.DepartmentId ?? Guid.Empty;

        // Valida se categoria e departamento existem
        if (categoryId == Guid.Empty)
        {
            logger.LogWarning("No category provided, using default");
            // Aqui você pode buscar ou criar uma categoria padrão
        }

        if (departmentId == Guid.Empty)
        {
            logger.LogWarning("No department provided, using default");
            // Aqui você pode buscar ou criar um departamento padrão
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
            command.DueDate
        );

        if (!order.IsValid())
        {
            logger.LogError("Validate Order has error");
            throw new ValidationException("Validate Order has error", order.GetErrors());
        }

        await unitOfWork.BeginTransactionAsync();

        await unitOfWork.Orders.AddAsync(order);

        await unitOfWork.CommitAsync();

        logger.LogInformation("Order {OrderId} created successfully", order.Id);

        command.Result = new BaseResult<Guid>(order.Id);
        return await base.HandleAsync(command, cancellationToken);
    }
}
