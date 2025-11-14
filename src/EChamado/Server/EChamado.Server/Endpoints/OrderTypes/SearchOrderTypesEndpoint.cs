using EChamado.Server.Application.Common.Messaging;
using EChamado.Server.Application.UseCases.OrderTypes.Queries;
using EChamado.Server.Application.UseCases.OrderTypes.ViewModels;
using EChamado.Server.Common.Api;
using EChamado.Shared.Responses;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;

namespace EChamado.Server.Endpoints.OrderTypes;

public class SearchOrderTypesEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/", HandleAsync)
            .WithName("Buscar tipos de chamados")
            .Produces<BaseResultList<OrderTypeViewModel>>();

    private static async Task<IResult> HandleAsync(
        [FromServices] IAmACommandProcessor commandProcessor,
        [AsParameters] SearchOrderTypesParameters parameters)
    {
        var query = new SearchOrderTypesQuery
        {
            Name = parameters.Name ?? string.Empty,
            Description = parameters.Description ?? string.Empty,
            CreatedAt = parameters.CreatedAt,
            UpdatedAt = parameters.UpdatedAt,
            DeletedAt = parameters.DeletedAt,
            Order = parameters.Order,
            PageIndex = parameters.PageIndex,
            PageSize = parameters.PageSize
        };

        var result = await commandProcessor.SendWithResultAsync(query);

        if (result.Success)
            return TypedResults.Ok(result);

        return TypedResults.BadRequest(result);
    }
}

public class SearchOrderTypesParameters
{
    [FromQuery] public string? Name { get; set; }
    [FromQuery] public string? Description { get; set; }
    [FromQuery] public DateTime CreatedAt { get; set; }
    [FromQuery] public DateTime UpdatedAt { get; set; }
    [FromQuery] public DateTime? DeletedAt { get; set; }
    [FromQuery] public string? Order { get; set; }
    [FromQuery] public int PageIndex { get; set; } = 1;
    [FromQuery] public int PageSize { get; set; } = 10;
}
