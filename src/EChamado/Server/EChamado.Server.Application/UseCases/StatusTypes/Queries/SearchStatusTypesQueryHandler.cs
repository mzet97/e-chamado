using EChamado.Server.Application.UseCases.StatusTypes.ViewModels;
using EChamado.Server.Domain.Domains.Orders.Entities;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using LinqKit;
using Paramore.Brighter;
using System.Linq.Expressions;

namespace EChamado.Server.Application.UseCases.StatusTypes.Queries;

public class SearchStatusTypesQueryHandler(IUnitOfWork unitOfWork) :
    RequestHandlerAsync<SearchStatusTypesQuery>
{
    public override async Task<SearchStatusTypesQuery> HandleAsync(
        SearchStatusTypesQuery query,
        CancellationToken cancellationToken = default)
    {
        Expression<Func<StatusType, bool>>? filter = PredicateBuilder.New<StatusType>(true);
        Func<IQueryable<StatusType>, IOrderedQueryable<StatusType>>? orderBy = null;

        if (!string.IsNullOrWhiteSpace(query.Name))
        {
            filter = filter.And(x => x.Name == query.Name);
        }

        if (!string.IsNullOrWhiteSpace(query.Description))
        {
            filter = filter.And(x => x.Description == query.Description);
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

        var result = await unitOfWork.StatusTypes
            .SearchAsync(
                filter,
                orderBy,
                query.PageSize,
                query.PageIndex);

        var items = result.Data.Select(st => new StatusTypeViewModel(
            st.Id,
            st.Name,
            st.Description,
            st.CreatedAtUtc,
            st.UpdatedAtUtc,
            st.DeletedAtUtc,
            st.IsDeleted
        )).ToList();

        query.Result = new BaseResultList<StatusTypeViewModel>(items, result.PagedResult);

        return await base.HandleAsync(query, cancellationToken);
    }
}
