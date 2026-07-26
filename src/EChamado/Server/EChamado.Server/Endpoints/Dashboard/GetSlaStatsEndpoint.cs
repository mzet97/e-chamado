using EChamado.Server.Common.Api;
using EChamado.Server.Domain.Repositories.Orders;
using EChamado.Shared.Responses;
using Microsoft.EntityFrameworkCore;

namespace EChamado.Server.Endpoints.Dashboard;

/// <summary>
/// Endpoint de estatísticas de SLA.
/// Calcula compliance baseado em DueDate dos orders.
/// </summary>
public sealed class GetSlaStatsEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/sla/stats", HandleAsync)
            .WithName("Estatísticas de SLA")
            .WithTags("Dashboard", "SLA")
            .Produces<BaseResult<SlaStatsResponse>>();

    private static async Task<IResult> HandleAsync(
        [Microsoft.AspNetCore.Mvc.FromServices] IOrderRepository orderRepository,
        CancellationToken cancellationToken)
    {
        try
        {
            var query = orderRepository.GetAllQueryable();

            var now = DateTime.UtcNow;
            var totalOpen = await query
                .Where(o => !o.IsDeleted && o.ClosingDate == null)
                .CountAsync(cancellationToken);

            var overdue = await query
                .Where(o => !o.IsDeleted && o.ClosingDate == null &&
                            o.DueDate != null && o.DueDate < now)
                .CountAsync(cancellationToken);

            var atRisk = await query
                .Where(o => !o.IsDeleted && o.ClosingDate == null &&
                            o.DueDate != null &&
                            o.DueDate > now &&
                            o.DueDate < now.AddHours(24))
                .CountAsync(cancellationToken);

            var onTime = totalOpen - overdue - atRisk;

            var closedOnTime = await query
                .Where(o => !o.IsDeleted && o.ClosingDate != null &&
                            o.DueDate != null && o.ClosingDate <= o.DueDate)
                .CountAsync(cancellationToken);

            var closedLate = await query
                .Where(o => !o.IsDeleted && o.ClosingDate != null &&
                            o.DueDate != null && o.ClosingDate > o.DueDate)
                .CountAsync(cancellationToken);

            var totalClosed = closedOnTime + closedLate;
            var complianceRate = totalClosed > 0
                ? Math.Round((double)closedOnTime / totalClosed * 100, 1)
                : 0;

            var stats = new SlaStatsResponse
            {
                TotalOpen = totalOpen,
                Overdue = overdue,
                AtRisk = atRisk,
                OnTime = onTime,
                ClosedOnTime = closedOnTime,
                ClosedLate = closedLate,
                ComplianceRate = complianceRate
            };

            return TypedResults.Ok(new BaseResult<SlaStatsResponse>(stats));
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(new BaseResult<SlaStatsResponse>(
                null!, false, $"Erro ao calcular SLA: {ex.Message}"));
        }
    }
}

public sealed class SlaStatsResponse
{
    public int TotalOpen { get; set; }
    public int Overdue { get; set; }
    public int AtRisk { get; set; }
    public int OnTime { get; set; }
    public int ClosedOnTime { get; set; }
    public int ClosedLate { get; set; }
    public double ComplianceRate { get; set; }
}
