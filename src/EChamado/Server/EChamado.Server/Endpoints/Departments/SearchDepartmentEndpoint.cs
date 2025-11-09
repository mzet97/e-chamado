using EChamado.Server.Application.Common.Messaging;
using EChamado.Server.Application.UseCases.Departments.Queries;
using EChamado.Server.Application.UseCases.Departments.ViewModels;
using EChamado.Server.Common.Api;
using EChamado.Shared.Responses;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;

namespace EChamado.Server.Endpoints.Departments;

public class SearchDepartmentEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
       => app.MapGet("/", HandleAsync)
           .WithName("Busca departamento")
           .WithSummary("Busca departamento")
           .WithDescription("Busca departamento")
           .WithOrder(2)
           .Produces<BaseResultList<DepartmentViewModel>>();

    private static async Task<IResult> HandleAsync(
        IAmACommandProcessor commandProcessor,
        [AsParameters] SearchDepartment search)
    {
        var query = new SearchDepartmentQuery
        {
            Name = search.Name ?? "",
            Description = search.Description ?? "",
            Id = search.Id ?? Guid.Empty,
            CreatedAt = search.CreatedAt ?? default,
            UpdatedAt = search.UpdatedAt ?? default,
            DeletedAt = search.DeletedAt ?? default,
            Order = search.Order ?? "",
            PageIndex = search.PageIndex ?? 1,
            PageSize = search.PageSize ?? 10,
        };

        var result = await commandProcessor.Send(query);

        if (result.Success)
        {
            return TypedResults.Ok(result);
        }

        return TypedResults.BadRequest(result);
    }
}

public class SearchDepartment
{
    [FromQuery] public string? Name { get; set; }
    [FromQuery] public string? Description { get; set; }
    [FromQuery] public Guid? Id { get; set; }
    [FromQuery] public DateTime? CreatedAt { get; set; }
    [FromQuery] public DateTime? UpdatedAt { get; set; }
    [FromQuery] public DateTime? DeletedAt { get; set; }
    [FromQuery] public string? Order { get; set; }
    [FromQuery] public int? PageIndex { get; set; } = 1;
    [FromQuery] public int? PageSize { get; set; } = 10;
}