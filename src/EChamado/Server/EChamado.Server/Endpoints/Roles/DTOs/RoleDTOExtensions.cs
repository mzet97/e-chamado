using EChamado.Server.Application.UseCases.Roles.Commands;

namespace EChamado.Server.Endpoints.Roles.DTOs;

public static class RoleDTOExtensions
{
    public static CreateRoleCommand ToCommand(this CreateRoleRequest request)
    {
        return new CreateRoleCommand(request.Name);
    }

    public static UpdateRoleCommand ToCommand(this UpdateRoleRequest request)
    {
        return new UpdateRoleCommand(request.Id, request.Name);
    }

    public static DeleteRoleCommand ToCommand(this DeleteRoleRequest request)
    {
        return new DeleteRoleCommand(request.Id);
    }
}
