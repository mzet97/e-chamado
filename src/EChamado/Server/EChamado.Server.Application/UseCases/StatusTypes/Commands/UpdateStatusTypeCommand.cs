using EChamado.Shared.Responses;
using MediatR;

namespace EChamado.Server.Application.UseCases.StatusTypes.Commands;

public record UpdateStatusTypeCommand(
    Guid Id,
    string Name,
    string Description
) : IRequest<BaseResult>;
