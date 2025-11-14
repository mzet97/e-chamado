using EChamado.Server.Application.UseCases.StatusTypes.ViewModels;
using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using Paramore.Brighter;
using Microsoft.Extensions.Logging;

namespace EChamado.Server.Application.UseCases.StatusTypes.Queries;

public class GetStatusTypeByIdQueryHandler(
    IUnitOfWork unitOfWork,
    ILogger<GetStatusTypeByIdQueryHandler> logger) :
    RequestHandlerAsync<GetStatusTypeByIdQuery>
{
    public override async Task<GetStatusTypeByIdQuery> HandleAsync(GetStatusTypeByIdQuery query, CancellationToken cancellationToken = default)
    {
        var statusType = await unitOfWork.StatusTypes.GetByIdAsync(query.StatusTypeId);

        if (statusType == null)
        {
            logger.LogError("StatusType {StatusTypeId} not found", query.StatusTypeId);
            throw new NotFoundException($"StatusType {query.StatusTypeId} not found");
        }

        var viewModel = new StatusTypeViewModel(
            statusType.Id,
            statusType.Name,
            statusType.Description
        );

        logger.LogInformation("StatusType {StatusTypeId} retrieved successfully", query.StatusTypeId);

        query.Result = new BaseResult<StatusTypeViewModel>(viewModel);

        return await base.HandleAsync(query, cancellationToken);
    }
}
