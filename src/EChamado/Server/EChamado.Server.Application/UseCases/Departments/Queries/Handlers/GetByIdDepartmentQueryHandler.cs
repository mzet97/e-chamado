using EChamado.Server.Application.UseCases.Departments.ViewModels;
using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using Paramore.Brighter;

namespace EChamado.Server.Application.UseCases.Departments.Queries.Handlers;

public class GetByIdDepartmentQueryHandler(IUnitOfWork unitOfWork) :
    RequestHandlerAsync<GetByIdDepartmentQuery>
{
    public override async Task<GetByIdDepartmentQuery> HandleAsync(
        GetByIdDepartmentQuery query,
        CancellationToken cancellationToken = default)
    {
        if (query == null)
            throw new ArgumentNullException(nameof(query));

        var department = await unitOfWork.Departments.GetByIdAsync(query.Id);

        if (department == null)
            throw new NotFoundException("Not found");

        var viewModel = new DepartmentViewModel(
            department.Id,
            department.CreatedAtUtc,
            department.UpdatedAtUtc,
            department.DeletedAtUtc,
            department.IsDeleted,
            department.Name,
            department.Description);

        query.Result = new BaseResult<DepartmentViewModel>(viewModel, true, "Obitido com sucesso");

        return await base.HandleAsync(query, cancellationToken);
    }
}
