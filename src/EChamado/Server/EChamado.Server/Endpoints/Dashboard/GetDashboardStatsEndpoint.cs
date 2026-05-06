using EChamado.Server.Common.Api;
using EChamado.Server.Domain.Domains.Orders;
using EChamado.Server.Domain.Repositories;
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
        [FromServices] IRepository<Order> orderRepository,
        [FromQuery] Guid? userId)
    {
        try
        {
            var query = orderRepository.GetAllQueryable();

            var totalTask = query.CountAsync();
            var overdueTask = query.CountAsync(o =>
                o.DueDate != null && o.DueDate < DateTime.UtcNow && o.ClosingDate == null);

            int myTickets = 0;
            int assignedToMe = 0;

            if (userId.HasValue && userId.Value != Guid.Empty)
            {
                myTickets = await query.CountAsync(o => o.RequestingUserId == userId.Value);
                assignedToMe = await query.CountAsync(o => o.ResponsibleUserId == userId.Value);
            }

            var total = await totalTask;
            var overdue = await overdueTask;

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
        catch (Exception)
        {
            return TypedResults.BadRequest(new BaseResult<DashboardStatsResponse>(
                data: null,
                success: false,
                message: "Erro ao processar a solicitacao."));
        }
    }
}
