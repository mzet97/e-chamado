using EChamado.Server.Application.Orders.Queries;
using EChamado.Server.Application.UseCases.StatusTypes.ViewModels;
using EChamado.Server.Domain.Repositories;
using Paramore.Darker;

namespace EChamado.Server.Application.Orders.QueryHandlers;

public sealed class ListStatusTypesQueryHandler : QueryHandlerAsync<ListStatusTypesQuery, IEnumerable<StatusTypeViewModel>>
{
    private readonly IUnitOfWork _unitOfWork;

    public ListStatusTypesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public override async Task<IEnumerable<StatusTypeViewModel>> ExecuteAsync(ListStatusTypesQuery query, CancellationToken cancellationToken = default)
    {
        var statusTypes = await _unitOfWork.StatusTypes.GetAllAsync();
        return statusTypes.Select(status => new StatusTypeViewModel(
            status.Id,
            status.Name,
            status.Description,
            status.CreatedAtUtc,
            status.UpdatedAtUtc,
            status.DeletedAtUtc,
            status.IsDeleted));
    }
}
