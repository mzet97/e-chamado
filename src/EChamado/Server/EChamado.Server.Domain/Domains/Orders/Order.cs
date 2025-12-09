using EChamado.Server.Domain.Domains.Orders.Entities;
using EChamado.Server.Domain.Domains.Orders.Events.Orders;
using EChamado.Server.Domain.Domains.Orders.Validations;
using EChamado.Shared.Domain;
using EChamado.Shared.Services;

namespace EChamado.Server.Domain.Domains.Orders;

public class Order : SoftDeletableAggregateRoot<Order>
{
    public string Description { get; private set; } = string.Empty;
    public string Title { get; private set; } = string.Empty;

    public string? Evaluation { get; private set; }

    public DateTime? OpeningDate { get; private set; }
    public DateTime? ClosingDate { get; private set; }
    public DateTime? DueDate { get; private set; }

    public Guid StatusId { get; private set; }
    public StatusType Status { get; set; } = null!;

    public Guid TypeId { get; private set; }
    public OrderType Type { get; set; } = null!;

    public Guid RequestingUserId { get; private set; }
    public string RequestingUserEmail { get; private set; } = string.Empty;

    public Guid ResponsibleUserId { get; private set; }
    public string ResponsibleUserEmail { get; private set; } = string.Empty;

    public Guid CategoryId { get; private set; }
    public Category Category { get; set; } = null!;

    public Guid? SubCategoryId { get; private set; }
    public SubCategory? SubCategory { get; set; }

    public Guid DepartmentId { get; private set; }
    public Department Department { get; set; } = null!;

    private Order() : base(new OrderValidation()) { }


    // ctor interno para testes/EF
    internal Order(
        Guid id,
        string title,
        string description,
        string requestingUserEmail,
        string responsibleUserEmail,
        Guid requestingUserId,
        Guid responsibleUserId,
        Guid categoryId,
        Guid departmentId,
        Guid orderTypeId,
        Guid statusTypeId,
        Guid? subCategoryId,
        DateTime? dueDate,
        DateTime? openingDateUtc,
        IDateTimeProvider dateTimeProvider)
        : base(new OrderValidation())
    {
        Id = id;

        Title = title;
        Description = description;

        RequestingUserId = requestingUserId;
        ResponsibleUserId = responsibleUserId;

        RequestingUserEmail = requestingUserEmail;
        ResponsibleUserEmail = responsibleUserEmail;

        CategoryId = categoryId;
        DepartmentId = departmentId;

        TypeId = orderTypeId;
        StatusId = statusTypeId;

        SubCategoryId = subCategoryId;
        DueDate = dueDate;

        var now = dateTimeProvider.UtcNow;
        OpeningDate = openingDateUtc ?? now;

        MarkCreated(now);
        Validate();
    }


    internal static Order CreateForTest(
        string title,
        string description,
        string requestingUserEmail,
        string responsibleUserEmail,
        Guid requestingUserId,
        Guid responsibleUserId,
        Guid categoryId,
        Guid departmentId,
        Guid orderTypeId,
        Guid statusTypeId,
        IDateTimeProvider dateTimeProvider,
        Guid? subCategoryId = null,
        DateTime? dueDate = null,
        DateTime? openingDate = null)
    {
        return new Order(
            Guid.NewGuid(),
            title,
            description,
            requestingUserEmail,
            responsibleUserEmail,
            requestingUserId,
            responsibleUserId,
            categoryId,
            departmentId,
            orderTypeId,
            statusTypeId,
            subCategoryId,
            dueDate,
            openingDate,
            dateTimeProvider);
    }

    public static Order Create(
        string title,
        string description,
        string requestingUserEmail,
        string responsibleUserEmail,
        Guid requestingUserId,
        Guid responsibleUserId,
        Guid categoryId,
        Guid departmentId,
        Guid orderTypeId,
        Guid statusTypeId,
        Guid? subCategoryId,
        DateTime? dueDate,
        IDateTimeProvider dateTimeProvider)
    {
        var order = new Order(
            Guid.NewGuid(),
            title,
            description,
            requestingUserEmail,
            responsibleUserEmail,
            requestingUserId,
            responsibleUserId,
            categoryId,
            departmentId,
            orderTypeId,
            statusTypeId,
            subCategoryId,
            dueDate,
            openingDateUtc: null,
            dateTimeProvider);

        order.AddEvent(new OrderCreated(
            order.Id,
            order.Title,
            order.Description,
            order.StatusId,
            order.TypeId,
            order.CategoryId,
            order.DepartmentId,
            order.SubCategoryId,
            order.RequestingUserId,
            order.RequestingUserEmail,
            order.ResponsibleUserId,
            order.ResponsibleUserEmail,
            order.OpeningDate!.Value,
            order.DueDate
        ));

        return order;
    }

    public void Update(
        string title,
        string description,
        string requestingUserEmail,
        Guid requestingUserId,
        Guid responsibleUserId,
        Guid categoryId,
        Guid departmentId,
        Guid orderTypeId,
        Guid statusTypeId,
        Guid? subCategoryId,
        DateTime? dueDate,
        IDateTimeProvider dateTimeProvider)
    {
        Title = title;
        Description = description;

        RequestingUserEmail = requestingUserEmail;
        RequestingUserId = requestingUserId;

        ResponsibleUserId = responsibleUserId;

        CategoryId = categoryId;
        DepartmentId = departmentId;

        TypeId = orderTypeId;
        StatusId = statusTypeId;

        SubCategoryId = subCategoryId;
        DueDate = dueDate;

        MarkUpdated(dateTimeProvider.UtcNow);
        Validate();

        AddEvent(new OrderUpdated(
            Id,
            Title,
            Description,
            StatusId,
            TypeId,
            CategoryId,
            DepartmentId,
            SubCategoryId,
            ResponsibleUserId,
            ResponsibleUserEmail,
            DueDate
        ));
    }

    public void AssignTo(Guid userId, string userEmail, IDateTimeProvider dateTimeProvider)
    {
        ResponsibleUserId = userId;
        ResponsibleUserEmail = userEmail;

        MarkUpdated(dateTimeProvider.UtcNow);
        Validate();

        AddEvent(new OrderUpdated(
            Id,
            Title,
            Description,
            StatusId,
            TypeId,
            CategoryId,
            DepartmentId,
            SubCategoryId,
            ResponsibleUserId,
            ResponsibleUserEmail,
            DueDate
        ));
    }

    public void ChangeStatus(Guid statusId, IDateTimeProvider dateTimeProvider)
    {
        StatusId = statusId;

        MarkUpdated(dateTimeProvider.UtcNow);
        Validate();

        AddEvent(new OrderUpdated(
            Id,
            Title,
            Description,
            StatusId,
            TypeId,
            CategoryId,
            DepartmentId,
            SubCategoryId,
            ResponsibleUserId,
            ResponsibleUserEmail,
            DueDate
        ));
    }

    public void Close(int evaluation, IDateTimeProvider dateTimeProvider)
    {
        Evaluation = evaluation.ToString();
        ClosingDate = dateTimeProvider.UtcNow;

        MarkUpdated(dateTimeProvider.UtcNow);
        Validate();

        AddEvent(new OrderClosed(
            Id,
            ClosingDate!.Value,
            Evaluation
        ));
    }

    public override void Validate()
    {
        var validator = new OrderValidation();
        var result = validator.Validate(this);

        _errors = result.Errors.Select(x => x.ErrorMessage);
        _isValid = result.IsValid;
    }
}
