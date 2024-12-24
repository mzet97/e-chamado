using EChamado.Application.Features.Departments.ViewModels;
using EChamado.Core.Responses;
using MediatR;

namespace EChamado.Application.Features.Departments.Queries;

public class GetByIdDepartmentQuery : IRequest<BaseResult<DepartmentViewModel>>
{
    public Guid Id { get; set; }

    public GetByIdDepartmentQuery(Guid id)
    {
        Id = id;
    }
}
