namespace EChamado.Client.Models;

public record DepartmentResponse(Guid Id, string Name, string Description);
public record CreateDepartmentRequest(string Name, string Description);
public record UpdateDepartmentRequest(string Name, string Description);
