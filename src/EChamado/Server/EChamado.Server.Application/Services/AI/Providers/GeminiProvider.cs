using System.Diagnostics;
using EChamado.Server.Application.Services.AI.Configuration;
using EChamado.Server.Application.Services.AI.Interfaces;
using EChamado.Server.Application.Services.AI.Models;
using Mscc.GenerativeAI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EChamado.Server.Application.Services.AI.Providers;

/// <summary>
/// Google Gemini provider implementation
/// </summary>
public sealed class GeminiProvider : IAIProvider
{
    private readonly GeminiSettings _settings;
    private readonly ILogger<GeminiProvider> _logger;
    private readonly GoogleAI? _client;

    public GeminiProvider(
        IOptions<AISettings> options,
        ILogger<GeminiProvider> logger)
    {
        _settings = options.Value.Gemini;
        _logger = logger;

        if (_settings.Enabled && !string.IsNullOrWhiteSpace(_settings.ApiKey))
        {
            try
            {
                _client = new GoogleAI(_settings.ApiKey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize Gemini client");
            }
        }
    }

    public string ProviderName => "Gemini";

    public bool IsAvailable => _settings.Enabled && _client is not null;

    public async Task<AIResponse> GenerateAsync(
        AIRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!IsAvailable)
        {
            throw new InvalidOperationException("Gemini provider is not available. Check configuration.");
        }

        var stopwatch = Stopwatch.StartNew();

        try
        {
            var model = _client!.GenerativeModel(_settings.Model);

            var config = new GenerationConfig
            {
                Temperature = (float?)request.Temperature,
                MaxOutputTokens = request.MaxTokens
            };

            // Build prompt with system message and conversation history
            var fullPrompt = new System.Text.StringBuilder();

            if (!string.IsNullOrWhiteSpace(request.SystemMessage))
            {
                fullPrompt.AppendLine($"SYSTEM: {request.SystemMessage}");
                fullPrompt.AppendLine();
            }

            if (request.ConversationHistory is not null)
            {
                foreach (var msg in request.ConversationHistory)
                {
                    fullPrompt.AppendLine($"{msg.Role.ToUpperInvariant()}: {msg.Content}");
                }
                fullPrompt.AppendLine();
            }

            fullPrompt.AppendLine($"USER: {request.Prompt}");

            _logger.LogInformation(
                "Sending request to Gemini. Model: {Model}, Temperature: {Temperature}, MaxTokens: {MaxTokens}",
                _settings.Model, request.Temperature, request.MaxTokens);

            var response = await model.GenerateContent(
                fullPrompt.ToString(),
                config);

            stopwatch.Stop();

            var aiResponse = new AIResponse
            {
                Content = response.Text ?? string.Empty,
                Model = _settings.Model,
                Provider = ProviderName,
                TotalTokens = response.UsageMetadata?.TotalTokenCount ?? 0,
                ResponseTime = stopwatch.Elapsed,
                FromCache = false
            };

            _logger.LogInformation(
                "Gemini response received. Tokens: {Tokens}, Time: {Time}ms",
                aiResponse.TotalTokens, aiResponse.ResponseTime.TotalMilliseconds);

            return aiResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling Gemini API");
            throw new InvalidOperationException($"Gemini API error: {ex.Message}", ex);
        }
    }
}
