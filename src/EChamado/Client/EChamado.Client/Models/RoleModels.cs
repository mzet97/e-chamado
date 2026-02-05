namespace EChamado.Client.Models;

public record RoleResponse(
    Guid Id,
    string Name,
    string? Description,
    List<string> Permissions
);

public record CreateRoleRequest(string Name, string Description, List<string> Permissions);
public record UpdateRoleRequest(string Name, string Description, List<string> Permissions);