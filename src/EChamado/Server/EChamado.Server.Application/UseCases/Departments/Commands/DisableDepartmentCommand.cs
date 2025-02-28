using EChamado.Shared.Responses;
using MediatR;

namespace EChamado.Server.Application.UseCases.Departments.Commands;

public class DisableDepartmentCommand : IRequest<BaseResult>
{
    public DisableDepartmentCommand(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; set; }
}
