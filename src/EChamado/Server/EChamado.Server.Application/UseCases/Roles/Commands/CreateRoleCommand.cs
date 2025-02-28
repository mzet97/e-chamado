using EChamado.Shared.Responses;
using MediatR;

namespace EChamado.Server.Application.UseCases.Roles.Commands;

public class CreateRoleCommand : IRequest<BaseResult<Guid>>
{
    public string Name { get; set; }
}
