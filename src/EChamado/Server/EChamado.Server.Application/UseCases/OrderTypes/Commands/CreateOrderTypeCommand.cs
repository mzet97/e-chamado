using EChamado.Shared.Responses;
using MediatR;

namespace EChamado.Server.Application.UseCases.OrderTypes.Commands;

public record CreateOrderTypeCommand(
    string Name,
    string Description
) : IRequest<BaseResult<Guid>>;
