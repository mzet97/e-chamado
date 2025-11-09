using EChamado.Server.Application.Common.Messaging;
using EChamado.Server.Application.UseCases.OrderTypes.ViewModels;
using EChamado.Shared.Responses;
using EChamado.Shared.ViewModels;

namespace EChamado.Server.Application.UseCases.OrderTypes.Queries;

public class SearchOrderTypesQuery : BaseSearch, BrighterRequest<BaseResultList<OrderTypeViewModel>>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
