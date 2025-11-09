using MediatR;
using System.Text.Json;

namespace EChamado.Server.Application.UseCases.Categories.Notifications;

public class CreatedSubCategoryNotification : INotification
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }

    public CreatedSubCategoryNotification(Guid id, string name, string description, Guid categoryId)
    {
        Id = id;
        Name = name;
        Description = description;
        CategoryId = categoryId;
    }

    public override string? ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}
