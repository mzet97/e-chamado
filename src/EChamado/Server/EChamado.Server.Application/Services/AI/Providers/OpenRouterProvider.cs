using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using EChamado.Server.Application.Services.AI.Configuration;
using EChamado.Server.Application.Services.AI.Interfaces;
using EChamado.Server.Application.Services.AI.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EChamado.Server.Application.Services.AI.Providers;

/// <summary>
/// OpenRouter provider implementation (access to multiple AI models)
/// </summary>
public sealed class OpenRouterProvider : IAIProvider
{
    private readonly OpenRouterSettings _settings;
    private readonly ILogger<OpenRouterProvider> _logger;
    private readonly HttpClient _httpClient;

    public OpenRouterProvider(
        IOptions<AISettings> options,
        ILogger<OpenRouterProvider> logger,
        IHttpClientFactory httpClientFactory)
    {
        _settings = options.Value.OpenRouter;
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient("OpenRouter");

        if (_settings.Enabled && !string.IsNullOrWhiteSpace(_settings.ApiKey))
        {
            // Trailing slash é OBRIGATÓRIO para que o HttpClient resolva paths relativos corretamente
            // Sem trailing slash: "https://openrouter.ai/api/v1" + "chat/completions" = "https://openrouter.ai/api/chat/completions" (ERRADO)
            // Com trailing slash: "https://openrouter.ai/api/v1/" + "chat/completions" = "https://openrouter.ai/api/v1/chat/completions" (CORRETO)
            var endpoint = _settings.Endpoint.TrimEnd('/') + "/";
            _httpClient.BaseAddress = new Uri(endpoint);
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_settings.ApiKey}");
            _httpClient.DefaultRequestHeaders.Add("HTTP-Referer", "https://echamado.com");
            _httpClient.DefaultRequestHeaders.Add("X-Title", "EChamado");
        }
    }

    public string ProviderName => "OpenRouter";

    public bool IsAvailable => _settings.Enabled && !string.IsNullOrWhiteSpace(_settings.ApiKey);

    public async Task<AIResponse> GenerateAsync(
        AIRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!IsAvailable)
        {
            throw new InvalidOperationException("OpenRouter provider is not available. Check configuration.");
        }

        var stopwatch = Stopwatch.StartNew();

        try
        {
            var messages = new List<object>();

            // Add system message if provided
            if (!string.IsNullOrWhiteSpace(request.SystemMessage))
            {
                messages.Add(new { role = "system", content = request.SystemMessage });
            }

            // Add conversation history if provided
            if (request.ConversationHistory is not null)
            {
                foreach (var msg in request.ConversationHistory)
                {
                    messages.Add(new { role = msg.Role.ToLowerInvariant(), content = msg.Content });
                }
            }

            // Add current user message
            messages.Add(new { role = "user", content = request.Prompt });

            var requestBody = new
            {
                model = _settings.Model,
                messages,
                temperature = request.Temperature,
                max_tokens = request.MaxTokens
            };

            _logger.LogInformation(
                "Sending request to OpenRouter. Model: {Model}, Temperature: {Temperature}, MaxTokens: {MaxTokens}",
                _settings.Model, request.Temperature, request.MaxTokens);

            // IMPORTANTE: usar caminho relativo SEM barra inicial para que o BaseAddress
            // seja respeitado. O BaseAddress já tem trailing slash no endpoint.
            var response = await _httpClient.PostAsJsonAsync(
                "chat/completions",
                requestBody,
                cancellationToken);

            response.EnsureSuccessStatusCode();

            var rawResponse = await response.Content.ReadAsStringAsync(cancellationToken);

            // OpenRouter API retorna JSON com lowercase (camelCase)
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var responseContent = JsonSerializer.Deserialize<OpenRouterResponse>(rawResponse, jsonOptions);

            if (responseContent is null || responseContent.Choices is null || responseContent.Choices.Length == 0)
            {
                throw new InvalidOperationException("Invalid response from OpenRouter");
            }

            stopwatch.Stop();

            // DeepSeek e modelos de raciocínio retornam o texto no campo "reasoning"
            // em vez de "content". Usamos content primeiro, fallback para reasoning.
            var message = responseContent.Choices[0].Message;
            var content = message.Content ?? message.Reasoning
                ?? throw new InvalidOperationException("OpenRouter response has no content or reasoning");

            var aiResponse = new AIResponse
            {
                Content = content,
                Model = _settings.Model,
                Provider = ProviderName,
                TotalTokens = responseContent.Usage?.TotalTokens ?? 0,
                ResponseTime = stopwatch.Elapsed,
                FromCache = false
            };

            _logger.LogInformation(
                "OpenRouter response received. Tokens: {Tokens}, Time: {Time}ms",
                aiResponse.TotalTokens, aiResponse.ResponseTime.TotalMilliseconds);

            return aiResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling OpenRouter API");
            throw new InvalidOperationException($"OpenRouter API error: {ex.Message}", ex);
        }
    }

    // DTOs for OpenRouter API
    private sealed record OpenRouterResponse(
        Choice[] Choices,
        Usage? Usage);

    private sealed record Choice(
        Message Message);

    private sealed record Message(
        string? Content,
        string? Reasoning);  // DeepSeek e outros modelos de raciocínio retornam aqui

    private sealed record Usage(
        [property: JsonPropertyName("total_tokens")] int TotalTokens);
}
