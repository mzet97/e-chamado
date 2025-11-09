using EChamado.Server.Application.Common.Messaging;
using EChamado.Server.Application.UseCases.StatusTypes.Queries;
using EChamado.Server.Application.UseCases.StatusTypes.ViewModels;
using EChamado.Shared.Responses;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;

namespace EChamado.Server.Endpoints.StatusTypes;

public class SearchStatusTypesEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/", HandleAsync)
            .WithName("Buscar status de chamados")
            .Produces<BaseResultList<StatusTypeViewModel>>();

    private static async Task<IResult> HandleAsync(
        IAmACommandProcessor commandProcessor,
        [AsParameters] SearchStatusTypesParameters parameters)
    {
        var query = new SearchStatusTypesQuery
        {
            Id = parameters.Id,
            Name = parameters.Name ?? string.Empty,
            Description = parameters.Description ?? string.Empty,
            CreatedAt = parameters.CreatedAt,
            UpdatedAt = parameters.UpdatedAt,
            DeletedAt = parameters.DeletedAt,
            Order = parameters.Order,
            PageIndex = parameters.PageIndex,
            PageSize = parameters.PageSize
        };

        var result = await commandProcessor.Send(query);

        if (result.Success)
            return TypedResults.Ok(result);

        return TypedResults.BadRequest(result);
    }
}

public class SearchStatusTypesParameters
{
    [FromQuery] public Guid Id { get; set; }
    [FromQuery] public string? Name { get; set; }
    [FromQuery] public string? Description { get; set; }
    [FromQuery] public DateTime CreatedAt { get; set; }
    [FromQuery] public DateTime UpdatedAt { get; set; }
    [FromQuery] public DateTime? DeletedAt { get; set; }
    [FromQuery] public string? Order { get; set; }
    [FromQuery] public int PageIndex { get; set; } = 1;
    [FromQuery] public int PageSize { get; set; } = 10;
}
