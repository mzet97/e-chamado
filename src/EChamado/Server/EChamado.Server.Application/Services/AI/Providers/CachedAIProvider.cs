using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EChamado.Server.Application.Services.AI.Configuration;
using EChamado.Server.Application.Services.AI.Interfaces;
using EChamado.Server.Application.Services.AI.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EChamado.Server.Application.Services.AI.Providers;

/// <summary>
/// Decorator that adds caching to any AI provider
/// </summary>
public sealed class CachedAIProvider : IAIProvider
{
    private readonly IAIProvider _innerProvider;
    private readonly IMemoryCache _cache;
    private readonly AISettings _settings;
    private readonly ILogger<CachedAIProvider> _logger;

    public CachedAIProvider(
        IAIProvider innerProvider,
        IMemoryCache cache,
        IOptions<AISettings> options,
        ILogger<CachedAIProvider> logger)
    {
        _innerProvider = innerProvider;
        _cache = cache;
        _settings = options.Value;
        _logger = logger;
    }

    public string ProviderName => _innerProvider.ProviderName;

    public bool IsAvailable => _innerProvider.IsAvailable;

    public async Task<AIResponse> GenerateAsync(
        AIRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!_settings.EnableCaching)
        {
            return await _innerProvider.GenerateAsync(request, cancellationToken);
        }

        var cacheKey = GenerateCacheKey(request);

        if (_cache.TryGetValue<AIResponse>(cacheKey, out var cachedResponse) && cachedResponse is not null)
        {
            _logger.LogInformation("Cache hit for AI request. Provider: {Provider}", ProviderName);
            return new AIResponse
            {
                Content = cachedResponse.Content,
                Model = cachedResponse.Model,
                Provider = cachedResponse.Provider,
                TotalTokens = cachedResponse.TotalTokens,
                ResponseTime = cachedResponse.ResponseTime,
                FromCache = true
            };
        }

        _logger.LogInformation("Cache miss for AI request. Provider: {Provider}", ProviderName);

        var response = await _innerProvider.GenerateAsync(request, cancellationToken);

        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_settings.CacheDurationMinutes),
            Size = 1 // For cache size management
        };

        _cache.Set(cacheKey, response, cacheOptions);

        return response;
    }

    private string GenerateCacheKey(AIRequest request)
    {
        var requestData = new
        {
            Provider = ProviderName,
            request.Prompt,
            request.SystemMessage,
            request.Temperature,
            request.MaxTokens,
            ConversationHistory = request.ConversationHistory
        };

        var json = JsonSerializer.Serialize(requestData);
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(json));
        return $"ai_cache_{Convert.ToHexString(hash)}";
    }
}
