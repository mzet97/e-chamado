using EChamado.Server.Application.Common.Messaging;
using EChamado.Shared.Responses;

namespace EChamado.Server.Application.UseCases.StatusTypes.Commands;

public class UpdateStatusStatusTypeCommand : BrighterRequest<BaseResult>
{
    public IEnumerable<Item> Items { get; set; } = default!;

    public UpdateStatusStatusTypeCommand()
    {
    }

    public UpdateStatusStatusTypeCommand(IEnumerable<Item> items)
    {
        Items = items;
    }
}

public class Item
{
    public Guid Id { get; set; }
    public bool Active { get; set; }
}
