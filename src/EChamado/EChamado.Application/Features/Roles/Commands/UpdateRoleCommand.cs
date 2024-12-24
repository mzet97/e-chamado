using EChamado.Core.Responses;
using MediatR;

namespace EChamado.Application.Features.Roles.Commands;

public class UpdateRoleCommand : IRequest<BaseResult>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
