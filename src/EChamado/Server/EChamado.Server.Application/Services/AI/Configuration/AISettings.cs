namespace EChamado.Server.Application.Services.AI.Configuration;

/// <summary>
/// Configuration settings for AI providers
/// </summary>
public sealed class AISettings
{
    public const string SectionName = "AISettings";

    /// <summary>
    /// Default AI provider to use (OpenAI, Gemini, OpenRouter)
    /// </summary>
    public string DefaultProvider { get; set; } = "OpenAI";

    /// <summary>
    /// Enable response caching to reduce costs
    /// </summary>
    public bool EnableCaching { get; set; } = true;

    /// <summary>
    /// Cache duration in minutes
    /// </summary>
    public int CacheDurationMinutes { get; set; } = 60;

    /// <summary>
    /// Enable request logging for debugging
    /// </summary>
    public bool EnableLogging { get; set; } = true;

    /// <summary>
    /// OpenAI provider settings
    /// </summary>
    public OpenAISettings OpenAI { get; set; } = new();

    /// <summary>
    /// Google Gemini provider settings
    /// </summary>
    public GeminiSettings Gemini { get; set; } = new();

    /// <summary>
    /// OpenRouter provider settings
    /// </summary>
    public OpenRouterSettings OpenRouter { get; set; } = new();
}

/// <summary>
/// OpenAI provider configuration
/// </summary>
public sealed class OpenAISettings
{
    /// <summary>
    /// OpenAI API Key
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Model to use (gpt-4o, gpt-4-turbo, gpt-3.5-turbo, etc.)
    /// </summary>
    public string Model { get; set; } = "gpt-4o-mini";

    /// <summary>
    /// API endpoint (optional, defaults to OpenAI official endpoint)
    /// </summary>
    public string? Endpoint { get; set; }

    /// <summary>
    /// Enable this provider
    /// </summary>
    public bool Enabled { get; set; } = true;
}

/// <summary>
/// Google Gemini provider configuration
/// </summary>
public sealed class GeminiSettings
{
    /// <summary>
    /// Google AI API Key
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Model to use (gemini-2.0-flash-exp, gemini-1.5-pro, gemini-1.5-flash, etc.)
    /// </summary>
    public string Model { get; set; } = "gemini-2.0-flash-exp";

    /// <summary>
    /// Enable this provider
    /// </summary>
    public bool Enabled { get; set; } = false;
}

/// <summary>
/// OpenRouter provider configuration
/// </summary>
public sealed class OpenRouterSettings
{
    /// <summary>
    /// OpenRouter API Key
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Model to use (meta-llama/llama-3-70b-instruct, anthropic/claude-3-opus, etc.)
    /// </summary>
    public string Model { get; set; } = "meta-llama/llama-3.1-70b-instruct";

    /// <summary>
    /// OpenRouter API endpoint
    /// </summary>
    public string Endpoint { get; set; } = "https://openrouter.ai/api/v1";

    /// <summary>
    /// Enable this provider
    /// </summary>
    public bool Enabled { get; set; } = false;
}
