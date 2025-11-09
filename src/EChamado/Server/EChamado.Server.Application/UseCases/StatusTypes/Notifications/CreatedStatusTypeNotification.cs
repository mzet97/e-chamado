using Paramore.Brighter;
using System.Text.Json;

namespace EChamado.Server.Application.UseCases.StatusTypes.Notifications;

public class CreatedStatusTypeNotification : IRequest
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public CreatedStatusTypeNotification()
    {
        Id = Guid.NewGuid();
    }

    public CreatedStatusTypeNotification(Guid id, string name, string description)
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
