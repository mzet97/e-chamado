using EChamado.Core.Responses;
using MediatR;

namespace EChamado.Application.Features.Departments.Commands;

public class UpdateStatusDepartmentCommand : IRequest<BaseResult>
{
    public IEnumerable<Item> Items { get; set; }
}

public class Item
{
    public Guid Id { get; set; }
    public bool Active { get; set; }
}