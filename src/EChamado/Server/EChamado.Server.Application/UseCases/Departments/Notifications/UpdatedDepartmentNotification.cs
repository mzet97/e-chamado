using Paramore.Brighter;
using System.Text.Json;

namespace EChamado.Server.Application.UseCases.Departments.Notifications;

public class UpdatedDepartmentNotification : IRequest
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public UpdatedDepartmentNotification()
    {
        Id = Guid.NewGuid();
    }

    public UpdatedDepartmentNotification(Guid id, string name, string description)
    {
        Id = id;
        Name = name;
        Description = description;
    }

    public override string? ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}
