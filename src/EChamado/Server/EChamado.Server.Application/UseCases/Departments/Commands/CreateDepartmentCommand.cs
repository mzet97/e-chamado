using EChamado.Shared.Responses;
using MediatR;

namespace EChamado.Server.Application.UseCases.Departments.Commands;

public class CreateDepartmentCommand : IRequest<BaseResult<Guid>>
{
    public required string Name { get; set; }
    public required string Description { get; set; }
}
