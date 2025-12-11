# 🤖 AI Natural Language Query - EChamado

## 📋 Visão Geral

O EChamado agora possui integração com IA para converter consultas em **linguagem natural** para **sintaxe Gridify**, permitindo que usuários busquem dados sem conhecer a sintaxe de query.

### Exemplos de Uso

**Entrada (Linguagem Natural):**
```
"Mostrar chamados abertos do departamento de TI"
```

**Saída (Gridify Query):**
```
StatusName *= 'Aberto' & DepartmentName *= 'TI'
```

---

## 🏗️ Arquitetura

### Componentes Server-Side

```
┌─────────────────────────────────────────────────────────────────┐
│                        AI INFRASTRUCTURE                         │
├─────────────────────────────────────────────────────────────────┤
│                                                                   │
│  ┌────────────────────┐      ┌──────────────────────┐          │
│  │  IAIProvider       │ ◄────┤  AIProviderFactory   │          │
│  │  Interface         │      │                       │          │
│  └────────────────────┘      └──────────────────────┘          │
│           ▲                                                      │
│           │ implements                                           │
│    ┌──────┴──────┬──────────────┬──────────────┐              │
│    │             │              │               │              │
│  ┌─▼──────┐  ┌──▼───────┐  ┌───▼───────┐  ┌───▼──────────┐  │
│  │ OpenAI │  │ Gemini   │  │OpenRouter │  │ Cached       │  │
│  │Provider│  │ Provider │  │ Provider  │  │AIProvider    │  │
│  └────────┘  └──────────┘  └───────────┘  └──────────────┘  │
│                                                                   │
│  ┌────────────────────────────────────────────────────────┐    │
│  │         GridifyPromptTemplates                          │    │
│  │  • OrderMetadata    • CategoryMetadata                 │    │
│  │  • DepartmentMetadata • StatusTypeMetadata             │    │
│  └────────────────────────────────────────────────────────┘    │
│                                                                   │
│  ┌────────────────────────────────────────────────────────┐    │
│  │         NLToGridifyService                              │    │
│  │  • ConvertAsync()                                       │    │
│  │  • ConvertWithOrderingAsync()                           │    │
│  │  • ConvertBatchAsync()                                  │    │
│  └────────────────────────────────────────────────────────┘    │
│                            │                                      │
│                            ▼                                      │
│  ┌────────────────────────────────────────────────────────┐    │
│  │     API Endpoint: /v1/ai/nl-to-gridify                 │    │
│  │     ConvertNLToGridifyEndpoint                          │    │
│  └────────────────────────────────────────────────────────┘    │
└─────────────────────────────────────────────────────────────────┘
```

### Componentes Client-Side

```
┌──────────────────────────────────────────────────────────┐
│                  BLAZOR COMPONENTS                        │
├──────────────────────────────────────────────────────────┤
│                                                            │
│  ┌──────────────────────────────────────────┐            │
│  │        NLQueryService                     │            │
│  │  • ConvertToGridifyAsync()               │            │
│  └──────────────────────────────────────────┘            │
│                    │                                       │
│                    ▼                                       │
│  ┌──────────────────────────────────────────┐            │
│  │     NLQueryInput.razor                    │            │
│  │  • Input de linguagem natural            │            │
│  │  • Botão de conversão                    │            │
│  │  • Exibição de query gerada              │            │
│  │  • Exemplos e sugestões                  │            │
│  └──────────────────────────────────────────┘            │
│                    │                                       │
│                    ▼                                       │
│  ┌──────────────────────────────────────────┐            │
│  │     OrderListAI.razor                     │            │
│  │  • Página de demonstração                │            │
│  │  • Integração com componente NL          │            │
│  │  • Filtros manuais Gridify               │            │
│  └──────────────────────────────────────────┘            │
└──────────────────────────────────────────────────────────┘
```

---

## ⚙️ Configuração

### 1. Configurar Provedores de IA

Edite `appsettings.json` no Server:

```json
{
  "AISettings": {
    "DefaultProvider": "OpenAI",
    "EnableCaching": true,
    "CacheDurationMinutes": 60,
    "EnableLogging": true,

    "OpenAI": {
      "ApiKey": "sk-proj-YOUR_OPENAI_API_KEY_HERE",
      "Model": "gpt-4o-mini",
      "Endpoint": null,
      "Enabled": true
    },

    "Gemini": {
      "ApiKey": "YOUR_GEMINI_API_KEY_HERE",
      "Model": "gemini-2.0-flash-exp",
      "Enabled": false
    },

    "OpenRouter": {
      "ApiKey": "YOUR_OPENROUTER_API_KEY_HERE",
      "Model": "meta-llama/llama-3.1-70b-instruct",
      "Endpoint": "https://openrouter.ai/api/v1",
      "Enabled": false
    }
  }
}
```

### 2. Obter API Keys

#### OpenAI (Recomendado)
1. Acesse https://platform.openai.com
2. Crie uma conta ou faça login
3. Vá em API Keys → Create new secret key
4. Copie a chave e adicione no `appsettings.json`
5. **Modelo recomendado**: `gpt-4o-mini` (rápido e econômico)

#### Google Gemini (Alternativo)
1. Acesse https://ai.google.dev
2. Obtenha sua API key
3. **Modelo recomendado**: `gemini-2.0-flash-exp` (gratuito no tier básico)

#### OpenRouter (Múltiplos modelos)
1. Acesse https://openrouter.ai
2. Cadastre-se e obtenha API key
3. Acesso a múltiplos modelos (GPT-4, Claude, Llama, etc.)

---

## 🚀 Uso

### API Endpoint

**POST** `/v1/ai/nl-to-gridify`

**Request:**
```json
{
  "entityName": "Order",
  "query": "Mostrar chamados abertos com prioridade alta",
  "provider": "OpenAI"
}
```

**Response:**
```json
{
  "success": true,
  "gridifyQuery": "StatusName *= 'Aberto' & Priority = 3",
  "originalQuery": "Mostrar chamados abertos com prioridade alta",
  "entityName": "Order",
  "provider": "OpenAI",
  "model": "gpt-4o-mini",
  "fromCache": false,
  "responseTimeMs": 450,
  "tokensUsed": 285,
  "errorMessage": null
}
```

### Usando no Blazor

```razor
@page "/example"
@using EChamado.Client.Components
@inject INLQueryService NLQueryService

<NLQueryInput EntityName="Order"
              OnQueryGenerated="HandleQueryGenerated" />

@code {
    private async Task HandleQueryGenerated(string gridifyQuery)
    {
        // Use a query gerada
        Console.WriteLine($"Query gerada: {gridifyQuery}");

        // Exemplo: aplicar no endpoint Gridify
        // await OrderService.GetOrdersGridifyAsync(filter: gridifyQuery);
    }
}
```

---

## 📝 Entidades Suportadas

### 1. Orders (Chamados)
```
Propriedades principais:
- Title, Description
- StatusName, TypeName, DepartmentName
- Priority (1=Low, 2=Medium, 3=High, 4=Critical)
- OpeningDate, ClosingDate, DueDate
- RequestingUserEmail, AssignedUserEmail
- IsDeleted, CreatedAt, UpdatedAt
```

**Exemplos de queries:**
- "Chamados abertos do TI"
- "Tickets urgentes criados hoje"
- "Orders atrasadas atribuídas para mim"

### 2. Categories (Categorias)
```
Propriedades principais:
- Name, Description
- Icon, Color
- IsActive, CreatedAt
```

**Exemplos:**
- "Categorias ativas"
- "Categorias com Hardware no nome"

### 3. Departments (Departamentos)
```
Propriedades principais:
- Name, Description
- ManagerEmail
- IsActive, CreatedAt
```

### 4. OrderTypes (Tipos de Chamado)
```
Propriedades principais:
- Name, Description
- DefaultPriority, SlaHours
- RequiresApproval, IsActive
```

### 5. StatusTypes (Status)
```
Propriedades principais:
- Name, Description
- IsFinal, Order, IsActive
```

---

## 💡 Exemplos de Consultas

### Chamados (Orders)

| Linguagem Natural | Gridify Gerado |
|-------------------|----------------|
| "Chamados abertos" | `StatusName *= 'Aberto'` |
| "Tickets de suporte urgentes" | `TypeName *= 'Suporte' & Priority >= 3` |
| "Chamados do TI criados hoje" | `DepartmentName *= 'TI' & CreatedAt >= 2025-01-27` |
| "Orders fechadas esta semana" | `StatusName *= 'Fechado' & ClosingDate >= 2025-01-20` |
| "Tickets não atribuídos" | `AssignedUserId = null` |
| "Chamados vencidos" | `DueDate < 2025-01-27 & StatusName != 'Fechado'` |

### Categorias

| Linguagem Natural | Gridify Gerado |
|-------------------|----------------|
| "Categorias ativas" | `IsActive = true & IsDeleted = false` |
| "Categorias de Hardware" | `Name *= 'Hardware'` |
| "Categorias criadas em 2024" | `CreatedAt >= 2024-01-01 & CreatedAt < 2025-01-01` |

### Departamentos

| Linguagem Natural | Gridify Gerado |
|-------------------|----------------|
| "Departamento de TI" | `Name *= 'TI'` |
| "Departamentos com gerente" | `ManagerId != null` |
| "Departamentos ativos" | `IsActive = true` |

---

## 🎯 Performance e Custos

### Cache Automático

O sistema possui cache automático de 60 minutos (configurável):

```csharp
// Primeira chamada: consulta a AI (custo: ~$0.0001)
var result1 = await NLQueryService.ConvertToGridifyAsync("Order", "chamados abertos");
// FromCache: false, ResponseTime: 450ms

// Segunda chamada (mesma query): retorna do cache (custo: $0)
var result2 = await NLQueryService.ConvertToGridifyAsync("Order", "chamados abertos");
// FromCache: true, ResponseTime: 2ms
```

### Estimativa de Custos (OpenAI gpt-4o-mini)

| Uso Mensal | Requests | Custo Estimado |
|------------|----------|----------------|
| Pequeno | 1.000 | ~$0.10 |
| Médio | 10.000 | ~$1.00 |
| Grande | 100.000 | ~$10.00 |

**Nota:** Com cache ativo, o custo real é muito menor!

---

## 🔒 Segurança

### Validação de Queries

O sistema **não executa queries diretamente**. A query Gridify gerada é:

1. ✅ Validada pelo backend Gridify
2. ✅ Sanitizada contra SQL Injection
3. ✅ Limitada às propriedades permitidas
4. ✅ Executada apenas em contextos autorizados

### Boas Práticas

```csharp
// ❌ NÃO fazer isso
var userInput = "'; DROP TABLE Orders; --";
await ConvertToGridifyAsync("Order", userInput); // Seguro: não é executado

// ✅ Fazer isso
var result = await ConvertToGridifyAsync("Order", userInput);
if (result.Success)
{
    // A query Gridify é validada pelo backend antes de ser usada
    var orders = await OrderService.GetGridifyAsync(result.GridifyQuery);
}
```

---

## 🐛 Troubleshooting

### Erro: "AI provider is not available"

**Solução:**
1. Verifique se a API key está configurada em `appsettings.json`
2. Confirme que `Enabled: true` no provedor
3. Teste a API key diretamente na plataforma do provedor

### Erro: "Conversion failed"

**Possíveis causas:**
- Query muito ambígua ou complexa
- Entidade não suportada
- Provedor de IA temporariamente indisponível

**Solução:**
- Reformule a query em linguagem natural
- Seja mais específico
- Tente outro provedor

### Query gerada está incorreta

**Solução:**
1. Seja mais específico na query em linguagem natural
2. Use nomes de propriedades conhecidas
3. Consulte a lista de propriedades suportadas por entidade

---

## 📊 Monitoramento

### Logs

O sistema gera logs detalhados:

```log
[INFO] Converting NL query to Gridify. Entity: Order, Query: chamados abertos
[INFO] Sending request to OpenAI. Model: gpt-4o-mini, Temperature: 0.1, MaxTokens: 500
[INFO] OpenAI response received. Tokens: 285, Time: 450ms
[INFO] NL to Gridify conversion successful. Input: 'chamados abertos' → Output: 'StatusName *= 'Aberto'' (Provider: OpenAI, Cached: false)
```

### Métricas

Cada conversão retorna métricas úteis:
- `ResponseTimeMs`: Tempo de resposta
- `TokensUsed`: Tokens consumidos
- `FromCache`: Se foi recuperado do cache
- `Provider`: Provedor usado

---

## 🔮 Próximos Passos

### Melhorias Planejadas

1. **Sugestões Inteligentes**: Autocompletar queries baseado em histórico
2. **Tradução Multi-idioma**: Suporte para inglês, espanhol, etc.
3. **Query History**: Salvar queries favoritas
4. **Feedback Loop**: Melhorar prompts baseado em feedback do usuário
5. **Integração Total**: Conectar diretamente com todos os endpoints Gridify

### Contribuindo

Para adicionar suporte a novas entidades:

1. Adicione metadados em `GridifyPromptTemplates.cs`
2. Teste a conversão com queries de exemplo
3. Documente as propriedades suportadas

---

## 📚 Referências

- [Gridify Documentation](https://github.com/alirezanet/Gridify)
- [OpenAI API](https://platform.openai.com/docs)
- [Google Gemini](https://ai.google.dev)
- [OpenRouter](https://openrouter.ai/docs)

---

**Desenvolvido com ❤️ para EChamado**
Versão: 1.0.0 (Janeiro 2025)
