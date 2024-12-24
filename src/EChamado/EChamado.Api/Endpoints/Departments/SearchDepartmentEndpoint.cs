using EChamado.Api.Common.Api;
using EChamado.Application.Features.Departments.Queries;
using EChamado.Application.Features.Departments.ViewModels;
using EChamado.Core.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EChamado.Api.Endpoints.Departments;

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
        IMediator mediator,
        [FromQuery] string? Name,
        [FromQuery] string? Description,
        [FromQuery] Guid? Id,
        [FromQuery] DateTime? CreatedAt,
        [FromQuery] DateTime? UpdatedAt,
        [FromQuery] DateTime? DeletedAt,
        [FromQuery] string? Order,
        [FromQuery] int PageIndex = 1,
        [FromQuery] int PageSize = 10)
    {
        var query = new SearchDepartmentQuery
        {
            Name = Name ?? "",
            Description = Description ?? "",
            Id = Id ?? Guid.Empty,
            CreatedAt = CreatedAt ?? default,
            UpdatedAt = UpdatedAt ?? default,
            DeletedAt = DeletedAt ?? default,
            Order = Order ?? "",
            PageIndex = PageIndex,
            PageSize = PageSize,
        };

        var result = await mediator.Send(query);

        if (result.Success)
        {
            return TypedResults.Ok(result);
        }

        return TypedResults.BadRequest(result);
    }
}
