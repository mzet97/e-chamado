using Paramore.Brighter;

namespace EChamado.Server.Application.Orders.Commands;

public sealed class CreateOrderCommand : Command
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid TypeId { get; set; }
    public Guid CategoryId { get; set; }
    public Guid DepartmentId { get; set; }
    public Guid? SubCategoryId { get; set; }
    public DateTime? DueDate { get; set; }
    public Guid RequestingUserId { get; set; }
    public string RequestingUserEmail { get; set; } = string.Empty;
    public Guid ResponsibleUserId { get; set; }
    public string ResponsibleUserEmail { get; set; } = string.Empty;

    public CreateOrderCommand() : base(Guid.NewGuid())
    {
    }
}
