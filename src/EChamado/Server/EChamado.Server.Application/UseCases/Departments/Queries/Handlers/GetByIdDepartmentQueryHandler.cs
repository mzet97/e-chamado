using EChamado.Server.Application.UseCases.Departments.ViewModels;
using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using MediatR;

namespace EChamado.Server.Application.UseCases.Departments.Queries.Handlers;

public class GetByIdDepartmentQueryHandler(IUnitOfWork unitOfWork) : 
    IRequestHandler<GetByIdDepartmentQuery, BaseResult<DepartmentViewModel>>
{
    public async Task<BaseResult<DepartmentViewModel>> Handle(
        GetByIdDepartmentQuery request,
        CancellationToken cancellationToken)
{
    if (request == null)
        throw new ArgumentNullException(nameof(request));

    var department = await unitOfWork.Departments.GetByIdAsync(request.Id);

    if (department == null)
        throw new NotFoundException("Not found");

    var viewModel = new DepartmentViewModel(
        department.Id,
        department.CreatedAt,
        department.UpdatedAt,
        department.DeletedAt,
        department.Name,
        department.Description);

    var result = new BaseResult<DepartmentViewModel>(viewModel, true, "Obitido com sucesso");

    return result;
}
}
