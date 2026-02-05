using EChamado.Server.Application.Common.Messaging;
using EChamado.Shared.Responses;

namespace EChamado.Server.Application.UseCases.StatusTypes.Commands;

public class DisableStatusTypeCommand : BrighterRequest<BaseResult>
{
    public Guid Id { get; set; } = default!;

    public DisableStatusTypeCommand()
    {
    }

    public DisableStatusTypeCommand(Guid id)
    {
        Id = id;
    }
}
