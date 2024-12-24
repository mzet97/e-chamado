using EChamado.Core.Responses;
using MediatR;

namespace EChamado.Application.Features.Departments.Commands;

public class DisableDepartmentCommand : IRequest<BaseResult>
{
    public DisableDepartmentCommand(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; set; }
}
