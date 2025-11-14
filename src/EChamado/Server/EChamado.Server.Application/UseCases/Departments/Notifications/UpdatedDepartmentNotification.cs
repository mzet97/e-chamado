using Paramore.Brighter;
using System.Text.Json;

namespace EChamado.Server.Application.UseCases.Departments.Notifications;

public class UpdatedDepartmentNotification : IRequest
{
    public Id Id { get; set; }
    public Id CorrelationId { get; set; } = new Id(Guid.NewGuid().ToString());
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public UpdatedDepartmentNotification()
    {
        Id = new Id(Guid.NewGuid().ToString());
    }

    public UpdatedDepartmentNotification(Guid id, string name, string description)
    {
        Id = new Id(id.ToString());
        Name = name;
        Description = description;
    }

    public override string? ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}
