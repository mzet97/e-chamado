using EChamado.Server.Application.Common.Messaging;
using EChamado.Shared.Responses;

namespace EChamado.Server.Application.UseCases.Departments.Commands;

public class DisableDepartmentCommand : BrighterRequest<BaseResult>
{
    public Guid Id { get; set; } = default!;

    public DisableDepartmentCommand()
    {
    }

    public DisableDepartmentCommand(Guid id)
    {
        Id = id;
    }
}
