using EChamado.Server.Application.Common.Messaging;
using EChamado.Server.Application.UseCases.Departments.ViewModels;
using EChamado.Shared.Responses;

namespace EChamado.Server.Application.UseCases.Departments.Queries;

public class GetByIdDepartmentQuery : BrighterRequest<BaseResult<DepartmentViewModel>>
{
    public Guid Id { get; set; }

    public GetByIdDepartmentQuery()
    {
    }

    public GetByIdDepartmentQuery(Guid id)
    {
        Id = id;
    }
}
