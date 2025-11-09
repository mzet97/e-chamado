using EChamado.Server.Application.UseCases.Categories.ViewModels;
using EChamado.Server.Domain.Domains.Orders.Entities;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using LinqKit;
using MediatR;
using System.Linq.Expressions;

namespace EChamado.Server.Application.UseCases.SubCategories.Queries;

public class SearchSubCategoriesQueryHandler(IUnitOfWork unitOfWork) :
    IRequestHandler<SearchSubCategoriesQuery, BaseResultList<SubCategoryViewModel>>
{
    public async Task<BaseResultList<SubCategoryViewModel>> Handle(
        SearchSubCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        Expression<Func<SubCategory, bool>>? filter = PredicateBuilder.New<SubCategory>(true);
        Func<IQueryable<SubCategory>, IOrderedQueryable<SubCategory>>? orderBy = null;

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            filter = filter.And(x => x.Name == request.Name);
        }

        if (!string.IsNullOrWhiteSpace(request.Description))
        {
            filter = filter.And(x => x.Description == request.Description);
        }

        if (request.CategoryId.HasValue && request.CategoryId.Value != Guid.Empty)
        {
            filter = filter.And(x => x.CategoryId == request.CategoryId.Value);
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

                case "CategoryId":
                    orderBy = x => x.OrderBy(n => n.CategoryId);
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

        var result = await unitOfWork.SubCategories
            .SearchAsync(
                filter,
                orderBy,
                request.PageSize,
                request.PageIndex);

        var items = result.Data.Select(sc => new SubCategoryViewModel(
            sc.Id,
            sc.Name,
            sc.Description,
            sc.CategoryId
        )).ToList();

        return new BaseResultList<SubCategoryViewModel>(items, result.PagedResult);
    }
}
