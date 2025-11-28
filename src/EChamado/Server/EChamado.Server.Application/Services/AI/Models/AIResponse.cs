namespace EChamado.Server.Application.Services.AI.Models;

/// <summary>
/// Response model from AI provider
/// </summary>
public sealed class AIResponse
{
    /// <summary>
    /// The generated response text
    /// </summary>
    public required string Content { get; init; }

    /// <summary>
    /// Model used for generation
    /// </summary>
    public required string Model { get; init; }

    /// <summary>
    /// Provider name (OpenAI, Gemini, etc.)
    /// </summary>
    public required string Provider { get; init; }

    /// <summary>
    /// Total tokens used in the request
    /// </summary>
    public int TotalTokens { get; init; }

    /// <summary>
    /// Time taken to generate the response
    /// </summary>
    public TimeSpan ResponseTime { get; init; }

    /// <summary>
    /// Indicates if the response was retrieved from cache
    /// </summary>
    public bool FromCache { get; init; }
}
