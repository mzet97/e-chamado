using MediatR;
using System.Text.Json;

namespace EChamado.Server.Application.UseCases.StatusTypes.Notifications;

public class UpdatedStatusTypeNotification : INotification
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public UpdatedStatusTypeNotification(Guid id, string name, string description)
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
