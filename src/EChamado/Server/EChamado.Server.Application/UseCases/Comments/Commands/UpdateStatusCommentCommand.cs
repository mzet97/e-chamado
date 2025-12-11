using EChamado.Server.Application.Common.Messaging;
using EChamado.Shared.Responses;

namespace EChamado.Server.Application.UseCases.Comments.Commands;

public class UpdateStatusCommentCommand : BrighterRequest<BaseResult>
{
    public IEnumerable<Item> Items { get; set; } = default!;

    public UpdateStatusCommentCommand()
    {
    }

    public UpdateStatusCommentCommand(IEnumerable<Item> items)
    {
        Items = items;
    }
}

public class Item
{
    public Guid Id { get; set; }
    public bool Active { get; set; }
}
