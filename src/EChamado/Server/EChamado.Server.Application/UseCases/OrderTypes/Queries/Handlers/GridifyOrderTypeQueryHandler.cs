using EChamado.Server.Application.Common;
using EChamado.Server.Application.UseCases.OrderTypes.ViewModels;
using EChamado.Server.Domain.Repositories.Orders;
using EChamado.Shared.Responses;
using MediatR;

namespace EChamado.Server.Application.UseCases.OrderTypes.Queries.Handlers;

/// <summary>
/// Handler para processar queries de order types com Gridify
/// </summary>
public class GridifyOrderTypeQueryHandler : IRequestHandler<GridifyOrderTypeQuery, BaseResultList<OrderTypeViewModel>>
{
    private readonly IOrderTypeRepository _orderTypeRepository;

    public GridifyOrderTypeQueryHandler(IOrderTypeRepository orderTypeRepository)
    {
        _orderTypeRepository = orderTypeRepository;
    }

    public async Task<BaseResultList<OrderTypeViewModel>> Handle(GridifyOrderTypeQuery request, CancellationToken cancellationToken)
    {
        // 1. Obtém a query base
        var query = _orderTypeRepository.GetAllQueryable()
            .Where(ot => !ot.IsDeleted); // Filtro padrão: não retornar registros deletados

        // 2. Aplica Gridify (filtros, ordenação e paginação)
        var result = await query.ApplyGridifyAsync(request, cancellationToken);

        // 3. Mapeia as entidades para ViewModels
        var viewModels = result.Data.Select(orderType => new OrderTypeViewModel(
            orderType.Id,
            orderType.Name,
            orderType.Description,
            orderType.CreatedAtUtc,
            orderType.UpdatedAtUtc,
            orderType.DeletedAtUtc,
            orderType.IsDeleted
        )).ToList();

        // 4. Retorna resultado paginado com ViewModels
        return new BaseResultList<OrderTypeViewModel>(viewModels, result.PagedResult);
    }
}
