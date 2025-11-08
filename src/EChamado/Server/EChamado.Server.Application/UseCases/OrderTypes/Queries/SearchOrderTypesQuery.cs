using EChamado.Server.Application.UseCases.OrderTypes.ViewModels;
using EChamado.Shared.Responses;
using EChamado.Shared.ViewModels;
using MediatR;

namespace EChamado.Server.Application.UseCases.OrderTypes.Queries;

public class SearchOrderTypesQuery : BaseSearch, IRequest<BaseResultList<OrderTypeViewModel>>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
