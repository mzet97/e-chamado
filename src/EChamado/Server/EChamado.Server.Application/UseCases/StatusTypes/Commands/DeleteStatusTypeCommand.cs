using EChamado.Server.Application.Common.Messaging;
using EChamado.Shared.Responses;

namespace EChamado.Server.Application.UseCases.StatusTypes.Commands;

public class DeleteStatusTypeCommand : BrighterRequest<BaseResult>
{
    public Guid StatusTypeId { get; set; } = default!;

    public DeleteStatusTypeCommand()
    {
    }

    public DeleteStatusTypeCommand(Guid statusTypeId)
    {
        StatusTypeId = statusTypeId;
    }
}
