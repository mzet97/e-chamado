using EChamado.Server.Application.Common;
using EChamado.Server.Application.UseCases.SubCategories.ViewModels;
using EChamado.Server.Domain.Repositories.Orders;
using EChamado.Shared.Responses;
using MediatR;

namespace EChamado.Server.Application.UseCases.SubCategories.Queries.Handlers;

/// <summary>
/// Handler para processar queries de sub-categories com Gridify
/// </summary>
public class GridifySubCategoryQueryHandler : IRequestHandler<GridifySubCategoryQuery, BaseResultList<SubCategoryViewModel>>
{
    private readonly ISubCategoryRepository _subCategoryRepository;

    public GridifySubCategoryQueryHandler(ISubCategoryRepository subCategoryRepository)
    {
        _subCategoryRepository = subCategoryRepository;
    }

    public async Task<BaseResultList<SubCategoryViewModel>> Handle(GridifySubCategoryQuery request, CancellationToken cancellationToken)
    {
        // 1. Obtém a query base
        var query = _subCategoryRepository.GetAllQueryable()
            .Where(sc => !sc.IsDeleted); // Filtro padrão: não retornar registros deletados

        // 2. Aplica Gridify (filtros, ordenação e paginação)
        var result = await query.ApplyGridifyAsync(request, cancellationToken);

        // 3. Mapeia as entidades para ViewModels
        var viewModels = result.Data.Select(subCategory => new SubCategoryViewModel(
            subCategory.Id,
            subCategory.Name,
            subCategory.Description,
            subCategory.CategoryId,
            subCategory.CreatedAtUtc,
            subCategory.UpdatedAtUtc,
            subCategory.DeletedAtUtc,
            subCategory.IsDeleted
        )).ToList();

        // 4. Retorna resultado paginado com ViewModels
        return new BaseResultList<SubCategoryViewModel>(viewModels, result.PagedResult);
    }
}
