using EChamado.Server.Domain.Domains.Orders.Entities.Validations;
using EChamado.Server.Domain.Domains.Orders.Events.SubCategories;
using EChamado.Shared.Domain;
using EChamado.Shared.Services;

namespace EChamado.Server.Domain.Domains.Orders.Entities;

public class SubCategory : SoftDeletableEntity<SubCategory>
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;

    public Guid CategoryId { get; private set; }
    public Category Category { get; set; } = null!;

    private SubCategory() : base(new SubCategoryValidation()) { }

    private SubCategory(
        Guid id,
        string name,
        string description,
        Guid categoryId,
        IDateTimeProvider dateTimeProvider)
        : base(new SubCategoryValidation())
    {
        Id = id;
        Name = name;
        Description = description;
        CategoryId = categoryId;

        MarkCreated(dateTimeProvider.UtcNow);
        Validate();
    }

    public static SubCategory Create(
        string name,
        string description,
        Guid categoryId,
        IDateTimeProvider dateTimeProvider)
    {
        var subCategory = new SubCategory(Guid.NewGuid(), name, description, categoryId, dateTimeProvider);
        subCategory.AddEvent(new SubCategoryCreated(
            subCategory.Id,
            subCategory.CategoryId,
            subCategory.Name,
            subCategory.Description
        ));
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

        MarkUpdated(dateTimeProvider.UtcNow);
        Validate();

        AddEvent(new SubCategoryUpdated(
            Id,
            CategoryId,
            Name,
            Description
        ));
    }

    public override void Validate()
    {
        var validator = new SubCategoryValidation();
        var result = validator.Validate(this);

        _errors = result.Errors.Select(x => x.ErrorMessage);
        _isValid = result.IsValid;
    }
}