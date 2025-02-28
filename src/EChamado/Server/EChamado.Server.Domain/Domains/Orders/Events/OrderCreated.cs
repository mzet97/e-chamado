using EChamado.Shared.Shared;

namespace EChamado.Server.Domain.Domains.Orders.Events;

public class OrderCreated : IDomainEvent
{
    public Guid Id { get; set; }

    public string Description { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;

    public string Evaluation { get; set; } = string.Empty;
    public DateTime? OpeningDate { get; set; }
    public DateTime? ClosingDate { get; set; }
    public DateTime? DueDate { get; set; }

    public Guid StatusId { get; set; }
    public Guid TypeId { get; set; }
    public Guid RequestingUserId { get; set; }
    public string RequestingUserEmail { get; set; } = string.Empty;

    public Guid ResponsibleUserId { get; set; }
    public string ResponsibleUserEmail { get; set; } = string.Empty;

    public Guid CategoryId { get; set; }

    public Guid? SubCategoryId { get; set; }

    public Guid DepartmentId { get; set; }

    public OrderCreated()
    {
        
    }

    public OrderCreated(Order order)
    {
        Id = order.Id;
        Title = order.Title;
        Description = order.Description;
        Evaluation = order.Evaluation;
        OpeningDate = order.OpeningDate;
        ClosingDate = order.ClosingDate;
        DueDate = order.DueDate;
        StatusId = order.StatusId;
        TypeId = order.TypeId;
        RequestingUserId = order.RequestingUserId;
        RequestingUserEmail = order.RequestingUserEmail;
        ResponsibleUserId = order.ResponsibleUserId;
        ResponsibleUserEmail = order.ResponsibleUserEmail;
        CategoryId = order.CategoryId;
        SubCategoryId = order.SubCategoryId;
        DepartmentId = order.DepartmentId;
    }

    public OrderCreated(
        Guid id,
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
        Id = id;
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
    }
}
