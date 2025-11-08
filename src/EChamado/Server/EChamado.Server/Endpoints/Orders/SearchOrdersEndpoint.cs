using EChamado.Server.Application.UseCases.Orders.Queries;
using EChamado.Server.Application.UseCases.Orders.ViewModels;
using EChamado.Server.Common.Api;
using EChamado.Shared.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EChamado.Server.Endpoints.Orders;

public class SearchOrdersEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
       => app.MapGet("/", HandleAsync)
           .WithName("Buscar chamados")
           .WithSummary("Buscar chamados")
           .WithDescription("Buscar chamados com filtros e paginação")
           .WithOrder(1)
           .Produces<BaseResultList<OrderListViewModel>>();

    private static async Task<IResult> HandleAsync(
        IMediator mediator,
        [AsParameters] SearchOrdersParameters parameters)
    {
        var query = new SearchOrdersQuery
        {
            SearchText = parameters.SearchText ?? "",
            StatusId = parameters.StatusId,
            DepartmentId = parameters.DepartmentId,
            TypeId = parameters.TypeId,
            CategoryId = parameters.CategoryId,
            RequestingUserId = parameters.RequestingUserId,
            AssignedToUserId = parameters.AssignedToUserId,
            StartDate = parameters.StartDate,
            EndDate = parameters.EndDate,
            IsOverdue = parameters.IsOverdue,
            PageNumber = parameters.PageNumber ?? 1,
            PageSize = parameters.PageSize ?? 10
        };

        var result = await mediator.Send(query);

        if (result.Success)
        {
            return TypedResults.Ok(result);
        }

        return TypedResults.BadRequest(result);
    }
}

public class SearchOrdersParameters
{
    [FromQuery] public string? SearchText { get; set; }
    [FromQuery] public Guid? StatusId { get; set; }
    [FromQuery] public Guid? DepartmentId { get; set; }
    [FromQuery] public Guid? TypeId { get; set; }
    [FromQuery] public Guid? CategoryId { get; set; }
    [FromQuery] public Guid? RequestingUserId { get; set; }
    [FromQuery] public Guid? AssignedToUserId { get; set; }
    [FromQuery] public DateTime? StartDate { get; set; }
    [FromQuery] public DateTime? EndDate { get; set; }
    [FromQuery] public bool? IsOverdue { get; set; }
    [FromQuery] public int? PageNumber { get; set; } = 1;
    [FromQuery] public int? PageSize { get; set; } = 10;
}
