using Paramore.Brighter;
using System.Text.Json;

namespace EChamado.Server.Application.UseCases.Categories.Notifications;

public class CreatedCategoryNotification : IRequest
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public CreatedCategoryNotification()
    {
        Id = Guid.NewGuid();
    }

    public CreatedCategoryNotification(Guid id, string name, string description)
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
