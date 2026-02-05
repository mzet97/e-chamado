using EChamado.Server.Application.Common.Messaging;
using EChamado.Server.Application.UseCases.StatusTypes.Queries;
using EChamado.Server.Application.UseCases.StatusTypes.ViewModels;
using EChamado.Server.Common.Api;
using EChamado.Shared.Responses;
using Paramore.Brighter;
using Microsoft.AspNetCore.Mvc;

namespace EChamado.Server.Endpoints.StatusTypes;

public class SearchStatusTypesEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/", HandleAsync)
            .WithName("Buscar status de chamados")
            .Produces<BaseResultList<StatusTypeViewModel>>();

    private static async Task<IResult> HandleAsync(
        [FromServices] IAmACommandProcessor commandProcessor,
        [AsParameters] SearchStatusTypesParameters parameters)
    {
        try
        {
            var query = new SearchStatusTypesQuery
            {
                Name = parameters.Name ?? string.Empty,
                Description = parameters.Description ?? string.Empty,
                CreatedAt = parameters.CreatedAt ?? default,
                UpdatedAt = parameters.UpdatedAt ?? default,
                DeletedAt = parameters.DeletedAt ?? default,
                Order = parameters.Order ?? string.Empty,
                PageIndex = parameters.PageIndex ?? 1,
                PageSize = parameters.PageSize ?? 10
            };

            await commandProcessor.SendAsync(query);

            return query.Result.Success
                ? TypedResults.Ok(query.Result)
                : TypedResults.BadRequest(query.Result);
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(new BaseResultList<StatusTypeViewModel>(
                new List<StatusTypeViewModel>(),
                null,
                false,
                $"Erro interno: {ex.Message}"));
        }
    }
}

public class SearchStatusTypesParameters
{
    [FromQuery] public Guid? Id { get; set; }
    [FromQuery] public string? Name { get; set; }
    [FromQuery] public string? Description { get; set; }
    [FromQuery] public DateTime? CreatedAt { get; set; }
    [FromQuery] public DateTime? UpdatedAt { get; set; }
    [FromQuery] public DateTime? DeletedAt { get; set; }
    [FromQuery] public string? Order { get; set; }
    [FromQuery] public int? PageIndex { get; set; } = 1;
    [FromQuery] public int? PageSize { get; set; } = 10;
}
