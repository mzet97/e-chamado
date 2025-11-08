using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EChamado.Server.Application.UseCases.Orders.Commands;

public class UpdateOrderCommandHandler(
    IUnitOfWork unitOfWork,
    ILogger<UpdateOrderCommandHandler> logger) :
    IRequestHandler<UpdateOrderCommand, BaseResult>
{
    public async Task<BaseResult> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await unitOfWork.Orders.GetByIdAsync(request.Id, cancellationToken);

        if (order == null)
        {
            logger.LogError("Order {OrderId} not found", request.Id);
            throw new NotFoundException($"Order {request.Id} not found");
        }

        order.Update(
            request.Title,
            request.Description,
            request.CategoryId,
            request.SubCategoryId,
            request.DepartmentId,
            request.DueDate
        );

        if (!order.IsValid())
        {
            logger.LogError("Validate Order has error");
            throw new ValidationException("Validate Order has error", order.GetErrors());
        }

        await unitOfWork.BeginTransactionAsync();

        await unitOfWork.Orders.UpdateAsync(order, cancellationToken);

        await unitOfWork.CommitAsync();

        logger.LogInformation("Order {OrderId} updated successfully", request.Id);

        return new BaseResult();
    }
}
