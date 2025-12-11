using EChamado.Server.Application.Common.Messaging;
using EChamado.Shared.Responses;

namespace EChamado.Server.Application.UseCases.SubCategories.Commands;

public class UpdateSubCategoryCommand : BrighterRequest<BaseResult>
{
    public Guid Id { get; set; } = default!;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }

    public UpdateSubCategoryCommand()
    {
    }

    public UpdateSubCategoryCommand(Guid id, string name, string description, Guid categoryId)
    {
        Id = id;
        Name = name;
        Description = description;
        CategoryId = categoryId;
    }
}
