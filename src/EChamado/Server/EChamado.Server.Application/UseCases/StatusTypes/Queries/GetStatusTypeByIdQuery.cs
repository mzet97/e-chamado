using EChamado.Server.Application.UseCases.StatusTypes.ViewModels;
using EChamado.Shared.Responses;
using MediatR;

namespace EChamado.Server.Application.UseCases.StatusTypes.Queries;

public record GetStatusTypeByIdQuery(Guid StatusTypeId) : IRequest<BaseResult<StatusTypeViewModel>>;
