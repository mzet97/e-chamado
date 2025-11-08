using EChamado.Shared.Responses;
using MediatR;

namespace EChamado.Server.Application.UseCases.OrderTypes.Commands;

public record UpdateOrderTypeCommand(
    Guid Id,
    string Name,
    string Description
) : IRequest<BaseResult>;
