using Paramore.Brighter;
using System.Text.Json;

namespace EChamado.Server.Application.UseCases.Categories.Notifications;

public class DeletedSubCategoryNotification : IRequest
{
    public Id Id { get; set; }
    public Id CorrelationId { get; set; } = new Id(Guid.NewGuid().ToString());
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public DeletedSubCategoryNotification()
    {
        Id = new Id(Guid.NewGuid().ToString());
    }

    public DeletedSubCategoryNotification(Guid id, string name, string description)
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
