using EChamado.Server.Domain.Domains.Orders.Entities.Validations;
using EChamado.Server.Domain.Domains.Orders.Events.Categories;
using EChamado.Shared.Domain;
using EChamado.Shared.Services;

namespace EChamado.Server.Domain.Domains.Orders.Entities;

public class Category : SoftDeletableEntity<Category>
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;

    public IEnumerable<SubCategory> SubCategories { get; set; } = new List<SubCategory>();

    private Category() : base(new CategoryValidation()) { }

    private Category(Guid id, string name, string description, IDateTimeProvider dateTimeProvider)
        : base(new CategoryValidation())
    {
        Id = id;
        Name = name;
        Description = description;

        MarkCreated(dateTimeProvider.UtcNow);
        Validate();
    }

    public static Category Create(
        string name,
        string description,
        IDateTimeProvider dateTimeProvider)
    {
        var category = new Category(Guid.NewGuid(), name, description, dateTimeProvider);
        category.AddEvent(new CategoryCreated(category.Id, category.Name, category.Description));
        return category;
    }

    public void Update(string name, string description, IDateTimeProvider dateTimeProvider)
    {
        Name = name;
        Description = description;

        MarkUpdated(dateTimeProvider.UtcNow);
        Validate();

        AddEvent(new CategoryUpdated(Id, Name, Description));
    }

    public override void Validate()
    {
        var validator = new CategoryValidation();
        var result = validator.Validate(this);

        _errors = result.Errors.Select(x => x.ErrorMessage);
        _isValid = result.IsValid;
    }
}
