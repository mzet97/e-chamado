using EChamado.Server.Application.Common.Messaging;
using EChamado.Shared.Responses;

namespace EChamado.Server.Application.UseCases.Departments.Commands;

public class DeletesDepartmentCommand : BrighterRequest<BaseResult>
{
    public IEnumerable<Guid> Ids { get; set; } = default!;

    public DeletesDepartmentCommand()
    {
    }

    public DeletesDepartmentCommand(IEnumerable<Guid> ids)
    {
        Ids = ids;
    }
}
