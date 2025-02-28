using EChamado.Server.Application.UseCases.Departments.ViewModels;
using EChamado.Shared.Responses;
using EChamado.Shared.ViewModels;
using MediatR;

namespace EChamado.Server.Application.UseCases.Departments.Queries;

public class SearchDepartmentQuery : BaseSearch, IRequest<BaseResultList<DepartmentViewModel>>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
