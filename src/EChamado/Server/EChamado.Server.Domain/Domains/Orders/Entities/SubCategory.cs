using EChamado.Server.Domain.Domains.Orders.Entities.Validations;
using EChamado.Server.Domain.Domains.Orders.Events.SubCategories;
using EChamado.Shared.Services;
using EChamado.Shared.Shared;

namespace EChamado.Server.Domain.Domains.Orders.Entities;

public class SubCategory : Entity
{
    public string Name { get; private set; }
    public string Description { get; private set; }

    public Guid CategoryId { get; private set; }
    public Category Category { get; set; } = null!; // ef navigation property

    public SubCategory(
        Guid id,
        string name,
        string description,
        Guid categoryId,
        DateTime createdAt,
        DateTime? updatedAt,
        DateTime? deletedAt,
        bool isDeleted) : base(id, createdAt, updatedAt, deletedAt, isDeleted)
    {
        Name = name;
        Description = description;
        CategoryId = categoryId;
        Validate();
    }

    public override void Validate()
    {
        var validator = new SubCategoryValidation();
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

    public static SubCategory Create(
        string name,
        string description,
        Guid categoryId,
        IDateTimeProvider dateTimeProvider)
    {
        var subCategory =
            new SubCategory(
                Guid.NewGuid(),
                name,
                description,
                categoryId,
                dateTimeProvider.UtcNow,
                null, null, false);

        subCategory.AddEvent(
            new SubCategoryCreated(subCategory));

        return subCategory;
    }

    public void Update(
        string name,
        string description,
        Guid categoryId,
        IDateTimeProvider dateTimeProvider)
    {
        Name = name;
        Description = description;
        CategoryId = categoryId;

        Update(dateTimeProvider);
        Validate();


        AddEvent(
            new SubCategoryUpdated(this));
    }
}
