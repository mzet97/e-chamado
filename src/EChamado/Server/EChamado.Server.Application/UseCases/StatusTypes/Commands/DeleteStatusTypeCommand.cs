using EChamado.Shared.Responses;
using MediatR;

namespace EChamado.Server.Application.UseCases.StatusTypes.Commands;

public record DeleteStatusTypeCommand(Guid StatusTypeId) : IRequest<BaseResult>;
