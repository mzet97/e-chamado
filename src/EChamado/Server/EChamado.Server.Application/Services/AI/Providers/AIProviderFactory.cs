using EChamado.Server.Application.Services.AI.Configuration;
using EChamado.Server.Application.Services.AI.Interfaces;
using Microsoft.Extensions.Options;

namespace EChamado.Server.Application.Services.AI.Providers;

/// <summary>
/// Factory for creating and managing AI providers
/// </summary>
public sealed class AIProviderFactory
{
    private readonly IEnumerable<IAIProvider> _providers;
    private readonly AISettings _settings;

    public AIProviderFactory(
        IEnumerable<IAIProvider> providers,
        IOptions<AISettings> options)
    {
        _providers = providers;
        _settings = options.Value;
    }

    /// <summary>
    /// Get the default configured AI provider
    /// </summary>
    public IAIProvider GetDefaultProvider()
    {
        return GetProvider(_settings.DefaultProvider);
    }

    /// <summary>
    /// Get a specific AI provider by name
    /// </summary>
    public IAIProvider GetProvider(string providerName)
    {
        var provider = _providers.FirstOrDefault(p =>
            p.ProviderName.Equals(providerName, StringComparison.OrdinalIgnoreCase));

        if (provider is null)
        {
            throw new InvalidOperationException($"AI provider '{providerName}' not found");
        }

        if (!provider.IsAvailable)
        {
            throw new InvalidOperationException($"AI provider '{providerName}' is not available");
        }

        return provider;
    }

    /// <summary>
    /// Get all available providers
    /// </summary>
    public IEnumerable<IAIProvider> GetAvailableProviders()
    {
        return _providers.Where(p => p.IsAvailable);
    }
}
