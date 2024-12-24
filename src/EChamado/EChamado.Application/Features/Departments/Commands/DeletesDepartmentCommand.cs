using EChamado.Core.Responses;
using MediatR;

namespace EChamado.Application.Features.Departments.Commands;

public class DeletesDepartmentCommand : IRequest<BaseResult>
{
    public DeletesDepartmentCommand(IEnumerable<Guid> ids)
    {
        Ids = ids;
    }

    public IEnumerable<Guid> Ids { get; set; }
}
