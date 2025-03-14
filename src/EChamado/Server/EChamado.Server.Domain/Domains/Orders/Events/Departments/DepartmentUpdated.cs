using EChamado.Server.Domain.Domains.Orders.Entities;
using EChamado.Shared.Shared;

namespace EChamado.Server.Domain.Domains.Orders.Events.Departments;

public class DepartmentUpdated : DomainEvent
{
    public Department Department { get; }

    public DepartmentUpdated(Department department)
    {
        Department = department;
    }
}
