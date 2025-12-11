using EChamado.Server.Application.Common.Messaging;
using EChamado.Shared.Responses;

namespace EChamado.Server.Application.UseCases.Departments.Commands;

public class CreateDepartmentCommand : BrighterRequest<BaseResult<Guid>>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public CreateDepartmentCommand()
    {
    }

    public CreateDepartmentCommand(string name, string description)
    {
        Name = name;
        Description = description;
    }
}
