using EChamado.Server.Application.Common.Messaging;
using EChamado.Shared.Responses;

namespace EChamado.Server.Application.UseCases.StatusTypes.Commands;

public class DeletesStatusTypeCommand : BrighterRequest<BaseResult>
{
    public IEnumerable<Guid> Ids { get; set; } = default!;

    public DeletesStatusTypeCommand()
    {
    }

    public DeletesStatusTypeCommand(IEnumerable<Guid> ids)
    {
        Ids = ids;
    }
}
