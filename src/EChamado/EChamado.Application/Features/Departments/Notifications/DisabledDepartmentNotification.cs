using MediatR;
using System.Text.Json;

namespace EChamado.Application.Features.Departments.Notifications;

public class DisabledDepartmentNotification : INotification
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public DisabledDepartmentNotification(Guid id, string name, string description)
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
