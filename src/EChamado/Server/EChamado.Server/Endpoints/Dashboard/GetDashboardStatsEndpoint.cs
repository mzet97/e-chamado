using EChamado.Server.Common.Api;
using EChamado.Server.Domain.Domains.Orders;
using EChamado.Server.Domain.Repositories.Orders;
using EChamado.Shared.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EChamado.Server.Endpoints.Dashboard;

public class GetDashboardStatsEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/stats", HandleAsync)
            .WithName("Get Dashboard Stats")
            .Produces<BaseResult<DashboardStatsResponse>>();

    private static async Task<IResult> HandleAsync(
        [FromServices] IOrderRepository orderRepository,
        [FromQuery] Guid? userId)
    {
        try
        {
            var query = orderRepository.GetAllQueryable();

            // Execução sequencial para evitar InvalidOperationException
            // (EF Core não suporta operações concorrentes no mesmo DbContext)
            var total = await query.CountAsync();
            var overdue = await query.CountAsync(o =>
                o.DueDate != null && o.DueDate < DateTime.UtcNow && o.ClosingDate == null);

            int myTickets = 0;
            int assignedToMe = 0;

            if (userId.HasValue && userId.Value != Guid.Empty)
            {
                myTickets = await query.CountAsync(o => o.RequestingUserId == userId.Value);
                assignedToMe = await query.CountAsync(o => o.ResponsibleUserId == userId.Value);
            }

            var stats = new DashboardStatsResponse
            {
                TotalTickets = total,
                MyTickets = myTickets,
                AssignedToMe = assignedToMe,
                OverdueTickets = overdue
            };

            return TypedResults.Ok(new BaseResult<DashboardStatsResponse>(
                data: stats,
                success: true,
                message: "Dashboard statistics retrieved successfully"));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Dashboard] ERRO: {ex.GetType().Name}: {ex.Message}");
            Console.WriteLine($"[Dashboard] StackTrace: {ex.StackTrace}");
            return TypedResults.BadRequest(new BaseResult<DashboardStatsResponse>(
                data: null,
                success: false,
                message: $"Erro ao processar a solicitacao: {ex.Message}"));
        }
    }
}
