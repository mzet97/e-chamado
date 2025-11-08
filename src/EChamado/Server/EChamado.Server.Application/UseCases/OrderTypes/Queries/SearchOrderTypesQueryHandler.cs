using EChamado.Server.Application.UseCases.OrderTypes.ViewModels;
using EChamado.Server.Domain.Domains.Orders.Entities;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using LinqKit;
using MediatR;
using System.Linq.Expressions;

namespace EChamado.Server.Application.UseCases.OrderTypes.Queries;

public class SearchOrderTypesQueryHandler(IUnitOfWork unitOfWork) :
    IRequestHandler<SearchOrderTypesQuery, BaseResultList<OrderTypeViewModel>>
{
    public async Task<BaseResultList<OrderTypeViewModel>> Handle(
        SearchOrderTypesQuery request,
        CancellationToken cancellationToken)
    {
        Expression<Func<OrderType, bool>>? filter = PredicateBuilder.New<OrderType>(true);
        Func<IQueryable<OrderType>, IOrderedQueryable<OrderType>>? orderBy = null;

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

        var result = await unitOfWork.OrderTypes
            .SearchAsync(
                filter,
                orderBy,
                request.PageSize,
                request.PageIndex);

        var items = result.Data.Select(ot => new OrderTypeViewModel(
            ot.Id,
            ot.Name,
            ot.Description
        )).ToList();

        return new BaseResultList<OrderTypeViewModel>(items, result.PagedResult);
    }
}
