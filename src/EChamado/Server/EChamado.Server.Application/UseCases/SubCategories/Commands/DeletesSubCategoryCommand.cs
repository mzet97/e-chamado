using EChamado.Server.Application.Common.Messaging;
using EChamado.Shared.Responses;

namespace EChamado.Server.Application.UseCases.SubCategories.Commands;

public class DeletesSubCategoryCommand : BrighterRequest<BaseResult>
{
    public IEnumerable<Guid> Ids { get; set; } = default!;

    public DeletesSubCategoryCommand()
    {
    }

    public DeletesSubCategoryCommand(IEnumerable<Guid> ids)
    {
        Ids = ids;
    }
}
