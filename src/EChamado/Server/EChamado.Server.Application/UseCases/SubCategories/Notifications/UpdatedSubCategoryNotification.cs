using Paramore.Brighter;
using System.Text.Json;

namespace EChamado.Server.Application.UseCases.SubCategories.Notifications;

public class UpdatedSubCategoryNotification : IRequest
{
    public Id Id { get; set; }
    public Id CorrelationId { get; set; } = new Id(Guid.NewGuid().ToString());
    public Guid CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public UpdatedSubCategoryNotification()
    {
        Id = new Id(Guid.NewGuid().ToString());
    }

    public UpdatedSubCategoryNotification(Guid id, Guid categoryId, string name, string description)
    {
        Id = new Id(id.ToString());
        CategoryId = categoryId;
        Name = name;
        Description = description;
    }

    public override string? ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}
