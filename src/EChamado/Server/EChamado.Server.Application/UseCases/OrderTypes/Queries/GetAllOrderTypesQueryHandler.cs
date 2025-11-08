using EChamado.Server.Application.UseCases.OrderTypes.ViewModels;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EChamado.Server.Application.UseCases.OrderTypes.Queries;

public class GetAllOrderTypesQueryHandler(
    IUnitOfWork unitOfWork,
    ILogger<GetAllOrderTypesQueryHandler> logger) :
    IRequestHandler<GetAllOrderTypesQuery, BaseResultList<OrderTypeViewModel>>
{
    public async Task<BaseResultList<OrderTypeViewModel>> Handle(GetAllOrderTypesQuery request, CancellationToken cancellationToken)
    {
        var orderTypes = await unitOfWork.OrderTypes.GetAllAsync(cancellationToken);

        if (!string.IsNullOrEmpty(request.SearchText))
        {
            orderTypes = orderTypes.Where(ot =>
                ot.Name.Contains(request.SearchText, StringComparison.OrdinalIgnoreCase) ||
                ot.Description.Contains(request.SearchText, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        var totalCount = orderTypes.Count;

        var items = orderTypes
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(ot => new OrderTypeViewModel(
                ot.Id,
                ot.Name,
                ot.Description
            ))
            .ToList();

        logger.LogInformation("Get all order types returned {Count} results", items.Count);

        return new BaseResultList<OrderTypeViewModel>(items, totalCount, request.PageNumber, request.PageSize);
    }
}
