using EChamado.Server.Application.UseCases.StatusTypes.ViewModels;
using EChamado.Server.Domain.Domains.Orders.Entities;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using LinqKit;
using MediatR;
using System.Linq.Expressions;

namespace EChamado.Server.Application.UseCases.StatusTypes.Queries;

public class SearchStatusTypesQueryHandler(IUnitOfWork unitOfWork) :
    IRequestHandler<SearchStatusTypesQuery, BaseResultList<StatusTypeViewModel>>
{
    public async Task<BaseResultList<StatusTypeViewModel>> Handle(
        SearchStatusTypesQuery request,
        CancellationToken cancellationToken)
    {
        Expression<Func<StatusType, bool>>? filter = PredicateBuilder.New<StatusType>(true);
        Func<IQueryable<StatusType>, IOrderedQueryable<StatusType>>? orderBy = null;

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

        var result = await unitOfWork.StatusTypes
            .SearchAsync(
                filter,
                orderBy,
                request.PageSize,
                request.PageIndex);

        var items = result.Data.Select(st => new StatusTypeViewModel(
            st.Id,
            st.Name,
            st.Description
        )).ToList();

        return new BaseResultList<StatusTypeViewModel>(items, result.PagedResult);
    }
}
