using EChamado.Server.Application.Services.AI;
using EChamado.Server.Common.Api;
using Microsoft.AspNetCore.Mvc;

namespace EChamado.Server.Endpoints.AI;

/// <summary>
/// Endpoint para converter linguagem natural para Gridify com ordenação sugerida
/// </summary>
public sealed class ConvertWithOrderingEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPost("/nl-to-gridify-with-ordering", HandleAsync)
            .WithName("Converter consulta com ordenação sugerida")
            .WithTags("AI", "Gridify")
            .WithDescription("Converte linguagem natural para Gridify com ordenação padrão sugerida")
            .Produces<NLToGridifyResponse>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

    private static async Task<IResult> HandleAsync(
        [FromServices] NLToGridifyService nlToGridifyService,
        [FromBody] NLWithOrderingRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await nlToGridifyService.ConvertWithOrderingAsync(
                entityName: request.EntityName,
                naturalLanguageQuery: request.Query,
                defaultOrdering: request.DefaultOrdering,
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

            return result.Success ? TypedResults.Ok(response) : TypedResults.BadRequest(response);
        }
        catch (Exception)
        {
            return TypedResults.BadRequest(new NLToGridifyResponse
            {
                Success = false,
                ErrorMessage = "Erro ao processar a solicitacao.",
                OriginalQuery = request.Query,
                EntityName = request.EntityName
            });
        }
    }
}

public sealed record NLWithOrderingRequest
{
    public required string EntityName { get; init; }
    public required string Query { get; init; }
    public string? DefaultOrdering { get; init; }
    public string? Provider { get; init; }
}
