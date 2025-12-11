using EChamado.Server.Application.UseCases.StatusTypes.ViewModels;
using Paramore.Darker;

namespace EChamado.Server.Application.Orders.Queries;

public sealed class ListStatusTypesQuery : IQuery<IEnumerable<StatusTypeViewModel>>
{
}
