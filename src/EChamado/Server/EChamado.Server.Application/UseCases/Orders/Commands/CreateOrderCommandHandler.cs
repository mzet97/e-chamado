using EChamado.Server.Domain.Domains.Orders;
using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EChamado.Server.Application.UseCases.Orders.Commands;

public class CreateOrderCommandHandler(
    IUnitOfWork unitOfWork,
    IMediator mediator,
    ILogger<CreateOrderCommandHandler> logger) :
    IRequestHandler<CreateOrderCommand, BaseResult<Guid>>
{
    public async Task<BaseResult<Guid>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
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
            request.Title,
            request.Description,
            statusId.Value,
            request.TypeId,
            request.CategoryId,
            request.SubCategoryId,
            request.DepartmentId,
            request.RequestingUserId,
            request.RequestingUserEmail,
            request.DueDate
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

        return new BaseResult<Guid>(order.Id);
    }
}
