using EChamado.Server.Domain.Domains.Orders.Entities;
using EChamado.Shared.Shared;

namespace EChamado.Server.Domain.Domains.Orders.Events.Departments;


public class DepartmentCreated : DomainEvent
{
    public Department Department { get; }

    public DepartmentCreated(Department department)
    {
        Department = department;
    }
}
