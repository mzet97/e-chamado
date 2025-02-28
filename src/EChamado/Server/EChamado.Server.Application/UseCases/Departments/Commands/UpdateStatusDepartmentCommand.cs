using EChamado.Shared.Responses;
using MediatR;

namespace EChamado.Server.Application.UseCases.Departments.Commands;

public class UpdateStatusDepartmentCommand : IRequest<BaseResult>
{
    public IEnumerable<Item> Items { get; set; }
}

public class Item
{
    public Guid Id { get; set; }
    public bool Active { get; set; }
}