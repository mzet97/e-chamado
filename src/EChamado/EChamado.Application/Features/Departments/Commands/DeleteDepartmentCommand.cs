using EChamado.Core.Responses;
using MediatR;

namespace EChamado.Application.Features.Departments.Commands;

public class DeleteDepartmentCommand : IRequest<BaseResult>
{
    public DeleteDepartmentCommand(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; set; }
}
