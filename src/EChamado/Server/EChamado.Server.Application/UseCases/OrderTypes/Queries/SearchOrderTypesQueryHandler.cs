using EChamado.Server.Application.UseCases.OrderTypes.ViewModels;
using EChamado.Server.Domain.Domains.Orders.Entities;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using LinqKit;
using Paramore.Brighter;
using System.Linq.Expressions;

namespace EChamado.Server.Application.UseCases.OrderTypes.Queries;

public class SearchOrderTypesQueryHandler(IUnitOfWork unitOfWork) :
    RequestHandlerAsync<SearchOrderTypesQuery>
{
    public override async Task<SearchOrderTypesQuery> HandleAsync(
        SearchOrderTypesQuery query,
        CancellationToken cancellationToken = default)
    {
        Expression<Func<OrderType, bool>>? filter = PredicateBuilder.New<OrderType>(true);
        Func<IQueryable<OrderType>, IOrderedQueryable<OrderType>>? orderBy = null;

        if (!string.IsNullOrWhiteSpace(query.Name))
        {
            filter = filter.And(x => x.Name == query.Name);
        }

        if (!string.IsNullOrWhiteSpace(query.Description))
        {
            filter = filter.And(x => x.Description == query.Description);
        }

        if (query.Id != Guid.Empty)
        {
            filter = filter.And(x => x.Id == query.Id);
        }

        if (query.CreatedAt != default)
        {
            filter = filter.And(x => x.CreatedAt == query.CreatedAt);
        }

        if (query.UpdatedAt != default)
        {
            filter = filter.And(x => x.UpdatedAt == query.UpdatedAt);
        }

        if (query.DeletedAt != new DateTime())
        {
            filter = filter.And(x => x.DeletedAt == query.DeletedAt);
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

        var result = await unitOfWork.OrderTypes
            .SearchAsync(
                filter,
                orderBy,
                query.PageSize,
                query.PageIndex);

        var items = result.Data.Select(ot => new OrderTypeViewModel(
            ot.Id,
            ot.Name,
            ot.Description
        )).ToList();

        query.Result = new BaseResultList<OrderTypeViewModel>(items, result.PagedResult);

        return await base.HandleAsync(query, cancellationToken);
    }
}
