using EChamado.Server.Application.Services.AI;
using EChamado.Server.Common.Api;
using Microsoft.AspNetCore.Mvc;

namespace EChamado.Server.Endpoints.AI;

/// <summary>
/// Endpoint for converting Natural Language to Gridify query syntax
/// </summary>
public sealed class ConvertNLToGridifyEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPost("/nl-to-gridify", HandleAsync)
            .WithName("Converter consulta em linguagem natural para Gridify")
            .WithTags("AI", "Gridify")
            .WithDescription("Converte uma consulta em linguagem natural para sintaxe Gridify")
            .Produces<NLToGridifyResponse>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .WithOpenApi();

    private static async Task<IResult> HandleAsync(
        [FromServices] NLToGridifyService nlToGridifyService,
        [FromBody] NLToGridifyRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await nlToGridifyService.ConvertAsync(
                entityName: request.EntityName,
                naturalLanguageQuery: request.Query,
                providerName: request.Provider,
                cancellationToken: cancellationToken);

            var response = new NLToGridifyResponse
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
            };

            if (result.Success)
            {
                return TypedResults.Ok(response);
            }

            return TypedResults.BadRequest(response);
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(new NLToGridifyResponse
            {
                Success = false,
                ErrorMessage = $"Erro ao processar consulta: {ex.Message}",
                OriginalQuery = request.Query,
                EntityName = request.EntityName
            });
        }
    }
}

/// <summary>
/// Request for NL to Gridify conversion
/// </summary>
public sealed record NLToGridifyRequest
{
    /// <summary>
    /// Entity name (Order, Category, Department, etc.)
    /// </summary>
    public required string EntityName { get; init; }

    /// <summary>
    /// Natural language query
    /// </summary>
    public required string Query { get; init; }

    /// <summary>
    /// Optional provider name (OpenAI, Gemini, OpenRouter)
    /// </summary>
    public string? Provider { get; init; }
}

/// <summary>
/// Response from NL to Gridify conversion
/// </summary>
public sealed record NLToGridifyResponse
{
    public required bool Success { get; init; }
    public string GridifyQuery { get; init; } = string.Empty;
    public required string OriginalQuery { get; init; }
    public required string EntityName { get; init; }
    public string? Provider { get; init; }
    public string? Model { get; init; }
    public bool FromCache { get; init; }
    public int ResponseTimeMs { get; init; }
    public int TokensUsed { get; init; }
    public string? ErrorMessage { get; init; }
}
