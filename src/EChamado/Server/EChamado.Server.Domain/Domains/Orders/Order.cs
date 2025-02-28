using EChamado.Server.Domain.Domains.Orders.Events;
using EChamado.Server.Domain.Domains.Orders.ValueObjects;
using EChamado.Shared.Shared;

namespace EChamado.Server.Domain.Domains.Orders;

public class Order : AggregateRoot
{
    public string Description { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;

    public string Evaluation { get; set; } = string.Empty;

    public DateTime? OpeningDate { get; set; }
    public DateTime? ClosingDate { get; set; }
    public DateTime? DueDate { get; set; }

    public Guid StatusId { get; set; }
    public StatusType Status { get; set; } = null!;

    public Guid TypeId { get; set; }
    public OrderType Type { get; set; } = null!;

    public Guid RequestingUserId { get; set; }
    public string RequestingUserEmail { get; set; } = string.Empty;

    public Guid ResponsibleUserId { get; set; }
    public string ResponsibleUserEmail { get; set; } = string.Empty;

    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public Guid? SubCategoryId { get; set; }
    public SubCategory? SubCategory { get; set; }

    public Guid DepartmentId { get; set; }
    public Department Department { get; set; } = null!;

    public Order()
    {

    }

    public Order(
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
        Id = Guid.NewGuid();
        Title = title;
        Description = description;
        RequestingUserEmail = requestingUserEmail;
        RequestingUserId = requestingUserId;
        ResponsibleUserId = responsibleUserId;
        CategoryId = categoryId;
        DepartmentId = departmentId;
        Type = new OrderType { Id = orderTypeId };
        Status = new StatusType { Id = statusTypeId };
        SubCategoryId = subCategoryId;
        DueDate = dueDate;
    }

    public static Order Create(
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
        var order = new Order(
            title,
            description,
            requestingUserEmail,
            requestingUserId,
            responsibleUserId,
            categoryId,
            departmentId,
            orderTypeId,
            statusTypeId,
            subCategoryId,
            dueDate);

        order.CreatedAt = DateTime.UtcNow;

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

        UpdatedAt = DateTime.UtcNow;

        AddEvent(
            new OrderUpdated(this));
    }
}
