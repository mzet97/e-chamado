using EChamado.Server.Application.Common;
using EChamado.Server.Application.UseCases.Departments.ViewModels;
using EChamado.Server.Domain.Repositories.Orders;
using EChamado.Shared.Responses;
using MediatR;

namespace EChamado.Server.Application.UseCases.Departments.Queries;

/// <summary>
/// Handler para processar queries de departments com Gridify
/// </summary>
public class GridifyDepartmentQueryHandler : IRequestHandler<GridifyDepartmentQuery, BaseResultList<DepartmentViewModel>>
{
    private readonly IDepartmentRepository _departmentRepository;

    public GridifyDepartmentQueryHandler(IDepartmentRepository departmentRepository)
    {
        _departmentRepository = departmentRepository;
    }

    public async Task<BaseResultList<DepartmentViewModel>> Handle(GridifyDepartmentQuery request, CancellationToken cancellationToken)
    {
        // 1. Obtém a query base
        var query = _departmentRepository.GetAllQueryable()
            .Where(d => !d.IsDeleted); // Filtro padrão: não retornar registros deletados

        // 2. Aplica Gridify (filtros, ordenação e paginação)
        var result = await query.ApplyGridifyAsync(request, cancellationToken);

        // 3. Mapeia as entidades para ViewModels
        var viewModels = result.Data.Select(department => new DepartmentViewModel(
            department.Id,
            department.CreatedAtUtc,
            department.UpdatedAtUtc,
            department.DeletedAtUtc,
            department.IsDeleted,
            department.Name,
            department.Description
        )).ToList();

        // 4. Retorna resultado paginado com ViewModels
        return new BaseResultList<DepartmentViewModel>(viewModels, result.PagedResult);
    }
}
