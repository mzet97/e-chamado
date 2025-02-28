using EChamado.Shared.Responses;
using MediatR;

namespace EChamado.Server.Application.UseCases.Departments.Commands;

public class UpdateDepartmentCommand : IRequest<BaseResult>
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
}
