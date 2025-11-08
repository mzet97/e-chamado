using EChamado.Server.Application.UseCases.Orders.ViewModels;
using MediatR;

namespace EChamado.Server.Application.UseCases.Orders.Queries;

public record GetOrderByIdQuery(Guid OrderId) : IRequest<OrderViewModel?>;
