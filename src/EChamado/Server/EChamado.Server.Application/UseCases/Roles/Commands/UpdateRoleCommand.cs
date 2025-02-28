using EChamado.Shared.Responses;
using MediatR;

namespace EChamado.Server.Application.UseCases.Roles.Commands;

public class UpdateRoleCommand : IRequest<BaseResult>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
