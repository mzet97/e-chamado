using EChamado.Server.Application.Services.AI.Interfaces;
using EChamado.Server.Application.Services.AI.Models;
using EChamado.Server.Application.Services.AI.Prompts;
using EChamado.Server.Application.Services.AI.Providers;
using Microsoft.Extensions.Logging;

namespace EChamado.Server.Application.Services.AI;

/// <summary>
/// Service for converting Natural Language queries to Gridify syntax
/// </summary>
public sealed class NLToGridifyService
{
    private readonly AIProviderFactory _providerFactory;
    private readonly ILogger<NLToGridifyService> _logger;

    public NLToGridifyService(
        AIProviderFactory providerFactory,
        ILogger<NLToGridifyService> logger)
    {
        _providerFactory = providerFactory;
        _logger = logger;
    }

    /// <summary>
    /// Convert natural language query to Gridify query syntax
    /// </summary>
    /// <param name="entityName">Entity name (Order, Category, etc.)</param>
    /// <param name="naturalLanguageQuery">The natural language query</param>
    /// <param name="providerName">Optional provider name (defaults to configured provider)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Conversion result with Gridify query</returns>
    public async Task<NLToGridifyResult> ConvertAsync(
        string entityName,
        string naturalLanguageQuery,
        string? providerName = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation(
                "Converting NL query to Gridify. Entity: {Entity}, Query: {Query}",
                entityName, naturalLanguageQuery);

            // Get AI provider
            var provider = string.IsNullOrWhiteSpace(providerName)
                ? _providerFactory.GetDefaultProvider()
                : _providerFactory.GetProvider(providerName);

            // Build prompt
            var systemMessage = GridifyPromptTemplates.SystemMessage;
            var userPrompt = GridifyPromptTemplates.GetEntityPrompt(entityName, naturalLanguageQuery);

            var aiRequest = new AIRequest
            {
                SystemMessage = systemMessage,
                Prompt = userPrompt,
                Temperature = 0.1, // Low temperature for consistent, deterministic outputs
                MaxTokens = 500
            };

            // Call AI
            var aiResponse = await provider.GenerateAsync(aiRequest, cancellationToken);

            // Clean the response (remove markdown, code blocks, etc.)
            var gridifyQuery = CleanGridifyQuery(aiResponse.Content);

            _logger.LogInformation(
                "NL to Gridify conversion successful. Input: '{Input}' → Output: '{Output}' (Provider: {Provider}, Cached: {Cached})",
                naturalLanguageQuery, gridifyQuery, aiResponse.Provider, aiResponse.FromCache);

            return new NLToGridifyResult
            {
                Success = true,
                GridifyQuery = gridifyQuery,
                OriginalQuery = naturalLanguageQuery,
                EntityName = entityName,
                Provider = aiResponse.Provider,
                Model = aiResponse.Model,
                FromCache = aiResponse.FromCache,
                ResponseTime = aiResponse.ResponseTime,
                TokensUsed = aiResponse.TotalTokens
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error converting NL query to Gridify. Entity: {Entity}, Query: {Query}",
                entityName, naturalLanguageQuery);

            return new NLToGridifyResult
            {
                Success = false,
                ErrorMessage = ex.Message,
                OriginalQuery = naturalLanguageQuery,
                EntityName = entityName
            };
        }
    }

    /// <summary>
    /// Convert natural language query to Gridify query with ordering
    /// </summary>
    public async Task<NLToGridifyResult> ConvertWithOrderingAsync(
        string entityName,
        string naturalLanguageQuery,
        string? defaultOrdering = null,
        string? providerName = null,
        CancellationToken cancellationToken = default)
    {
        var result = await ConvertAsync(entityName, naturalLanguageQuery, providerName, cancellationToken);

        if (result.Success && !string.IsNullOrWhiteSpace(defaultOrdering))
        {
            // If the query doesn't contain ordering, add default
            if (!result.GridifyQuery.Contains("ORDER:", StringComparison.OrdinalIgnoreCase))
            {
                result = result with
                {
                    GridifyQuery = $"{result.GridifyQuery}".Trim(),
                    SuggestedOrdering = defaultOrdering
                };
            }
        }

        return result;
    }

    /// <summary>
    /// Batch convert multiple queries
    /// </summary>
    public async Task<IEnumerable<NLToGridifyResult>> ConvertBatchAsync(
        string entityName,
        IEnumerable<string> naturalLanguageQueries,
        string? providerName = null,
        CancellationToken cancellationToken = default)
    {
        var tasks = naturalLanguageQueries.Select(query =>
            ConvertAsync(entityName, query, providerName, cancellationToken));

        return await Task.WhenAll(tasks);
    }

    /// <summary>
    /// Clean and normalize the Gridify query from AI response
    /// </summary>
    private static string CleanGridifyQuery(string rawQuery)
    {
        // Remove markdown code blocks
        var cleaned = rawQuery
            .Replace("```gridify", "")
            .Replace("```", "")
            .Trim();

        // Remove common prefixes
        var prefixesToRemove = new[]
        {
            "Query:",
            "Gridify Query:",
            "Gridify:",
            "Result:",
            "Output:"
        };

        foreach (var prefix in prefixesToRemove)
        {
            if (cleaned.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                cleaned = cleaned[prefix.Length..].Trim();
            }
        }

        // Remove quotes at the beginning and end if present
        if (cleaned.StartsWith('"') && cleaned.EndsWith('"'))
        {
            cleaned = cleaned[1..^1];
        }

        if (cleaned.StartsWith('\'') && cleaned.EndsWith('\''))
        {
            cleaned = cleaned[1..^1];
        }

        return cleaned.Trim();
    }
}

/// <summary>
/// Result of NL to Gridify conversion
/// </summary>
public sealed record NLToGridifyResult
{
    public required bool Success { get; init; }
    public string GridifyQuery { get; init; } = string.Empty;
    public string? SuggestedOrdering { get; init; }
    public required string OriginalQuery { get; init; }
    public required string EntityName { get; init; }
    public string? Provider { get; init; }
    public string? Model { get; init; }
    public bool FromCache { get; init; }
    public TimeSpan ResponseTime { get; init; }
    public int TokensUsed { get; init; }
    public string? ErrorMessage { get; init; }
}
