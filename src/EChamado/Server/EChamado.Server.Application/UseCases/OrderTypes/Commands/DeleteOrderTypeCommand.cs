using EChamado.Shared.Responses;
using MediatR;

namespace EChamado.Server.Application.UseCases.OrderTypes.Commands;

public record DeleteOrderTypeCommand(Guid OrderTypeId) : IRequest<BaseResult>;
