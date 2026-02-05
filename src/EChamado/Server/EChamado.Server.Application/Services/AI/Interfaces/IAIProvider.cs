using EChamado.Server.Application.Services.AI.Models;

namespace EChamado.Server.Application.Services.AI.Interfaces;

/// <summary>
/// Interface for AI providers (OpenAI, Gemini, OpenRouter, etc.)
/// </summary>
public interface IAIProvider
{
    /// <summary>
    /// Generate a response from the AI provider
    /// </summary>
    /// <param name="request">The AI request with prompt and parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>AI response with generated content</returns>
    Task<AIResponse> GenerateAsync(AIRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Provider name (e.g., "OpenAI", "Gemini", "OpenRouter")
    /// </summary>
    string ProviderName { get; }

    /// <summary>
    /// Check if the provider is available and configured
    /// </summary>
    bool IsAvailable { get; }
}
