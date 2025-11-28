using Paramore.Brighter;
using System.Text.Json;

namespace EChamado.Server.Application.UseCases.Categories.Notifications;

public class CreatedSubCategoryNotification : IRequest
{
    public Id Id { get; set; }
    public Id CorrelationId { get; set; } = new Id(Guid.NewGuid().ToString());
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }

    public CreatedSubCategoryNotification()
    {
        Id = new Id(Guid.NewGuid().ToString());
    }

    public CreatedSubCategoryNotification(Guid id, string name, string description, Guid categoryId)
    {
        Id = new Id(id.ToString());
        Name = name;
        Description = description;
        CategoryId = categoryId;
    }

    public override string? ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}
