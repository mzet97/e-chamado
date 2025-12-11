using EChamado.Server.Application.Common.Messaging;
using EChamado.Shared.Responses;

namespace EChamado.Server.Application.UseCases.SubCategories.Commands;

public class DisableSubCategoryCommand : BrighterRequest<BaseResult>
{
    public Guid Id { get; set; } = default!;

    public DisableSubCategoryCommand()
    {
    }

    public DisableSubCategoryCommand(Guid id)
    {
        Id = id;
    }
}
