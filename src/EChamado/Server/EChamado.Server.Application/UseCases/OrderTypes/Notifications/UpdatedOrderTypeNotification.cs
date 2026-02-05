using Paramore.Brighter;
using System.Text.Json;

namespace EChamado.Server.Application.UseCases.OrderTypes.Notifications;

public class UpdatedOrderTypeNotification : IRequest
{
    public Id Id { get; set; }
    public Id CorrelationId { get; set; } = new Id(Guid.NewGuid().ToString());
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public UpdatedOrderTypeNotification()
    {
        Id = new Id(Guid.NewGuid().ToString());
    }

    public UpdatedOrderTypeNotification(Guid id, string name, string description)
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
