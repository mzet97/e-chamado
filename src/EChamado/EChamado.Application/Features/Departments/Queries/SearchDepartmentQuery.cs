using EChamado.Application.Features.Departments.ViewModels;
using EChamado.Application.ViewModels;
using EChamado.Core.Responses;
using MediatR;

namespace EChamado.Application.Features.Departments.Queries;

public class SearchDepartmentQuery : BaseSearch, IRequest<BaseResultList<DepartmentViewModel>>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
