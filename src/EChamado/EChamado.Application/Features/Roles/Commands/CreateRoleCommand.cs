using EChamado.Core.Responses;
using MediatR;

namespace EChamado.Application.Features.Roles.Commands;

public class CreateRoleCommand : IRequest<BaseResult<Guid>>
{
    public string Name { get; set; }
}
