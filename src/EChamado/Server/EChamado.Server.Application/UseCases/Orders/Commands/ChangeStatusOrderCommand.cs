using EChamado.Shared.Responses;
using MediatR;

namespace EChamado.Server.Application.UseCases.Orders.Commands;

public record ChangeStatusOrderCommand(
    Guid OrderId,
    Guid StatusId
) : IRequest<BaseResult>;
