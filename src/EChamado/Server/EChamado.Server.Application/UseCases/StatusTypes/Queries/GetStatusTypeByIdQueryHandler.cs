using EChamado.Server.Application.UseCases.StatusTypes.ViewModels;
using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EChamado.Server.Application.UseCases.StatusTypes.Queries;

public class GetStatusTypeByIdQueryHandler(
    IUnitOfWork unitOfWork,
    ILogger<GetStatusTypeByIdQueryHandler> logger) :
    IRequestHandler<GetStatusTypeByIdQuery, BaseResult<StatusTypeViewModel>>
{
    public async Task<BaseResult<StatusTypeViewModel>> Handle(GetStatusTypeByIdQuery request, CancellationToken cancellationToken)
    {
        var statusType = await unitOfWork.StatusTypes.GetByIdAsync(request.StatusTypeId, cancellationToken);

        if (statusType == null)
        {
            logger.LogError("StatusType {StatusTypeId} not found", request.StatusTypeId);
            throw new NotFoundException($"StatusType {request.StatusTypeId} not found");
        }

        var viewModel = new StatusTypeViewModel(
            statusType.Id,
            statusType.Name,
            statusType.Description
        );

        logger.LogInformation("StatusType {StatusTypeId} retrieved successfully", request.StatusTypeId);

        return new BaseResult<StatusTypeViewModel>(viewModel);
    }
}
