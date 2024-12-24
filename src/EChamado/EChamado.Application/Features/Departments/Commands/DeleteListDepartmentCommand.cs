using EChamado.Core.Responses;
using MediatR;

namespace EChamado.Application.Features.Departments.Commands;

public class DeleteListDepartmentCommand : IRequest<BaseResult>
{
    public IEnumerable<Guid> Ids { get; set; }
}
