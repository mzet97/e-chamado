using EChamado.Application.Features.Departments.ViewModels;
using EChamado.Core.Exceptions;
using EChamado.Core.Repositories;
using EChamado.Core.Responses;
using MediatR;

namespace EChamado.Application.Features.Departments.Queries.Handlers;

public class GetByIdDepartmentQueryHandler(IUnitOfWork unitOfWork) : 
    IRequestHandler<GetByIdDepartmentQuery, BaseResult<DepartmentViewModel>>
{
    public async Task<BaseResult<DepartmentViewModel>> Handle(
        GetByIdDepartmentQuery request, 
        CancellationToken cancellationToken)
    {
       if(request == null)
            throw new ArgumentNullException(nameof(request));

        var department = await unitOfWork.Departments.GetByIdAsync(request.Id);

        if(department == null)
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
