using EChamado.Server.Application.UseCases.Categories.ViewModels;
using Paramore.Darker;

namespace EChamado.Server.Application.Orders.Queries;

public sealed class ListCategoriesQuery : IQuery<IEnumerable<CategoryViewModel>>
{
}
