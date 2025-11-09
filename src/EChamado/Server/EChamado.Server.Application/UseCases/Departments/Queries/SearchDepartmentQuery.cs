using EChamado.Server.Application.Common.Messaging;
using EChamado.Server.Application.UseCases.Departments.ViewModels;
using EChamado.Shared.Responses;
using EChamado.Shared.ViewModels;

namespace EChamado.Server.Application.UseCases.Departments.Queries;

public class SearchDepartmentQuery : BaseSearch, BrighterRequest<BaseResultList<DepartmentViewModel>>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
