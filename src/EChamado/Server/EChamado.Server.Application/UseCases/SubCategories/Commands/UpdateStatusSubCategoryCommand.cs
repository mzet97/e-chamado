using EChamado.Server.Application.Common.Messaging;
using EChamado.Shared.Responses;

namespace EChamado.Server.Application.UseCases.SubCategories.Commands;

public class UpdateStatusSubCategoryCommand : BrighterRequest<BaseResult>
{
    public IEnumerable<Item> Items { get; set; } = default!;

    public UpdateStatusSubCategoryCommand()
    {
    }

    public UpdateStatusSubCategoryCommand(IEnumerable<Item> items)
    {
        Items = items;
    }
}

public class Item
{
    public Guid Id { get; set; }
    public bool Active { get; set; }
}
