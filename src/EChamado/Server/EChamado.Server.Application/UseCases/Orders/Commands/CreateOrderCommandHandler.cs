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
            cancellationToken);

        var statusId = defaultStatus.FirstOrDefault()?.Id;

        if (statusId == null)
        {
            logger.LogError("No default status found");
            throw new NotFoundException("No default status found. Please create a status first.");
        }

        var order = Order.Create(
            command.Title,
            command.Description,
            statusId.Value,
            command.TypeId,
            command.CategoryId,
            command.SubCategoryId,
            command.DepartmentId,
            command.RequestingUserId,
            command.RequestingUserEmail,
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
