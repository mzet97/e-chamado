using EChamado.Server.Application.UseCases.Orders.ViewModels;
using EChamado.Shared.Responses;
using MediatR;

namespace EChamado.Server.Application.UseCases.Orders.Queries;

public record GetOrderByIdQuery(Guid OrderId) : IRequest<BaseResult<OrderViewModel>>;
