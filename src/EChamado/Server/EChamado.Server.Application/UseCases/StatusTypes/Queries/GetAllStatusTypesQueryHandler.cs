using EChamado.Server.Application.UseCases.StatusTypes.ViewModels;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EChamado.Server.Application.UseCases.StatusTypes.Queries;

public class GetAllStatusTypesQueryHandler(
    IUnitOfWork unitOfWork,
    ILogger<GetAllStatusTypesQueryHandler> logger) :
    IRequestHandler<GetAllStatusTypesQuery, BaseResultList<StatusTypeViewModel>>
{
    public async Task<BaseResultList<StatusTypeViewModel>> Handle(GetAllStatusTypesQuery request, CancellationToken cancellationToken)
    {
        var statusTypes = await unitOfWork.StatusTypes.GetAllAsync(cancellationToken);

        if (!string.IsNullOrEmpty(request.SearchText))
        {
            statusTypes = statusTypes.Where(st =>
                st.Name.Contains(request.SearchText, StringComparison.OrdinalIgnoreCase) ||
                st.Description.Contains(request.SearchText, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        var totalCount = statusTypes.Count;

        var items = statusTypes
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(st => new StatusTypeViewModel(
                st.Id,
                st.Name,
                st.Description
            ))
            .ToList();

        logger.LogInformation("Get all status types returned {Count} results", items.Count);

        return new BaseResultList<StatusTypeViewModel>(items, totalCount, request.PageNumber, request.PageSize);
    }
}
