using EChamado.Server.Application.Common.Messaging;
using EChamado.Shared.Responses;

namespace EChamado.Server.Application.UseCases.Categories.Commands;

public class CreateCategoryCommand : BrighterRequest<BaseResult<Guid>>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public CreateCategoryCommand()
    {
    }

    public CreateCategoryCommand(string name, string description)
    {
        Name = name;
        Description = description;
    }
}
