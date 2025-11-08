using EChamado.Shared.Responses;
using MediatR;

namespace EChamado.Server.Application.UseCases.StatusTypes.Commands;

public record CreateStatusTypeCommand(
    string Name,
    string Description
) : IRequest<BaseResult<Guid>>;
