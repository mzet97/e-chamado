using EChamado.Server.Application.Common.Messaging;
using EChamado.Shared.Responses;

namespace EChamado.Server.Application.UseCases.Categories.Commands;

public class DeleteCategoryCommand : BrighterRequest<BaseResult>
{
    public Guid CategoryId { get; set; } = default!;

    public DeleteCategoryCommand()
    {
    }

    public DeleteCategoryCommand(Guid categoryId)
    {
        CategoryId = categoryId;
    }
}
