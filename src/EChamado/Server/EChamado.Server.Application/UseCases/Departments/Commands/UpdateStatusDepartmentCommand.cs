using EChamado.Server.Application.Common.Messaging;
using EChamado.Shared.Responses;

namespace EChamado.Server.Application.UseCases.Departments.Commands;

public class UpdateStatusDepartmentCommand : BrighterRequest<BaseResult>
{
    public IEnumerable<Item> Items { get; set; } = default!;

    public UpdateStatusDepartmentCommand()
    {
    }

    public UpdateStatusDepartmentCommand(IEnumerable<Item> items)
    {
        Items = items;
    }
}

public class Item
{
    public Guid Id { get; set; }
    public bool Active { get; set; }
}