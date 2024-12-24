using EChamado.Core.Responses;
using MediatR;

namespace EChamado.Application.Features.Departments.Commands;

public class CreateDepartmentCommand : IRequest<BaseResult<Guid>>
{
    public required string Name { get; set; }
    public required string Description { get; set; }
}
