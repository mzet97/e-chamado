using EChamado.Server.Common.Api;
using EChamado.Server.Domain.Repositories.Orders;
using EChamado.Shared.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EChamado.Server.Endpoints.Dashboard;

/// <summary>
/// Endpoint de supervisão de equipe.
/// Retorna métricas por responsável: carga de trabalho, tempo médio, SLA.
/// </summary>
public sealed class GetTeamStatsEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/team/stats", HandleAsync)
            .WithName("Estatísticas da equipe")
            .WithTags("Dashboard", "Team")
            .Produces<BaseResult<TeamStatsResponse>>();

    private static async Task<IResult> HandleAsync(
        [FromServices] IOrderRepository orderRepository,
        CancellationToken cancellationToken)
    {
        try
        {
            var query = orderRepository.GetAllQueryable();
            var now = DateTime.UtcNow;

            // Buscar todos os orders não deletados com responsável
            var orders = await query
                .Where(o => !o.IsDeleted && o.ResponsibleUserId != Guid.Empty)
                .ToListAsync(cancellationToken);

            // Agrupar por responsável
            var agentStats = orders
                .GroupBy(o => new { o.ResponsibleUserId, o.ResponsibleUserEmail })
                .Select(g =>
                {
                    var agentOrders = g.ToList();
                    var open = agentOrders.Count(o => o.ClosingDate == null);
                    var closed = agentOrders.Count(o => o.ClosingDate.HasValue);
                    var overdue = agentOrders.Count(o => o.ClosingDate == null && o.DueDate.HasValue && o.DueDate < now);

                    var closedWithDates = agentOrders
                        .Where(o => o.ClosingDate.HasValue && o.OpeningDate.HasValue)
                        .ToList();

                    var avgResolutionDays = closedWithDates.Any()
                        ? Math.Round(closedWithDates.Average(o => (o.ClosingDate!.Value - o.OpeningDate!.Value).TotalDays), 1)
                        : 0;

                    return new AgentStats
                    {
                        UserId = g.Key.ResponsibleUserId,
                        Email = g.Key.ResponsibleUserEmail,
                        OpenTickets = open,
                        ClosedTickets = closed,
                        OverdueTickets = overdue,
                        TotalTickets = agentOrders.Count,
                        AvgResolutionDays = avgResolutionDays
                    };
                })
                .OrderByDescending(a => a.OpenTickets)
                .ToList();

            var response = new TeamStatsResponse
            {
                TotalAgents = agentStats.Count,
                TotalOpenTickets = agentStats.Sum(a => a.OpenTickets),
                TotalOverdueTickets = agentStats.Sum(a => a.OverdueTickets),
                Agents = agentStats
            };

            return TypedResults.Ok(new BaseResult<TeamStatsResponse>(response));
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(new BaseResult<TeamStatsResponse>(
                null!, false, $"Erro ao calcular estatísticas da equipe: {ex.Message}"));
        }
    }
}

public sealed class TeamStatsResponse
{
    public int TotalAgents { get; set; }
    public int TotalOpenTickets { get; set; }
    public int TotalOverdueTickets { get; set; }
    public List<AgentStats> Agents { get; set; } = new();
}

public sealed class AgentStats
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public int OpenTickets { get; set; }
    public int ClosedTickets { get; set; }
    public int OverdueTickets { get; set; }
    public int TotalTickets { get; set; }
    public double AvgResolutionDays { get; set; }
}
