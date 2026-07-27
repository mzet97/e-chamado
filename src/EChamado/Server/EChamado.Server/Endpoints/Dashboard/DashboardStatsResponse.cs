namespace EChamado.Server.Endpoints.Dashboard;

public record DashboardStatsResponse
{
    public int TotalTickets { get; init; }
    public int MyTickets { get; init; }
    public int AssignedToMe { get; init; }
    public int OverdueTickets { get; init; }
}
