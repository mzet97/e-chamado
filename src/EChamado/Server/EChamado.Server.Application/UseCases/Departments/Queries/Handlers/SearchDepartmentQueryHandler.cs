using EChamado.Server.Application.UseCases.Departments.ViewModels;
using EChamado.Server.Domain.Domains.Orders.Entities;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using LinqKit;
using Paramore.Brighter;
using System.Linq.Expressions;

namespace EChamado.Server.Application.UseCases.Departments.Queries.Handlers;

public class SearchDepartmentQueryHandler(IUnitOfWork unitOfWork) :
    RequestHandlerAsync<SearchDepartmentQuery>
{
    public override async Task<SearchDepartmentQuery> HandleAsync(
        SearchDepartmentQuery query,
        CancellationToken cancellationToken = default)
    {
        Expression<Func<Department, bool>>? filter = PredicateBuilder.New<Department>(true);
        Func<IQueryable<Department>, IOrderedQueryable<Department>>? ordeBy = null;

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
                    ordeBy = x => x.OrderBy(n => n.Id);
                    break;

                case "Name":
                    ordeBy = x => x.OrderBy(n => n.Name);
                    break;

                case "Description":
                    ordeBy = x => x.OrderBy(n => n.Description);
                    break;

                case "CreatedAt":
                    ordeBy = x => x.OrderBy(n => n.CreatedAt);
                    break;

                case "UpdatedAt":
                    ordeBy = x => x.OrderBy(n => n.UpdatedAt);
                    break;

                case "DeletedAt":
                    ordeBy = x => x.OrderBy(n => n.DeletedAt);
                    break;

                default:
                    ordeBy = x => x.OrderBy(n => n.Id);
                    break;
            }
        }

        var result = await unitOfWork.Departments
          .SearchAsync(
              filter,
              ordeBy,
              query.PageSize,
              query.PageIndex);

        query.Result = new BaseResultList<DepartmentViewModel>(
            result.Data.Select(x => DepartmentViewModel.FromEntity(x)).ToList(), result.PagedResult);

        return await base.HandleAsync(query, cancellationToken);
    }
}
