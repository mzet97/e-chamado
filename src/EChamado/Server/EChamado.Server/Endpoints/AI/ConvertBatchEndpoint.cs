using EChamado.Server.Application.Services.AI;
using EChamado.Server.Common.Api;
using Microsoft.AspNetCore.Mvc;

namespace EChamado.Server.Endpoints.AI;

/// <summary>
/// Endpoint para converter múltiplas consultas em lote
/// </summary>
public sealed class ConvertBatchEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPost("/nl-to-gridify-batch", HandleAsync)
            .WithName("Converter múltiplas consultas em lote")
            .WithTags("AI", "Gridify")
            .WithDescription("Converte múltiplas consultas em linguagem natural para Gridify em paralelo")
            .Produces<BatchNLResponse>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

    private static async Task<IResult> HandleAsync(
        [FromServices] NLToGridifyService nlToGridifyService,
        [FromBody] BatchNLRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var results = await nlToGridifyService.ConvertBatchAsync(
                entityName: request.EntityName,
                naturalLanguageQueries: request.Queries,
                providerName: request.Provider,
                cancellationToken: cancellationToken);

            var responses = results.Select(result => new NLToGridifyResponse
            {
                Success = result.Success,
                GridifyQuery = result.GridifyQuery,
                OriginalQuery = result.OriginalQuery,
                EntityName = result.EntityName,
                Provider = result.Provider,
                Model = result.Model,
                FromCache = result.FromCache,
                ResponseTimeMs = (int)result.ResponseTime.TotalMilliseconds,
                TokensUsed = result.TokensUsed,
                ErrorMessage = result.ErrorMessage
            }).ToList();

            return TypedResults.Ok(new BatchNLResponse
            {
                Success = responses.All(r => r.Success),
                Results = responses,
                TotalCount = responses.Count,
                SuccessCount = responses.Count(r => r.Success)
            });
        }
        catch (Exception)
        {
            return TypedResults.BadRequest(new BatchNLResponse
            {
                Success = false,
                Results = new List<NLToGridifyResponse>(),
                TotalCount = 0,
                SuccessCount = 0
            });
        }
    }
}

public sealed record BatchNLRequest
{
    public required string EntityName { get; init; }
    public required List<string> Queries { get; init; }
    public string? Provider { get; init; }
}

public sealed record BatchNLResponse
{
    public required bool Success { get; init; }
    public required List<NLToGridifyResponse> Results { get; init; }
    public int TotalCount { get; init; }
    public int SuccessCount { get; init; }
}
