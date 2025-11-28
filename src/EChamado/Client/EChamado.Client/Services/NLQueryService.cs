using System.Net.Http.Json;
using EChamado.Client.Models;
using Microsoft.Extensions.Logging;

namespace EChamado.Client.Services;

/// <summary>
/// Service for converting Natural Language queries to Gridify syntax via API
/// </summary>
public interface INLQueryService
{
    /// <summary>
    /// Convert natural language query to Gridify query
    /// </summary>
    Task<NLToGridifyResult?> ConvertToGridifyAsync(string entityName, string query, string? provider = null);
}

public sealed class NLQueryService : INLQueryService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<NLQueryService> _logger;

    public NLQueryService(HttpClient httpClient, ILogger<NLQueryService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<NLToGridifyResult?> ConvertToGridifyAsync(
        string entityName,
        string query,
        string? provider = null)
    {
        try
        {
            _logger.LogInformation("Converting NL query: {Query} for entity: {Entity}", query, entityName);

            var request = new NLToGridifyRequest
            {
                EntityName = entityName,
                Query = query,
                Provider = provider
            };

            var response = await _httpClient.PostAsJsonAsync("/v1/ai/nl-to-gridify", request);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<NLToGridifyResult>();
                _logger.LogInformation(
                    "Conversion successful: {Original} → {Gridify} (Provider: {Provider}, Cached: {Cached})",
                    result?.OriginalQuery, result?.GridifyQuery, result?.Provider, result?.FromCache);
                return result;
            }

            _logger.LogError("Conversion failed with status: {Status}", response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error converting NL query to Gridify");
            return null;
        }
    }
}

/// <summary>
/// Request model for NL to Gridify conversion
/// </summary>
public sealed record NLToGridifyRequest
{
    public required string EntityName { get; init; }
    public required string Query { get; init; }
    public string? Provider { get; init; }
}

/// <summary>
/// Result model from NL to Gridify conversion
/// </summary>
public sealed record NLToGridifyResult
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
