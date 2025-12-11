using EChamado.Server.Application.Common.Messaging;
using EChamado.Shared.Responses;

namespace EChamado.Server.Application.UseCases.SubCategories.Commands;

public class CreateSubCategoryCommand : BrighterRequest<BaseResult<Guid>>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }

    public CreateSubCategoryCommand()
    {
    }

    public CreateSubCategoryCommand(string name, string description, Guid categoryId)
    {
        Name = name;
        Description = description;
        CategoryId = categoryId;
    }
}
