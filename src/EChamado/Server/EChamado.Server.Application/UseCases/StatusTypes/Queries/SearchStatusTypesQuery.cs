using EChamado.Server.Application.UseCases.StatusTypes.ViewModels;
using EChamado.Shared.Responses;
using EChamado.Shared.ViewModels;
using MediatR;

namespace EChamado.Server.Application.UseCases.StatusTypes.Queries;

public class SearchStatusTypesQuery : BaseSearch, IRequest<BaseResultList<StatusTypeViewModel>>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
