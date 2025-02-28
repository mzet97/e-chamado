using EChamado.Server.Application.UseCases.Departments.ViewModels;
using EChamado.Shared.Responses;
using MediatR;

namespace EChamado.Server.Application.UseCases.Departments.Queries;

public class GetByIdDepartmentQuery : IRequest<BaseResult<DepartmentViewModel>>
{
    public Guid Id { get; set; }

    public GetByIdDepartmentQuery(Guid id)
    {
        Id = id;
    }
}
