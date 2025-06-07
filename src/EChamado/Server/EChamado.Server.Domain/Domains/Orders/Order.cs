using EChamado.Server.Domain.Domains.Orders.Entities;
using EChamado.Server.Domain.Domains.Orders.Events.Orders;
using EChamado.Server.Domain.Domains.Orders.Validations;
using EChamado.Shared.Shared;

namespace EChamado.Server.Domain.Domains.Orders;

public class Order : AggregateRoot
{
    public string Description { get; private set; }
    public string Title { get; private set; }

    public string Evaluation { get; private set; }

    public DateTime? OpeningDate { get; private set; }
    public DateTime? ClosingDate { get; private set; }
    public DateTime? DueDate { get; private set; }

    public Guid StatusId { get; private set; }
    public StatusType Status { get; set; } = null!; // ef navigation property

    public Guid TypeId { get; private set; }
    public OrderType Type { get; set; } = null!; // ef navigation property

    public Guid RequestingUserId { get; private set; }
    public string RequestingUserEmail { get; private set; }

    public Guid ResponsibleUserId { get; private set; }
    public string ResponsibleUserEmail { get; private set; }

    public Guid CategoryId { get; private set; }
    public Category Category { get; set; } = null!; // ef navigation property

    public Guid? SubCategoryId { get; private set; }
    public SubCategory? SubCategory { get; set; } // ef navigation property

    public Guid DepartmentId { get; private set; }
    public Department Department { get; set; } = null!; // ef navigation property

    public Order()
    {

    }

    public Order(
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
        DateTime createdAt,
        DateTime? updatedAt,
        DateTime? deletedAt
        ) : base(id, createdAt, updatedAt, deletedAt, false)
    {
        Title = title;
        Description = description;
        RequestingUserId = requestingUserId;
        ResponsibleUserId = responsibleUserId;
        RequestingUserEmail = requestingUserEmail;
        ResponsibleUserEmail = requestingUserEmail;
        CategoryId = categoryId;
        DepartmentId = departmentId;
        TypeId = orderTypeId;
        SubCategoryId = subCategoryId;
        StatusId = statusTypeId;
        DueDate = dueDate;
        Validate();
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
        DateTime? dueDate)
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
            DateTime.Now,
            null,
            null);

        order.AddEvent(
            new OrderCreated(order));

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
        DateTime? dueDate)
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

        Update();

        AddEvent(
            new OrderUpdated(this));
    }

    public void Close()
    {
        ClosingDate = DateTime.Now;
        Update();
        AddEvent(
            new OrderClosed(this));
    }

    public override void Validate()
    {
        var validator = new OrderValidation();
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
}
