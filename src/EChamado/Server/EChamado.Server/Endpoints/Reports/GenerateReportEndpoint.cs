using EChamado.Server.Common.Api;
using EChamado.Server.Domain.Repositories.Orders;
using EChamado.Shared.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;

namespace EChamado.Server.Endpoints.Reports;

/// <summary>
/// Endpoint para gerar relatórios analíticos.
/// Retorna dados em JSON (PDF/Excel podem ser adicionados com QuestPDF/ClosedXML).
/// </summary>
public sealed class GenerateReportEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPost("/generate", HandleAsync)
            .WithName("Gerar relatório")
            .WithTags("Reports")
            .Produces<BaseResult<ReportResult>>()
            .Produces(StatusCodes.Status400BadRequest);

    private static async Task<IResult> HandleAsync(
        [FromServices] IOrderRepository orderRepository,
        [FromBody] GenerateReportRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var query = orderRepository.GetAllQueryable();

            // Aplicar filtros de período
            if (request.FromDate.HasValue)
                query = query.Where(o => o.CreatedAtUtc >= request.FromDate.Value);

            if (request.ToDate.HasValue)
                query = query.Where(o => o.CreatedAtUtc <= request.ToDate.Value);

            if (request.DepartmentId.HasValue)
                query = query.Where(o => o.DepartmentId == request.DepartmentId.Value);

            var orders = await query.ToListAsync(cancellationToken);

            var report = request.ReportType.ToLower() switch
            {
                "volume" => GenerateVolumeReport(orders, request),
                "performance" => GeneratePerformanceReport(orders, request),
                "sla" => GenerateSlaReport(orders, request),
                _ => GenerateVolumeReport(orders, request)
            };

            return TypedResults.Ok(new BaseResult<ReportResult>(report));
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(new BaseResult<ReportResult>(
                null!, false, $"Erro ao gerar relatório: {ex.Message}"));
        }
    }

    private static ReportResult GenerateVolumeReport(
        List<Domain.Domains.Orders.Order> orders, GenerateReportRequest request)
    {
        var byMonth = orders
            .GroupBy(o => o.OpeningDate?.ToString("yyyy-MM") ?? "N/A")
            .ToDictionary(g => g.Key, g => g.Count());

        var byStatus = orders
            .GroupBy(o => o.StatusId.ToString())
            .ToDictionary(g => g.Key, g => g.Count());

        return new ReportResult
        {
            ReportType = "Volume por Período",
            GeneratedAt = DateTime.UtcNow,
            TotalRecords = orders.Count,
            Summary = new Dictionary<string, object>
            {
                ["totalOrders"] = orders.Count,
                ["byMonth"] = byMonth,
                ["byStatus"] = byStatus
            }
        };
    }

    private static ReportResult GeneratePerformanceReport(
        List<Domain.Domains.Orders.Order> orders, GenerateReportRequest request)
    {
        var closed = orders.Where(o => o.ClosingDate.HasValue).ToList();
        var avgResolutionDays = closed.Any()
            ? closed.Average(o => (o.ClosingDate!.Value - o.OpeningDate!.Value).TotalDays)
            : 0;

        return new ReportResult
        {
            ReportType = "Performance",
            GeneratedAt = DateTime.UtcNow,
            TotalRecords = orders.Count,
            Summary = new Dictionary<string, object>
            {
                ["totalOrders"] = orders.Count,
                ["closedOrders"] = closed.Count,
                ["avgResolutionDays"] = Math.Round(avgResolutionDays, 1)
            }
        };
    }

    private static ReportResult GenerateSlaReport(
        List<Domain.Domains.Orders.Order> orders, GenerateReportRequest request)
    {
        var now = DateTime.UtcNow;
        var withDueDate = orders.Where(o => o.DueDate.HasValue).ToList();
        var overdue = withDueDate.Count(o => o.ClosingDate == null && o.DueDate < now);
        var closedOnTime = orders.Count(o => o.ClosingDate.HasValue && o.DueDate.HasValue && o.ClosingDate <= o.DueDate);

        return new ReportResult
        {
            ReportType = "SLA Compliance",
            GeneratedAt = DateTime.UtcNow,
            TotalRecords = orders.Count,
            Summary = new Dictionary<string, object>
            {
                ["totalWithDueDate"] = withDueDate.Count,
                ["overdue"] = overdue,
                ["closedOnTime"] = closedOnTime,
                ["complianceRate"] = withDueDate.Any()
                    ? Math.Round((double)closedOnTime / withDueDate.Count * 100, 1)
                    : 0
            }
        };
    }
}

public sealed class GenerateReportRequest
{
    public string ReportType { get; set; } = "volume"; // volume, performance, sla
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public Guid? DepartmentId { get; set; }
}

public sealed class ReportResult
{
    public string ReportType { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; }
    public int TotalRecords { get; set; }
    public Dictionary<string, object> Summary { get; set; } = new();
}
