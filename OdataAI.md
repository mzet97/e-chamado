# 🤖 Prompt Engineering: Natural Language to OData Query Converter

Como engenheiro de prompt, vou criar uma solução completa para converter linguagem natural em queries OData usando múltiplos provedores de AI. 

---

## 📋 **Especificação Técnica**

### **Requisitos:**
1. ✅ Endpoint REST para conversão NL → OData
2. ✅ Suporte multi-provider (Gemini, OpenAI, OpenRouter)
3. ✅ Prompt engineering otimizado para queries OData
4. ✅ Validação e sanitização de queries geradas
5. ✅ Cache de conversões frequentes
6. ✅ Logging e observabilidade

---

## 🏗️ **Arquitetura da Solução**

```
┌─────────────────┐
│  User Request   │ "Me mostre produtos com preço acima de 100 reais"
└────────┬────────┘
         │
         ▼
┌─────────────────────────┐
│  NLQueryController      │ POST /api/nlquery/odata
└────────┬────────────────┘
         │
         ▼
┌─────────────────────────┐
│  NLToODataService       │ Orquestra conversão
└────────┬────────────────┘
         │
         ├─────► ┌──────────────────┐
         │       │ OpenAI Provider  │
         │       └──────────────────┘
         │
         ├─────► ┌──────────────────┐
         │       │ Gemini Provider  │
         │       └──────────────────┘
         │
         └─────► ┌──────────────────┐
                 │ OpenRouter Prov.  │
                 └──────────────────┘
         │
         ▼
┌─────────────────────────┐
│  OData Query Result     │ $filter=Price gt 100
└─────────────────────────┘
```

---

## 📦 **Passo 1: Instalar Dependências**

```bash
# OpenAI SDK
dotnet add package Azure.AI.OpenAI --version 1.0.0-beta.12

# Google Generative AI (Gemini)
dotnet add package Google.Ai.Generativelanguage --version 1.0.0

# HTTP Client for OpenRouter
# Já incluído no . NET

# Cache
dotnet add package Microsoft.Extensions. Caching.Memory

# JSON
dotnet add package System.Text.Json
```

---

## 🔧 **Passo 2: Configuração (appsettings.json)**

````json name=appsettings.json
{
  "AI": {
    "DefaultProvider": "OpenAI",
    "Providers": {
      "OpenAI": {
        "Enabled": true,
        "ApiKey": "sk-your-openai-key",
        "Model": "gpt-4-turbo-preview",
        "Endpoint": "https://api.openai.com/v1",
        "MaxTokens": 500,
        "Temperature": 0. 1
      },
      "Gemini": {
        "Enabled": true,
        "ApiKey": "your-gemini-key",
        "Model": "gemini-1.5-pro",
        "MaxTokens": 500,
        "Temperature": 0.1
      },
      "OpenRouter": {
        "Enabled": true,
        "ApiKey": "your-openrouter-key",
        "Model": "openai/gpt-4-turbo-preview",
        "Endpoint": "https://openrouter.ai/api/v1",
        "MaxTokens": 500,
        "Temperature": 0. 1
      }
    },
    "Cache": {
      "Enabled": true,
      "ExpirationMinutes": 60
    },
    "ODataContext": {
      "EntitySets": [
        {
          "Name": "Products",
          "Properties": [
            { "Name": "Id", "Type": "Guid" },
            { "Name": "Name", "Type": "string" },
            { "Name": "Description", "Type": "string" },
            { "Name": "Price", "Type": "decimal" },
            { "Name": "Weight", "Type": "decimal" },
            { "Name": "Height", "Type": "decimal" },
            { "Name": "Length", "Type": "decimal" },
            { "Name": "IdCategory", "Type": "Guid" },
            { "Name": "IdCompany", "Type": "Guid" },
            { "Name": "CreatedAt", "Type": "DateTime" },
            { "Name": "IsDeleted", "Type": "bool" }
          ],
          "NavigationProperties": [
            { "Name": "Category", "Type": "Category" },
            { "Name": "Company", "Type": "Company" }
          ]
        },
        {
          "Name": "Categories",
          "Properties": [
            { "Name": "Id", "Type": "Guid" },
            { "Name": "Name", "Type": "string" },
            { "Name": "Description", "Type": "string" }
          ]
        },
        {
          "Name": "Companies",
          "Properties": [
            { "Name": "Id", "Type": "Guid" },
            { "Name": "Name", "Type": "string" },
            { "Name": "DocId", "Type": "string" },
            { "Name": "Email", "Type": "string" }
          ]
        }
      ]
    }
  }
}
````

---

## 🎯 **Passo 3: Prompt Engineering - Sistema Base**

````csharp name=ODataPromptTemplates.cs
namespace e_Estoque_API.Application.AI.Prompts;

public static class ODataPromptTemplates
{
    /// <summary>
    /// Prompt principal otimizado para conversão NL → OData
    /// </summary>
    public const string SYSTEM_PROMPT = @"
Você é um especialista em OData Query Language e Entity Data Model (EDM).
Sua única função é converter perguntas em linguagem natural para queries OData válidas e otimizadas.

## CONTEXTO DO SISTEMA
Você está trabalhando com uma API de gerenciamento de estoque (e-Estoque-API) que expõe os seguintes EntitySets:

{ENTITY_SETS_SCHEMA}

## REGRAS OBRIGATÓRIAS

1. **FORMATO DE SAÍDA**: Retorne APENAS a query OData, sem explicações, sem markdown, sem aspas.
   ✅ CORRETO: $filter=Price gt 100&$orderby=Name
   ❌ ERRADO: ```odata\n$filter=Price gt 100\n```

2. **OPERADORES ODATA**:
   - Comparação: eq, ne, gt, ge, lt, le
   - Lógicos: and, or, not
   - String: contains, startswith, endswith, length, tolower, toupper
   - Data: year, month, day, hour, minute, second
   - Navegação: /PropertyName

3. **SINTAXE**:
   - Propriedades: CamelCase exatamente como no schema
   - Strings: 'valor entre aspas simples'
   - Números: sem aspas
   - Datas: yyyy-MM-ddTHH:mm:ssZ
   - Booleanos: true ou false (minúsculo)
   - Guid: guid'00000000-0000-0000-0000-000000000000'

4. **OPERAÇÕES COMUNS**:
   - Filtrar: $filter=PropertyName operator value
   - Ordenar: $orderby=PropertyName [asc|desc]
   - Limitar: $top=N
   - Pular: $skip=N
   - Selecionar: $select=Prop1,Prop2
   - Expandir: $expand=NavigationProperty
   - Contar: $count=true

5. **OTIMIZAÇÕES**:
   - Use $select para limitar campos retornados
   - Use $top para limitar resultados
   - Combine filtros com 'and' ao invés de múltiplos $filter
   - Use índices: filtre por Id, CreatedAt quando possível

6. **VALIDAÇÕES**:
   - Verifique se a propriedade existe no schema
   - Use o tipo correto (decimal para Price, não int)
   - Não invente propriedades
   - Retorne erro se não entender a pergunta

## EXEMPLOS DE CONVERSÃO

### Exemplo 1: Filtro Simples
Pergunta: ""Me mostre produtos com preço acima de 100 reais""
Resposta: $filter=Price gt 100

### Exemplo 2: Filtro Composto
Pergunta: ""Produtos baratos e leves""
Resposta: $filter=Price lt 50 and Weight lt 1

### Exemplo 3: Busca por Nome
Pergunta: ""Buscar produtos que contenham Samsung no nome""
Resposta: $filter=contains(Name,'Samsung')

### Exemplo 4: Ordenação
Pergunta: ""Lista de produtos ordenados por preço decrescente""
Resposta: $orderby=Price desc

### Exemplo 5: Paginação
Pergunta: ""Primeiros 10 produtos""
Resposta: $top=10

### Exemplo 6: Combinado
Pergunta: ""Top 5 produtos mais caros da categoria eletrônicos""
Resposta: $filter=Category/Name eq 'Eletrônicos'&$orderby=Price desc&$top=5&$expand=Category

### Exemplo 7: Data
Pergunta: ""Produtos criados em 2024""
Resposta: $filter=year(CreatedAt) eq 2024

### Exemplo 8: Navegação
Pergunta: ""Produtos da empresa ABC""
Resposta: $filter=Company/Name eq 'ABC'&$expand=Company

### Exemplo 9: Múltiplos Filtros
Pergunta: ""Produtos não deletados com preço entre 50 e 200""
Resposta: $filter=IsDeleted eq false and Price ge 50 and Price le 200

### Exemplo 10: Contagem
Pergunta: ""Quantos produtos existem? ""
Resposta: $count=true

## TRATAMENTO DE ERROS

Se a pergunta for ambígua ou impossível de converter:
❌ NÃO retorne uma query genérica
✅ RETORNE: ERROR: [explicação clara do problema]

Exemplo de erro:
Pergunta: ""Me mostre os melhores produtos""
Resposta: ERROR: Não está claro o critério de 'melhor'.  Por favor especifique: mais caro?  mais vendido? melhor avaliado?

## INSTRUÇÕES FINAIS

- Priorize simplicidade e performance
- Use apenas propriedades documentadas no schema
- Retorne queries executáveis diretamente
- Não adicione comentários na query
- Mantenha case-sensitive exato do schema
";

    public const string USER_PROMPT_TEMPLATE = @"
Converta a seguinte pergunta em uma query OData válida:

Pergunta: {USER_QUESTION}

Lembre-se:
- Retorne APENAS a query OData
- Sem explicações, sem markdown, sem formatação extra
- Use propriedades exatas do schema fornecido
- Se houver ambiguidade, retorne ERROR: com explicação
";

    /// <summary>
    /// Gera o schema dos EntitySets para contexto da AI
    /// </summary>
    public static string GenerateEntitySetsSchema(List<EntitySetMetadata> entitySets)
    {
        var schema = new StringBuilder();
        
        foreach (var entitySet in entitySets)
        {
            schema. AppendLine($"### EntitySet: {entitySet.Name}");
            schema.AppendLine("**Propriedades:**");
            
            foreach (var prop in entitySet.Properties)
            {
                schema.AppendLine($"  - {prop.Name} ({prop.Type})");
            }
            
            if (entitySet.NavigationProperties?. Any() == true)
            {
                schema.AppendLine("**Navegação:**");
                foreach (var nav in entitySet.NavigationProperties)
                {
                    schema.AppendLine($"  - {nav.Name} → {nav.Type}");
                }
            }
            
            schema.AppendLine();
        }
        
        return schema. ToString();
    }
}

public class EntitySetMetadata
{
    public string Name { get; set; } = string.Empty;
    public List<PropertyMetadata> Properties { get; set; } = new();
    public List<NavigationPropertyMetadata>? NavigationProperties { get; set; }
}

public class PropertyMetadata
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
}

public class NavigationPropertyMetadata
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
}
````

---

## 🔌 **Passo 4: Abstrações e Interfaces**

````csharp name=IAIProvider.cs
namespace e_Estoque_API.Application.AI. Interfaces;

/// <summary>
/// Interface abstrata para provedores de AI
/// </summary>
public interface IAIProvider
{
    /// <summary>
    /// Nome do provedor (OpenAI, Gemini, OpenRouter)
    /// </summary>
    string ProviderName { get; }
    
    /// <summary>
    /// Indica se o provedor está habilitado
    /// </summary>
    bool IsEnabled { get; }
    
    /// <summary>
    /// Gera resposta usando o modelo de AI
    /// </summary>
    Task<AIResponse> GenerateAsync(
        string systemPrompt, 
        string userPrompt, 
        CancellationToken cancellationToken = default);
}

public class AIResponse
{
    public bool Success { get; set; }
    public string? Content { get; set; }
    public string? ErrorMessage { get; set; }
    public AIUsageMetrics?  Usage { get; set; }
    public TimeSpan Elapsed { get; set; }
}

public class AIUsageMetrics
{
    public int PromptTokens { get; set; }
    public int CompletionTokens { get; set; }
    public int TotalTokens { get; set; }
}
````

---

## 🤖 **Passo 5: Implementação dos Providers**

### **5.1 OpenAI Provider**

````csharp name=OpenAIProvider.cs
using Azure.AI.OpenAI;
using System. Diagnostics;

namespace e_Estoque_API.Infrastructure.AI. Providers;

public class OpenAIProvider : IAIProvider
{
    private readonly OpenAIClient _client;
    private readonly string _model;
    private readonly int _maxTokens;
    private readonly float _temperature;
    private readonly ILogger<OpenAIProvider> _logger;
    private readonly bool _isEnabled;

    public string ProviderName => "OpenAI";
    public bool IsEnabled => _isEnabled;

    public OpenAIProvider(
        IConfiguration configuration,
        ILogger<OpenAIProvider> logger)
    {
        _logger = logger;
        
        var config = configuration.GetSection("AI:Providers:OpenAI");
        _isEnabled = config.GetValue<bool>("Enabled");
        
        if (!_isEnabled)
        {
            _logger.LogWarning("OpenAI provider está desabilitado");
            return;
        }

        var apiKey = config.GetValue<string>("ApiKey") 
            ?? throw new InvalidOperationException("OpenAI ApiKey não configurada");
        
        _model = config.GetValue<string>("Model") ??  "gpt-4-turbo-preview";
        _maxTokens = config.GetValue<int>("MaxTokens", 500);
        _temperature = config.GetValue<float>("Temperature", 0.1f);

        _client = new OpenAIClient(apiKey);
    }

    public async Task<AIResponse> GenerateAsync(
        string systemPrompt, 
        string userPrompt, 
        CancellationToken cancellationToken = default)
    {
        if (! _isEnabled)
        {
            return new AIResponse
            {
                Success = false,
                ErrorMessage = "OpenAI provider está desabilitado"
            };
        }

        var stopwatch = Stopwatch.StartNew();

        try
        {
            var chatCompletionsOptions = new ChatCompletionsOptions
            {
                DeploymentName = _model,
                Messages =
                {
                    new ChatRequestSystemMessage(systemPrompt),
                    new ChatRequestUserMessage(userPrompt)
                },
                MaxTokens = _maxTokens,
                Temperature = _temperature,
                NucleusSamplingFactor = 1f,
                FrequencyPenalty = 0f,
                PresencePenalty = 0f
            };

            var response = await _client.GetChatCompletionsAsync(
                chatCompletionsOptions, 
                cancellationToken);

            stopwatch.Stop();

            var choice = response.Value.Choices. FirstOrDefault();
            if (choice == null)
            {
                return new AIResponse
                {
                    Success = false,
                    ErrorMessage = "Nenhuma resposta retornada pela API",
                    Elapsed = stopwatch.Elapsed
                };
            }

            var content = choice. Message.Content. Trim();

            _logger.LogInformation(
                "OpenAI response: {Tokens} tokens em {Elapsed}ms",
                response.Value.Usage.TotalTokens,
                stopwatch.ElapsedMilliseconds);

            return new AIResponse
            {
                Success = true,
                Content = content,
                Usage = new AIUsageMetrics
                {
                    PromptTokens = response.Value.Usage. PromptTokens,
                    CompletionTokens = response. Value.Usage.CompletionTokens,
                    TotalTokens = response.Value.Usage. TotalTokens
                },
                Elapsed = stopwatch. Elapsed
            };
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            
            _logger.LogError(ex, "Erro ao chamar OpenAI API");
            
            return new AIResponse
            {
                Success = false,
                ErrorMessage = ex.Message,
                Elapsed = stopwatch.Elapsed
            };
        }
    }
}
````

### **5.2 Gemini Provider**

````csharp name=GeminiProvider.cs
using System.Diagnostics;
using System.Net.Http. Json;
using System.Text.Json;

namespace e_Estoque_API.Infrastructure.AI. Providers;

public class GeminiProvider : IAIProvider
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _model;
    private readonly int _maxTokens;
    private readonly float _temperature;
    private readonly ILogger<GeminiProvider> _logger;
    private readonly bool _isEnabled;

    public string ProviderName => "Gemini";
    public bool IsEnabled => _isEnabled;

    public GeminiProvider(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<GeminiProvider> logger)
    {
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient("Gemini");
        
        var config = configuration.GetSection("AI:Providers:Gemini");
        _isEnabled = config.GetValue<bool>("Enabled");
        
        if (!_isEnabled)
        {
            _logger.LogWarning("Gemini provider está desabilitado");
            return;
        }

        _apiKey = config. GetValue<string>("ApiKey") 
            ?? throw new InvalidOperationException("Gemini ApiKey não configurada");
        
        _model = config.GetValue<string>("Model") ?? "gemini-1.5-pro";
        _maxTokens = config.GetValue<int>("MaxTokens", 500);
        _temperature = config. GetValue<float>("Temperature", 0.1f);
    }

    public async Task<AIResponse> GenerateAsync(
        string systemPrompt, 
        string userPrompt, 
        CancellationToken cancellationToken = default)
    {
        if (!_isEnabled)
        {
            return new AIResponse
            {
                Success = false,
                ErrorMessage = "Gemini provider está desabilitado"
            };
        }

        var stopwatch = Stopwatch.StartNew();

        try
        {
            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        role = "user",
                        parts = new[]
                        {
                            new { text = $"{systemPrompt}\n\n{userPrompt}" }
                        }
                    }
                },
                generationConfig = new
                {
                    temperature = _temperature,
                    maxOutputTokens = _maxTokens,
                    topP = 1. 0,
                    topK = 1
                }
            };

            var url = $"https://generativelanguage.googleapis.com/v1beta/models/{_model}:generateContent? key={_apiKey}";

            var response = await _httpClient.PostAsJsonAsync(url, requestBody, cancellationToken);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadFromJsonAsync<JsonDocument>(cancellationToken: cancellationToken);
            
            stopwatch.Stop();

            var text = responseContent?.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                . GetProperty("text")
                . GetString()?. Trim();

            if (string.IsNullOrEmpty(text))
            {
                return new AIResponse
                {
                    Success = false,
                    ErrorMessage = "Resposta vazia do Gemini",
                    Elapsed = stopwatch.Elapsed
                };
            }

            _logger.LogInformation(
                "Gemini response em {Elapsed}ms",
                stopwatch. ElapsedMilliseconds);

            return new AIResponse
            {
                Success = true,
                Content = text,
                Elapsed = stopwatch.Elapsed
            };
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            
            _logger.LogError(ex, "Erro ao chamar Gemini API");
            
            return new AIResponse
            {
                Success = false,
                ErrorMessage = ex.Message,
                Elapsed = stopwatch.Elapsed
            };
        }
    }
}
````

### **5.3 OpenRouter Provider**

````csharp name=OpenRouterProvider.cs
using System.Diagnostics;
using System.Net. Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace e_Estoque_API.Infrastructure.AI.Providers;

public class OpenRouterProvider : IAIProvider
{
    private readonly HttpClient _httpClient;
    private readonly string _model;
    private readonly int _maxTokens;
    private readonly float _temperature;
    private readonly ILogger<OpenRouterProvider> _logger;
    private readonly bool _isEnabled;

    public string ProviderName => "OpenRouter";
    public bool IsEnabled => _isEnabled;

    public OpenRouterProvider(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<OpenRouterProvider> logger)
    {
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient("OpenRouter");
        
        var config = configuration.GetSection("AI:Providers:OpenRouter");
        _isEnabled = config.GetValue<bool>("Enabled");
        
        if (!_isEnabled)
        {
            _logger.LogWarning("OpenRouter provider está desabilitado");
            return;
        }

        var apiKey = config.GetValue<string>("ApiKey") 
            ?? throw new InvalidOperationException("OpenRouter ApiKey não configurada");
        
        _model = config.GetValue<string>("Model") ?? "openai/gpt-4-turbo-preview";
        _maxTokens = config.GetValue<int>("MaxTokens", 500);
        _temperature = config.GetValue<float>("Temperature", 0. 1f);

        _httpClient.BaseAddress = new Uri("https://openrouter.ai/api/v1/");
        _httpClient.DefaultRequestHeaders. Authorization = 
            new AuthenticationHeaderValue("Bearer", apiKey);
        _httpClient.DefaultRequestHeaders.Add("HTTP-Referer", "https://github.com/mzet97/e-Estoque-API");
        _httpClient.DefaultRequestHeaders. Add("X-Title", "e-Estoque-API NL to OData");
    }

    public async Task<AIResponse> GenerateAsync(
        string systemPrompt, 
        string userPrompt, 
        CancellationToken cancellationToken = default)
    {
        if (!_isEnabled)
        {
            return new AIResponse
            {
                Success = false,
                ErrorMessage = "OpenRouter provider está desabilitado"
            };
        }

        var stopwatch = Stopwatch.StartNew();

        try
        {
            var requestBody = new
            {
                model = _model,
                messages = new[]
                {
                    new { role = "system", content = systemPrompt },
                    new { role = "user", content = userPrompt }
                },
                max_tokens = _maxTokens,
                temperature = _temperature
            };

            var response = await _httpClient.PostAsJsonAsync("chat/completions", requestBody, cancellationToken);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadFromJsonAsync<JsonDocument>(cancellationToken: cancellationToken);
            
            stopwatch. Stop();

            var text = responseContent?.RootElement
                . GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString()?.Trim();

            if (string.IsNullOrEmpty(text))
            {
                return new AIResponse
                {
                    Success = false,
                    ErrorMessage = "Resposta vazia do OpenRouter",
                    Elapsed = stopwatch.Elapsed
                };
            }

            var usage = responseContent?.RootElement. GetProperty("usage");

            _logger.LogInformation(
                "OpenRouter response: {Tokens} tokens em {Elapsed}ms",
                usage?. GetProperty("total_tokens").GetInt32() ?? 0,
                stopwatch.ElapsedMilliseconds);

            return new AIResponse
            {
                Success = true,
                Content = text,
                Usage = new AIUsageMetrics
                {
                    PromptTokens = usage?.GetProperty("prompt_tokens").GetInt32() ?? 0,
                    CompletionTokens = usage?.GetProperty("completion_tokens").GetInt32() ?? 0,
                    TotalTokens = usage?. GetProperty("total_tokens").GetInt32() ?? 0
                },
                Elapsed = stopwatch.Elapsed
            };
        }
        catch (Exception ex)
        {
            stopwatch. Stop();
            
            _logger.LogError(ex, "Erro ao chamar OpenRouter API");
            
            return new AIResponse
            {
                Success = false,
                ErrorMessage = ex. Message,
                Elapsed = stopwatch.Elapsed
            };
        }
    }
}
````

---

## 🎯 **Passo 6: Serviço Principal de Conversão**

````csharp name=NLToODataService.cs
using Microsoft.Extensions. Caching.Memory;
using System.Text.RegularExpressions;

namespace e_Estoque_API.Application.AI.Services;

public class NLToODataService
{
    private readonly IEnumerable<IAIProvider> _aiProviders;
    private readonly IMemoryCache _cache;
    private readonly IConfiguration _configuration;
    private readonly ILogger<NLToODataService> _logger;
    private readonly string _defaultProvider;
    private readonly bool _cacheEnabled;
    private readonly int _cacheExpirationMinutes;

    public NLToODataService(
        IEnumerable<IAIProvider> aiProviders,
        IMemoryCache cache,
        IConfiguration configuration,
        ILogger<NLToODataService> logger)
    {
        _aiProviders = aiProviders;
        _cache = cache;
        _configuration = configuration;
        _logger = logger;
        
        _defaultProvider = configuration.GetValue<string>("AI:DefaultProvider") ?? "OpenAI";
        _cacheEnabled = configuration.GetValue<bool>("AI:Cache:Enabled", true);
        _cacheExpirationMinutes = configuration.GetValue<int>("AI:Cache:ExpirationMinutes", 60);
    }

    /// <summary>
    /// Converte linguagem natural para query OData
    /// </summary>
    public async Task<NLToODataResult> ConvertAsync(
        string naturalLanguageQuery,
        string?  providerName = null,
        string? entitySet = null,
        CancellationToken cancellationToken = default)
    {
        // Validação de entrada
        if (string. IsNullOrWhiteSpace(naturalLanguageQuery))
        {
            return NLToODataResult.Error("Query em linguagem natural não pode ser vazia");
        }

        // Cache
        var cacheKey = $"nl2odata:{providerName ??  _defaultProvider}:{naturalLanguageQuery. ToLowerInvariant()}";
        
        if (_cacheEnabled && _cache.TryGetValue<NLToODataResult>(cacheKey, out var cachedResult))
        {
            _logger.LogInformation("Cache hit para query: {Query}", naturalLanguageQuery);
            cachedResult! .FromCache = true;
            return cachedResult;
        }

        // Seleciona provider
        var provider = SelectProvider(providerName);
        if (provider == null)
        {
            return NLToODataResult.Error($"Provider '{providerName ??  _defaultProvider}' não disponível");
        }

        try
        {
            // Carrega schema dos EntitySets
            var entitySets = LoadEntitySetsMetadata();
            var schemaText = ODataPromptTemplates.GenerateEntitySetsSchema(entitySets);

            // Gera prompts
            var systemPrompt = ODataPromptTemplates. SYSTEM_PROMPT
                .Replace("{ENTITY_SETS_SCHEMA}", schemaText);

            var userPrompt = ODataPromptTemplates.USER_PROMPT_TEMPLATE
                .Replace("{USER_QUESTION}", naturalLanguageQuery);

            // Chama AI
            var aiResponse = await provider.GenerateAsync(systemPrompt, userPrompt, cancellationToken);

            if (! aiResponse.Success || string.IsNullOrEmpty(aiResponse.Content))
            {
                return NLToODataResult.Error(
                    aiResponse.ErrorMessage ?? "Falha ao gerar query OData",
                    provider.ProviderName);
            }

            // Processa resposta
            var odataQuery = CleanODataQuery(aiResponse. Content);

            // Valida query gerada
            var validationResult = ValidateODataQuery(odataQuery);
            if (!validationResult.IsValid)
            {
                return NLToODataResult.Error(
                    $"Query OData inválida: {validationResult. ErrorMessage}",
                    provider.ProviderName);
            }

            var result = NLToODataResult.Success(
                originalQuery: naturalLanguageQuery,
                odataQuery: odataQuery,
                provider: provider.ProviderName,
                elapsed: aiResponse.Elapsed,
                usage: aiResponse.Usage,
                entitySet: DetermineEntitySet(odataQuery, entitySet));

            // Cache
            if (_cacheEnabled)
            {
                _cache.Set(cacheKey, result, TimeSpan.FromMinutes(_cacheExpirationMinutes));
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao converter NL para OData");
            return NLToODataResult.Error($"Erro interno: {ex.Message}", provider.ProviderName);
        }
    }

    /// <summary>
    /// Seleciona o provider de AI
    /// </summary>
    private IAIProvider?  SelectProvider(string?  providerName)
    {
        var name = providerName ?? _defaultProvider;
        
        var provider = _aiProviders.FirstOrDefault(p => 
            p.ProviderName.Equals(name, StringComparison.OrdinalIgnoreCase) && p.IsEnabled);

        if (provider == null)
        {
            _logger.LogWarning("Provider {Provider} não encontrado ou desabilitado", name);
        }

        return provider;
    }

    /// <summary>
    /// Limpa a query OData removendo markdown e formatação extra
    /// </summary>
    private string CleanODataQuery(string rawQuery)
    {
        // Remove markdown code blocks
        rawQuery = Regex.Replace(rawQuery, @"```(? :odata|sql|plaintext)?\s*", "", RegexOptions.IgnoreCase);
        rawQuery = Regex.Replace(rawQuery, @"```\s*$", "");
        
        // Remove aspas ao redor
        rawQuery = rawQuery. Trim('\"', '\'', '`');
        
        // Remove quebras de linha e espaços extras
        rawQuery = Regex. Replace(rawQuery, @"\s+", " ");
        
        return rawQuery. Trim();
    }

    /// <summary>
    /// Valida básica de sintaxe OData
    /// </summary>
    private (bool IsValid, string?  ErrorMessage) ValidateODataQuery(string odataQuery)
    {
        // Verifica se começa com ERROR:
        if (odataQuery.StartsWith("ERROR:", StringComparison.OrdinalIgnoreCase))
        {
            return (false, odataQuery. Substring(6).Trim());
        }

        // Verifica se tem sintaxe básica OData
        if (!odataQuery.StartsWith("$") && !odataQuery.Contains("$"))
        {
            return (false, "Query não parece ser OData válida (falta operadores $)");
        }

        // Validações de segurança
        var dangerousPatterns = new[]
        {
            "javascript:",
            "<script",
            "eval(",
            "exec("
        };

        foreach (var pattern in dangerousPatterns)
        {
            if (odataQuery.Contains(pattern, StringComparison.OrdinalIgnoreCase))
            {
                return (false, $"Padrão perigoso detectado: {pattern}");
            }
        }

        return (true, null);
    }

    /// <summary>
    /// Determina o EntitySet baseado na query
    /// </summary>
    private string?  DetermineEntitySet(string odataQuery, string? providedEntitySet)
    {
        if (! string.IsNullOrEmpty(providedEntitySet))
        {
            return providedEntitySet;
        }

        // Tenta inferir do filtro
        var entitySets = new[] { "Products", "Categories", "Companies", "Customers", "Sales", "Inventories", "Taxs" };
        
        foreach (var entitySet in entitySets)
        {
            if (odataQuery.Contains(entitySet, StringComparison. OrdinalIgnoreCase))
            {
                return entitySet;
            }
        }

        return null;
    }

    /// <summary>
    /// Carrega metadados dos EntitySets da configuração
    /// </summary>
    private List<EntitySetMetadata> LoadEntitySetsMetadata()
    {
        var entitySets = _configuration
            .GetSection("AI:ODataContext:EntitySets")
            . Get<List<EntitySetMetadata>>() ?? new List<EntitySetMetadata>();

        return entitySets;
    }
}

/// <summary>
/// Resultado da conversão NL → OData
/// </summary>
public class NLToODataResult
{
    public bool Success { get; set; }
    public string? OriginalQuery { get; set; }
    public string? ODataQuery { get; set; }
    public string? EntitySet { get; set; }
    public string? ErrorMessage { get; set; }
    public string? Provider { get; set; }
    public TimeSpan Elapsed { get; set; }
    public AIUsageMetrics?  Usage { get; set; }
    public bool FromCache { get; set; }

    public static NLToODataResult Success(
        string originalQuery,
        string odataQuery,
        string provider,
        TimeSpan elapsed,
        AIUsageMetrics?  usage = null,
        string?  entitySet = null)
    {
        return new NLToODataResult
        {
            Success = true,
            OriginalQuery = originalQuery,
            ODataQuery = odataQuery,
            EntitySet = entitySet,
            Provider = provider,
            Elapsed = elapsed,
            Usage = usage
        };
    }

    public static NLToODataResult Error(string errorMessage, string?  provider = null)
    {
        return new NLToODataResult
        {
            Success = false,
            ErrorMessage = errorMessage,
            Provider = provider
        };
    }
}
````

---

## 🎮 **Passo 7: Controller**

````csharp name=NLQueryController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace e_Estoque_API.API.Controllers;

/// <summary>
/// Controller para conversão de linguagem natural para queries OData
/// </summary>
[Authorize]
[Route("api/nlquery")]
[ApiController]
public class NLQueryController : MainController
{
    private readonly NLToODataService _nlToODataService;
    private readonly ILogger<NLQueryController> _logger;

    public NLQueryController(
        NLToODataService nlToODataService,
        ILogger<NLQueryController> logger)
    {
        _nlToODataService = nlToODataService;
        _logger = logger;
    }

    /// <summary>
    /// Converte linguagem natural para query OData
    /// </summary>
    /// <param name="request">Requisição com query em linguagem natural</param>
    /// <returns>Query OData correspondente</returns>
    [HttpPost("odata")]
    [ProducesResponseType(typeof(NLToODataResponse), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IActionResult> ConvertToOData([FromBody] NLQueryRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Query))
        {
            return BadRequest(new { error = "Query não pode ser vazia" });
        }

        _logger.LogInformation(
            "Conversão NL→OData: '{Query}' usando provider '{Provider}'",
            request.Query,
            request.Provider ??  "default");

        var result = await _nlToODataService.ConvertAsync(
            request.Query,
            request. Provider,
            request.EntitySet);

        if (! result.Success)
        {
            _logger.LogWarning(
                "Falha na conversão: {Error}",
                result.ErrorMessage);

            return CustomResponse(false, new NLToODataResponse
            {
                Success = false,
                ErrorMessage = result.ErrorMessage,
                Provider = result.Provider
            });
        }

        _logger.LogInformation(
            "Conversão bem-sucedida: '{ODataQuery}' (Provider: {Provider}, Elapsed: {Elapsed}ms, Cache: {FromCache})",
            result.ODataQuery,
            result.Provider,
            result.Elapsed. TotalMilliseconds,
            result.FromCache);

        var response = new NLToODataResponse
        {
            Success = true,
            OriginalQuery = result.OriginalQuery,
            ODataQuery = result.ODataQuery,
            EntitySet = result.EntitySet,
            Provider = result.Provider,
            ElapsedMs = (int)result.Elapsed.TotalMilliseconds,
            FromCache = result.FromCache,
            Usage = result.Usage != null ?  new UsageResponse
            {
                PromptTokens = result.Usage. PromptTokens,
                CompletionTokens = result.Usage.CompletionTokens,
                TotalTokens = result. Usage.TotalTokens
            } : null,
            FullUrl = result.EntitySet != null 
                ? $"/odata/{result.EntitySet}?{result.ODataQuery}"
                : null
        };

        return CustomResponse(true, response);
    }

    /// <summary>
    /// Testa a conversão e executa a query OData
    /// </summary>
    [HttpPost("odata/execute")]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    public async Task<IActionResult> ConvertAndExecute([FromBody] NLQueryRequest request)
    {
        // Primeiro converte
        var conversionResult = await _nlToODataService.ConvertAsync(
            request.Query,
            request.Provider,
            request.EntitySet);

        if (!conversionResult.Success)
        {
            return BadRequest(new { error = conversionResult.ErrorMessage });
        }

        // Redireciona para o endpoint OData correspondente
        var entitySet = conversionResult.EntitySet ??  request.EntitySet;
        if (string.IsNullOrEmpty(entitySet))
        {
            return BadRequest(new { error = "Não foi possível determinar o EntitySet" });
        }

        var odataUrl = $"/odata/{entitySet}?{conversionResult.ODataQuery}";
        
        return Ok(new
        {
            converted = true,
            originalQuery = conversionResult.OriginalQuery,
            odataQuery = conversionResult.ODataQuery,
            fullUrl = odataUrl,
            message = "Query convertida com sucesso.  Use a URL fornecida para executar a consulta."
        });
    }

    /// <summary>
    /// Lista providers disponíveis
    /// </summary>
    [HttpGet("providers")]
    [ProducesResponseType(typeof(List<ProviderInfo>), 200)]
    public IActionResult GetProviders([FromServices] IEnumerable<IAIProvider> providers)
    {
        var providerList = providers
            .Select(p => new ProviderInfo
            {
                Name = p.ProviderName,
                Enabled = p.IsEnabled
            })
            .ToList();

        return Ok(providerList);
    }
}

/// <summary>
/// Requisição para conversão NL → OData
/// </summary>
public class NLQueryRequest
{
    /// <summary>
    /// Query em linguagem natural
    /// Exemplo: "Me mostre produtos com preço acima de 100 reais"
    /// </summary>
    public string Query { get; set; } = string.Empty;

    /// <summary>
    /// Provider de AI (OpenAI, Gemini, OpenRouter)
    /// Se não especificado, usa o padrão configurado
    /// </summary>
    public string?  Provider { get; set; }

    /// <summary>
    /// EntitySet alvo (Products, Categories, etc.)
    /// Se não especificado, tenta inferir da query
    /// </summary>
    public string? EntitySet { get; set; }
}

/// <summary>
/// Resposta da conversão
/// </summary>
public class NLToODataResponse
{
    public bool Success { get; set; }
    public string? OriginalQuery { get; set; }
    public string? ODataQuery { get; set; }
    public string? EntitySet { get; set; }
    public string? FullUrl { get; set; }
    public string? ErrorMessage { get; set; }
    public string? Provider { get; set; }
    public int ElapsedMs { get; set; }
    public bool FromCache { get; set; }
    public UsageResponse? Usage { get; set; }
}

public class UsageResponse
{
    public int PromptTokens { get; set; }
    public int CompletionTokens { get; set; }
    public int TotalTokens { get; set; }
}

public class ProviderInfo
{
    public string Name { get; set; } = string.Empty;
    public bool Enabled { get; set; }
}
````

---

## ⚙️ **Passo 8: Configuração de Dependency Injection**

````csharp name=AIServiceConfiguration.cs
namespace e_Estoque_API. API.Configuration;

public static class AIServiceConfiguration
{
    public static IServiceCollection AddAIServices(this IServiceCollection services, IConfiguration configuration)
    {
        // HttpClients
        services.AddHttpClient("Gemini")
            .ConfigureHttpClient(client =>
            {
                client. Timeout = TimeSpan.FromSeconds(30);
            });

        services.AddHttpClient("OpenRouter")
            .ConfigureHttpClient(client =>
            {
                client.Timeout = TimeSpan.FromSeconds(30);
            });

        // AI Providers
        services.AddSingleton<IAIProvider, OpenAIProvider>();
        services.AddSingleton<IAIProvider, GeminiProvider>();
        services.AddSingleton<IAIProvider, OpenRouterProvider>();

        // Services
        services.AddScoped<NLToODataService>();

        return services;
    }
}
````

````csharp name=Program.cs
// Adicionar após outras configurações
builder.Services.AddAIServices(builder.Configuration);
````

---

## 📊 **Passo 9: Swagger Documentation**

````csharp name=SwaggerExamples.cs
using Swashbuckle.AspNetCore.Filters;

namespace e_Estoque_API.API.Swagger;

public class NLQueryRequestExample : IExamplesProvider<NLQueryRequest>
{
    public NLQueryRequest GetExamples()
    {
        return new NLQueryRequest
        {
            Query = "Me mostre produtos com preço acima de 100 reais ordenados por preço",
            Provider = "OpenAI",
            EntitySet = "Products"
        };
    }
}

public class NLQueryResponseExample : IExamplesProvider<NLToODataResponse>
{
    public NLToODataResponse GetExamples()
    {
        return new NLToODataResponse
        {
            Success = true,
            OriginalQuery = "Me mostre produtos com preço acima de 100 reais ordenados por preço",
            ODataQuery = "$filter=Price gt 100&$orderby=Price asc",
            EntitySet = "Products",
            FullUrl = "/odata/Products?$filter=Price gt 100&$orderby=Price asc",
            Provider = "OpenAI",
            ElapsedMs = 1234,
            FromCache = false,
            Usage = new UsageResponse
            {
                PromptTokens = 450,
                CompletionTokens = 25,
                TotalTokens = 475
            }
        };
    }
}
````

---

## 🧪 **Passo 10: Exemplos de Uso**

### **Exemplo 1: Conversão Simples**

```bash
POST /api/nlquery/odata
Content-Type: application/json

{
  "query": "Me mostre produtos com preço acima de 100 reais",
  "provider": "OpenAI"
}
```

**Resposta:**
```json
{
  "success": true,
  "data": {
    "success": true,
    "originalQuery": "Me mostre produtos com preço acima de 100 reais",
    "odataQuery": "$filter=Price gt 100",
    "entitySet": "Products",
    "fullUrl": "/odata/Products?$filter=Price gt 100",
    "provider": "OpenAI",
    "elapsedMs": 1234,
    "fromCache": false,
    "usage": {
      "promptTokens": 450,
      "completionTokens": 25,
      "totalTokens": 475
    }
  }
}
```

### **Exemplo 2: Query Complexa**

```bash
POST /api/nlquery/odata
Content-Type: application/json

{
  "query": "Top 10 produtos mais caros da categoria eletrônicos criados nos últimos 30 dias",
  "provider": "Gemini",
  "entitySet": "Products"
}
```

**Resposta:**
```json
{
  "success": true,
  "data": {
    "odataQuery": "$filter=Category/Name eq 'Eletrônicos' and CreatedAt gt 2024-10-27T00:00:00Z&$orderby=Price desc&$top=10&$expand=Category",
    "fullUrl": "/odata/Products? $filter=Category/Name eq 'Eletrônicos' and CreatedAt gt 2024-10-27T00:00:00Z&$orderby=Price desc&$top=10&$expand=Category"
  }
}
```

### **Exemplo 3: Listar Providers**

```bash
GET /api/nlquery/providers
```

**Resposta:**
```json
[
  {
    "name": "OpenAI",
    "enabled": true
  },
  {
    "name": "Gemini",
    "enabled": true
  },
  {
    "name": "OpenRouter",
    "enabled": false
  }
]
```

---

## ✅ **Checklist Final de Implementação**

### **📦 Configuração:**
- [ ] Instalar pacotes NuGet (OpenAI, Gemini, HttpClient)
- [ ] Configurar appsettings.json com API keys
- [ ] Configurar schema dos EntitySets
- [ ] Habilitar cache (Memory Cache)

### **🤖 AI Providers:**
- [ ] Implementar OpenAIProvider
- [ ] Implementar GeminiProvider
- [ ] Implementar OpenRouterProvider
- [ ] Testar cada provider individualmente

### **🎯 Serviço Principal:**
- [ ] Criar NLToODataService
- [ ] Implementar lógica de cache
- [ ] Implementar validação de queries
- [ ] Adicionar logging completo

### **🎮 API:**
- [ ] Criar NLQueryController
- [ ] Endpoint POST /api/nlquery/odata
- [ ] Endpoint POST /api/nlquery/odata/execute
- [ ] Endpoint GET /api/nlquery/providers
- [ ] Documentar com Swagger

### **🔒 Segurança:**
- [ ] Validar inputs (SQL injection, XSS)
- [ ] Rate limiting no endpoint
- [ ] Autenticação/Autorização
- [ ] Sanitização de queries geradas

### **📊 Observabilidade:**
- [ ] Logging de requests/responses
- [ ] Métricas de uso (tokens, tempo)
- [ ] Alertas para falhas
- [ ] Dashboard de monitoramento

### **🧪 Testes:**
- [ ] Testes unitários para cada provider
- [ ] Testes de integração
- [ ] Testes de carga
- [ ] Validação de queries geradas

---

## 🚀 **Melhorias Futuras**

1. **Feedback Loop:** Permitir usuários corrigirem queries geradas
2. **Fine-tuning:** Treinar modelo específico para o domínio
3. **Multi-idioma:** Suporte para outras línguas além do português
4. **Query Suggestion:** Sugerir queries baseadas em histórico
5. **Explain Query:** Endpoint para explicar o que uma query OData faz
6. **Reverse:** Converter OData → Linguagem Natural

---

**💡 Resultado Final:**

Esta implementação fornece uma solução enterprise-ready para converter linguagem natural em queries OData, com:
- ✅ Suporte a múltiplos provedores de AI
- ✅ Cache inteligente
- ✅ Validação robusta
- ✅ Observabilidade completa
- ✅ Prompt engineering otimizado
- ✅ Pronto para produção