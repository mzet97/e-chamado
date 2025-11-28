namespace EChamado.Server.Application.Services.AI.Models;

/// <summary>
/// Request model for AI provider interactions
/// </summary>
public sealed class AIRequest
{
    /// <summary>
    /// The prompt/question to send to the AI
    /// </summary>
    public required string Prompt { get; init; }

    /// <summary>
    /// System message to guide AI behavior
    /// </summary>
    public string? SystemMessage { get; init; }

    /// <summary>
    /// Temperature for response randomness (0.0-1.0)
    /// </summary>
    public double Temperature { get; init; } = 0.1;

    /// <summary>
    /// Maximum tokens in the response
    /// </summary>
    public int MaxTokens { get; init; } = 500;

    /// <summary>
    /// Optional conversation history for context
    /// </summary>
    public List<ChatMessage>? ConversationHistory { get; init; }
}

/// <summary>
/// Chat message for conversation history
/// </summary>
public sealed class ChatMessage
{
    public required string Role { get; init; } // "user", "assistant", "system"
    public required string Content { get; init; }
}
