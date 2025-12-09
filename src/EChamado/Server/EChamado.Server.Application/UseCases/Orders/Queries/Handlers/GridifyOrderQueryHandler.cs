using EChamado.Server.Application.Common;
using EChamado.Server.Application.UseCases.Orders.ViewModels;
using EChamado.Server.Domain.Repositories.Orders;
using EChamado.Shared.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EChamado.Server.Application.UseCases.Orders.Queries.Handlers;

/// <summary>
/// Handler para processar queries de orders com Gridify
/// </summary>
public class GridifyOrderQueryHandler : IRequestHandler<GridifyOrderQuery, BaseResultList<OrderViewModel>>
{
    private readonly IOrderRepository _orderRepository;

    public GridifyOrderQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<BaseResultList<OrderViewModel>> Handle(GridifyOrderQuery request, CancellationToken cancellationToken)
    {
        // 1. Obtém a query base com includes para eager loading
        var query = _orderRepository.GetAllQueryable()
            .Include(o => o.Status)
            .Include(o => o.Type)
            .Include(o => o.Category)
            .Include(o => o.SubCategory)
            .Include(o => o.Department)
            .Where(o => !o.IsDeleted); // Filtro padrão: não retornar registros deletados

        // 2. Aplica Gridify (filtros, ordenação e paginação)
        var result = await query.ApplyGridifyAsync(request, cancellationToken);

        // 3. Mapeia as entidades para ViewModels
        var viewModels = result.Data.Select(order => new OrderViewModel(
            order.Id,
            order.Title,
            order.Description,
            string.IsNullOrEmpty(order.Evaluation) ? null : int.Parse(order.Evaluation),
            order.OpeningDate ?? DateTime.UtcNow,
            order.ClosingDate,
            order.DueDate,
            order.StatusId,
            order.Status?.Name ?? string.Empty,
            order.TypeId,
            order.Type?.Name ?? string.Empty,
            order.CategoryId,
            order.Category?.Name,
            order.SubCategoryId,
            order.SubCategory?.Name,
            order.DepartmentId,
            order.Department?.Name,
            order.RequestingUserId,
            order.RequestingUserEmail,
            order.ResponsibleUserId,
            order.ResponsibleUserEmail,
            order.CreatedAtUtc,
            order.UpdatedAtUtc,
            order.DeletedAtUtc,
            order.IsDeleted
        )).ToList();

        // 4. Retorna resultado paginado com ViewModels
        return new BaseResultList<OrderViewModel>(viewModels, result.PagedResult);
    }
}
