using EChamado.Server.Domain.Domains.Orders.Entities.Validations;
using EChamado.Server.Domain.Domains.Orders.Events.Categories;
using EChamado.Shared.Services;
using EChamado.Shared.Shared;

namespace EChamado.Server.Domain.Domains.Orders.Entities;

public class Category : Entity
{
    public string Name { get; private set; }
    public string Description { get; private set; }

    public IEnumerable<SubCategory> SubCategories { get; set; } = new List<SubCategory>(); // ef navigation property

    public Category(
        Guid id,
        string name,
        string description,
        DateTime createdAt,
        DateTime? updatedAt,
        DateTime? deletedAt,
        bool isDeleted) : base(id, createdAt, updatedAt, deletedAt, isDeleted)
    {
        Name = name;
        Description = description;
        Validate();
    }

    public override void Validate()
    {
        var validator = new CategoryValidation();
        var result = validator.Validate(this);
        if (!result.IsValid)
        {
            _errors = result.Errors.Select(x => x.ErrorMessage);
            _isValid = false;
        }
        else
        {
            _errors = Enumerable.Empty<string>();
            _isValid = true;
        }
    }

    public static Category Create(
        string name,
        string description,
        IDateTimeProvider dateTimeProvider)
    {
        var category =
            new Category(
                Guid.NewGuid(),
                name,
                description,
                dateTimeProvider.UtcNow,
                null, null, false);

        category.AddEvent(
            new CategoryCreated(category));

        return category;
    }

    public void Update(
        string name,
        string description,
        IDateTimeProvider dateTimeProvider)
    {
        Name = name;
        Description = description;

        Update(dateTimeProvider); // Chama base.Update() que define UpdatedAt

        AddEvent(
            new CategoryUpdated(this));
    }
}
