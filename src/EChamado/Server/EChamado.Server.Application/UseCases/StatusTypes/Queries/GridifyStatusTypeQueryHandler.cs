using EChamado.Server.Application.Common;
using EChamado.Server.Application.UseCases.StatusTypes.ViewModels;
using EChamado.Server.Domain.Repositories.Orders;
using EChamado.Shared.Responses;
using MediatR;

namespace EChamado.Server.Application.UseCases.StatusTypes.Queries.Handlers;

/// <summary>
/// Handler para processar queries de status types com Gridify
/// </summary>
public class GridifyStatusTypeQueryHandler : IRequestHandler<GridifyStatusTypeQuery, BaseResultList<StatusTypeViewModel>>
{
    private readonly IStatusTypeRepository _statusTypeRepository;

    public GridifyStatusTypeQueryHandler(IStatusTypeRepository statusTypeRepository)
    {
        _statusTypeRepository = statusTypeRepository;
    }

    public async Task<BaseResultList<StatusTypeViewModel>> Handle(GridifyStatusTypeQuery request, CancellationToken cancellationToken)
    {
        // 1. Obtém a query base
        var query = _statusTypeRepository.GetAllQueryable()
            .Where(st => !st.IsDeleted); // Filtro padrão: não retornar registros deletados

        // 2. Aplica Gridify (filtros, ordenação e paginação)
        var result = await query.ApplyGridifyAsync(request, cancellationToken);

        // 3. Mapeia as entidades para ViewModels
        var viewModels = result.Data.Select(statusType => new StatusTypeViewModel(
            statusType.Id,
            statusType.Name,
            statusType.Description,
            statusType.CreatedAtUtc,
            statusType.UpdatedAtUtc,
            statusType.DeletedAtUtc,
            statusType.IsDeleted
        )).ToList();

        // 4. Retorna resultado paginado com ViewModels
        return new BaseResultList<StatusTypeViewModel>(viewModels, result.PagedResult);
    }
}
