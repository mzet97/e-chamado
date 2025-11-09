using EChamado.Server.Application.UseCases.Categories.ViewModels;
using EChamado.Server.Domain.Domains.Orders.Entities;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using LinqKit;
using MediatR;
using System.Linq.Expressions;

namespace EChamado.Server.Application.UseCases.Categories.Queries;

public class SearchCategoriesQueryHandler(IUnitOfWork unitOfWork) :
    IRequestHandler<SearchCategoriesQuery, BaseResultList<CategoryViewModel>>
{
    public async Task<BaseResultList<CategoryViewModel>> Handle(
        SearchCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        Expression<Func<Category, bool>>? filter = PredicateBuilder.New<Category>(true);
        Func<IQueryable<Category>, IOrderedQueryable<Category>>? orderBy = null;

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            filter = filter.And(x => x.Name == request.Name);
        }

        if (!string.IsNullOrWhiteSpace(request.Description))
        {
            filter = filter.And(x => x.Description == request.Description);
        }

        if (request.Id != Guid.Empty)
        {
            filter = filter.And(x => x.Id == request.Id);
        }

        if (request.CreatedAt != default)
        {
            filter = filter.And(x => x.CreatedAt == request.CreatedAt);
        }

        if (request.UpdatedAt != default)
        {
            filter = filter.And(x => x.UpdatedAt == request.UpdatedAt);
        }

        if (request.DeletedAt != new DateTime())
        {
            filter = filter.And(x => x.DeletedAt == request.DeletedAt);
        }

        if (!string.IsNullOrWhiteSpace(request.Order))
        {
            switch (request.Order)
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

                case "CreatedAt":
                    orderBy = x => x.OrderBy(n => n.CreatedAt);
                    break;

                case "UpdatedAt":
                    orderBy = x => x.OrderBy(n => n.UpdatedAt);
                    break;

                case "DeletedAt":
                    orderBy = x => x.OrderBy(n => n.DeletedAt);
                    break;

                default:
                    orderBy = x => x.OrderBy(n => n.Id);
                    break;
            }
        }

        var result = await unitOfWork.Categories
            .SearchAsync(
                filter,
                orderBy,
                request.PageSize,
                request.PageIndex);

        var items = result.Data.Select(c => new CategoryViewModel(
            c.Id,
            c.Name,
            c.Description,
            c.SubCategories.Select(sc => new SubCategoryViewModel(
                sc.Id,
                sc.Name,
                sc.Description,
                sc.CategoryId
            )).ToList()
        )).ToList();

        return new BaseResultList<CategoryViewModel>(items, result.PagedResult);
    }
}
