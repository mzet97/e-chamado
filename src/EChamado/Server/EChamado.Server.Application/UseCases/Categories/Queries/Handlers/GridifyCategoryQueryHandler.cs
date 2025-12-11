using EChamado.Server.Application.Common;
using EChamado.Server.Application.UseCases.Categories.ViewModels;
using EChamado.Server.Domain.Repositories.Orders;
using EChamado.Shared.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EChamado.Server.Application.UseCases.Categories.Queries.Handlers;

/// <summary>
/// Handler para processar queries de categories com Gridify
/// </summary>
public class GridifyCategoryQueryHandler : IRequestHandler<GridifyCategoryQuery, BaseResultList<CategoryViewModel>>
{
    private readonly ICategoryRepository _categoryRepository;

    public GridifyCategoryQueryHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<BaseResultList<CategoryViewModel>> Handle(GridifyCategoryQuery request, CancellationToken cancellationToken)
    {
        // 1. Obtém a query base com includes para eager loading
        var query = _categoryRepository.GetAllQueryable()
            .Include(c => c.SubCategories)
            .Where(c => !c.IsDeleted); // Filtro padrão: não retornar registros deletados

        // 2. Aplica Gridify (filtros, ordenação e paginação)
        var result = await query.ApplyGridifyAsync(request, cancellationToken);

        // 3. Mapeia as entidades para ViewModels
        var viewModels = result.Data.Select(category => new CategoryViewModel(
            category.Id,
            category.Name,
            category.Description,
            category.CreatedAtUtc,
            category.UpdatedAtUtc,
            category.DeletedAtUtc,
            category.IsDeleted,
            category.SubCategories.Select(sc => new SubCategoryViewModel(
                sc.Id,
                sc.Name,
                sc.Description,
                sc.CategoryId,
                sc.CreatedAtUtc,
                sc.UpdatedAtUtc,
                sc.DeletedAtUtc,
                sc.IsDeleted
            )).ToList()
        )).ToList();

        // 4. Retorna resultado paginado com ViewModels
        return new BaseResultList<CategoryViewModel>(viewModels, result.PagedResult);
    }
}
