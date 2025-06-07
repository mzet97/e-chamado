using EChamado.Server.Application.UseCases.Departments.ViewModels;
using EChamado.Server.Domain.Domains.Orders.Entities;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using LinqKit;
using MediatR;
using System.Linq.Expressions;

namespace EChamado.Server.Application.UseCases.Departments.Queries.Handlers;

public class SearchDepartmentQueryHandler(IUnitOfWork unitOfWork) : 
    IRequestHandler<SearchDepartmentQuery, BaseResultList<DepartmentViewModel>>
{
    public async Task<BaseResultList<DepartmentViewModel>> Handle(
        SearchDepartmentQuery request,
        CancellationToken cancellationToken)
{
    Expression<Func<Department, bool>>? filter = PredicateBuilder.New<Department>(true);
    Func<IQueryable<Department>, IOrderedQueryable<Department>>? ordeBy = null;

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
          request.PageSize,
          request.PageIndex);

    return new BaseResultList<DepartmentViewModel>(
        result.Data.Select(x => DepartmentViewModel.FromEntity(x)).ToList(), result.PagedResult);
}
}
