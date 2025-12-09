using EChamado.Server.Application.UseCases.Categories.ViewModels;
using EChamado.Server.Domain.Domains.Orders.Entities;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using LinqKit;
using Paramore.Brighter;
using System.Linq.Expressions;

namespace EChamado.Server.Application.UseCases.SubCategories.Queries.Handlers;

public class SearchSubCategoriesQueryHandler(IUnitOfWork unitOfWork) :
    RequestHandlerAsync<SearchSubCategoriesQuery>
{
    public override async Task<SearchSubCategoriesQuery> HandleAsync(
        SearchSubCategoriesQuery query,
        CancellationToken cancellationToken = default)
    {
        Expression<Func<SubCategory, bool>>? filter = PredicateBuilder.New<SubCategory>(true);
        Func<IQueryable<SubCategory>, IOrderedQueryable<SubCategory>>? orderBy = null;

        if (!string.IsNullOrWhiteSpace(query.Name))
        {
            filter = filter.And(x => x.Name == query.Name);
        }

        if (!string.IsNullOrWhiteSpace(query.Description))
        {
            filter = filter.And(x => x.Description == query.Description);
        }

        if (query.CategoryId.HasValue && query.CategoryId.Value != Guid.Empty)
        {
            filter = filter.And(x => x.CategoryId == query.CategoryId.Value);
        }

        if (query.CreatedAt != default)
        {
            filter = filter.And(x => x.CreatedAtUtc == query.CreatedAt);
        }

        if (query.UpdatedAt != default)
        {
            filter = filter.And(x => x.UpdatedAtUtc == query.UpdatedAt);
        }

        if (query.DeletedAt != new DateTime())
        {
            filter = filter.And(x => x.DeletedAtUtc == query.DeletedAt);
        }

        if (!string.IsNullOrWhiteSpace(query.Order))
        {
            switch (query.Order)
            {
                case "Id":
                    orderBy = x => x.OrderBy(n => n.Id);
                    break;

                case "Name":
                    orderBy = x => x.OrderBy(n => n.Name);
                    break;

                case "Description":
                    orderBy = x => x.OrderBy(n => n.Description);
                    break;

                case "CategoryId":
                    orderBy = x => x.OrderBy(n => n.CategoryId);
                    break;

                case "CreatedAt":
                    orderBy = x => x.OrderBy(n => n.CreatedAtUtc);
                    break;

                case "UpdatedAt":
                    orderBy = x => x.OrderBy(n => n.UpdatedAtUtc);
                    break;

                case "DeletedAt":
                    orderBy = x => x.OrderBy(n => n.DeletedAtUtc);
                    break;

                default:
                    orderBy = x => x.OrderBy(n => n.Id);
                    break;
            }
        }

        var result = await unitOfWork.SubCategories
            .SearchAsync(
                filter,
                orderBy,
                query.PageSize,
                query.PageIndex);

        var items = result.Data.Select(sc => new SubCategoryViewModel(
            sc.Id,
            sc.Name,
            sc.Description,
            sc.CategoryId,
            sc.CreatedAtUtc,
            sc.UpdatedAtUtc,
            sc.DeletedAtUtc,
            sc.IsDeleted
        )).ToList();

        query.Result = new BaseResultList<SubCategoryViewModel>(items, result.PagedResult);

        return await base.HandleAsync(query, cancellationToken);
    }
}
