using EChamado.Server.Application.Common.Messaging;
using EChamado.Shared.Responses;

namespace EChamado.Server.Application.UseCases.Categories.Commands;

public class DeleteSubCategoryCommand : BrighterRequest<BaseResult>
{
    public Guid SubCategoryId { get; set; } = default!;

    public DeleteSubCategoryCommand()
    {
    }

    public DeleteSubCategoryCommand(Guid subCategoryId)
    {
        SubCategoryId = subCategoryId;
    }
}
