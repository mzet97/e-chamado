using EChamado.Shared.Responses;
using MediatR;

namespace EChamado.Server.Application.UseCases.Departments.Commands;

public class DeletesDepartmentCommand : IRequest<BaseResult>
{
    public DeletesDepartmentCommand(IEnumerable<Guid> ids)
    {
        Ids = ids;
    }

    public IEnumerable<Guid> Ids { get; set; }
}
