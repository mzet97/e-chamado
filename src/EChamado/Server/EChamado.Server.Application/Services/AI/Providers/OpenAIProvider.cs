using System.Diagnostics;
using Azure.AI.OpenAI;
using EChamado.Server.Application.Services.AI.Configuration;
using EChamado.Server.Application.Services.AI.Interfaces;
using EChamado.Server.Application.Services.AI.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenAI.Chat;

namespace EChamado.Server.Application.Services.AI.Providers;

/// <summary>
/// OpenAI provider implementation using Azure.AI.OpenAI SDK
/// </summary>
public sealed class OpenAIProvider : IAIProvider
{
    private readonly OpenAISettings _settings;
    private readonly ILogger<OpenAIProvider> _logger;
    private readonly AzureOpenAIClient? _client;

    public OpenAIProvider(
        IOptions<AISettings> options,
        ILogger<OpenAIProvider> logger)
    {
        _settings = options.Value.OpenAI;
        _logger = logger;

        if (_settings.Enabled && !string.IsNullOrWhiteSpace(_settings.ApiKey))
        {
            try
            {
                _client = new AzureOpenAIClient(
                    new Uri(_settings.Endpoint ?? "https://api.openai.com/v1"),
                    new System.ClientModel.ApiKeyCredential(_settings.ApiKey));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize OpenAI client");
            }
        }
    }

    public string ProviderName => "OpenAI";

    public bool IsAvailable => _settings.Enabled && _client is not null;

    public async Task<AIResponse> GenerateAsync(
        AIRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!IsAvailable)
        {
            throw new InvalidOperationException("OpenAI provider is not available. Check configuration.");
        }

        var stopwatch = Stopwatch.StartNew();

        try
        {
            var chatClient = _client!.GetChatClient(_settings.Model);

            var messages = new List<OpenAI.Chat.ChatMessage>();

            // Add system message if provided
            if (!string.IsNullOrWhiteSpace(request.SystemMessage))
            {
                messages.Add(OpenAI.Chat.ChatMessage.CreateSystemMessage(request.SystemMessage));
            }

            // Add conversation history if provided
            if (request.ConversationHistory is not null)
            {
                foreach (var msg in request.ConversationHistory)
                {
                    messages.Add(msg.Role.ToLowerInvariant() switch
                    {
                        "user" => OpenAI.Chat.ChatMessage.CreateUserMessage(msg.Content),
                        "assistant" => OpenAI.Chat.ChatMessage.CreateAssistantMessage(msg.Content),
                        "system" => OpenAI.Chat.ChatMessage.CreateSystemMessage(msg.Content),
                        _ => OpenAI.Chat.ChatMessage.CreateUserMessage(msg.Content)
                    });
                }
            }

            // Add current user message
            messages.Add(OpenAI.Chat.ChatMessage.CreateUserMessage(request.Prompt));

            var chatOptions = new ChatCompletionOptions
            {
                Temperature = (float)request.Temperature,
                MaxOutputTokenCount = request.MaxTokens
            };

            _logger.LogInformation(
                "Sending request to OpenAI. Model: {Model}, Temperature: {Temperature}, MaxTokens: {MaxTokens}",
                _settings.Model, request.Temperature, request.MaxTokens);

            var completion = await chatClient.CompleteChatAsync(messages, chatOptions, cancellationToken);

            stopwatch.Stop();

            var response = new AIResponse
            {
                Content = completion.Value.Content[0].Text,
                Model = _settings.Model,
                Provider = ProviderName,
                TotalTokens = completion.Value.Usage.TotalTokenCount,
                ResponseTime = stopwatch.Elapsed,
                FromCache = false
            };

            _logger.LogInformation(
                "OpenAI response received. Tokens: {Tokens}, Time: {Time}ms",
                response.TotalTokens, response.ResponseTime.TotalMilliseconds);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling OpenAI API");
            throw new InvalidOperationException($"OpenAI API error: {ex.Message}", ex);
        }
    }
}
