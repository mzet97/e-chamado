using EChamado.Server.Application.UseCases.OrderTypes.ViewModels;
using EChamado.Shared.Responses;
using MediatR;

namespace EChamado.Server.Application.UseCases.OrderTypes.Queries;

public record GetOrderTypeByIdQuery(Guid OrderTypeId) : IRequest<BaseResult<OrderTypeViewModel>>;
