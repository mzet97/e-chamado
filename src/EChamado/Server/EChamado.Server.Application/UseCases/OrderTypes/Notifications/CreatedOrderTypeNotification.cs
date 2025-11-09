using MediatR;
using System.Text.Json;

namespace EChamado.Server.Application.UseCases.OrderTypes.Notifications;

public class CreatedOrderTypeNotification : INotification
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public CreatedOrderTypeNotification(Guid id, string name, string description)
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
