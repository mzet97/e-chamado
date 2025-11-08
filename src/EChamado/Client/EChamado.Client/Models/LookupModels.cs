namespace EChamado.Client.Models;

public record OrderTypeResponse(Guid Id, string Name, string Description);
public record StatusTypeResponse(Guid Id, string Name, string Description);

public record CreateOrderTypeRequest(string Name, string Description);
public record UpdateOrderTypeRequest(string Name, string Description);

public record CreateStatusTypeRequest(string Name, string Description);
public record UpdateStatusTypeRequest(string Name, string Description);
