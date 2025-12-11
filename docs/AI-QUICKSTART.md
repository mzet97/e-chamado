# 🚀 AI Natural Language Query - Quick Start Guide

## ⚡ Setup em 5 Minutos

### Passo 1: Obter API Key (2 minutos)

Escolha um provedor:

**Opção A: OpenAI (Recomendado)**
1. Acesse: https://platform.openai.com
2. Login/Cadastro
3. API Keys → Create new secret key
4. Copie a chave: `sk-proj-...`

**Opção B: Google Gemini (Gratuito)**
1. Acesse: https://ai.google.dev
2. Get API Key
3. Copie a chave

**Opção C: OpenRouter (Múltiplos Modelos)**
1. Acesse: https://openrouter.ai
2. Sign Up → Get API Key
3. Copie a chave

---

### Passo 2: Configurar (1 minuto)

Edite `/src/EChamado/Server/EChamado.Server/appsettings.json`:

```json
{
  "AISettings": {
    "DefaultProvider": "OpenAI",
    "EnableCaching": true,
    "OpenAI": {
      "ApiKey": "sk-proj-SUA_CHAVE_AQUI",
      "Model": "gpt-4o-mini",
      "Enabled": true
    }
  }
}
```

**IMPORTANTE**: Não commite a API key no git! Use environment variables em produção.

---

### Passo 3: Executar (1 minuto)

```bash
cd /mnt/e/TI/git/e-chamado/src/EChamado/Server/EChamado.Server
dotnet run
```

Aguarde até ver:
```
Now listening on: https://localhost:7296
```

---

### Passo 4: Testar API (1 minuto)

**Opção A: Usando curl**

```bash
curl -X POST https://localhost:7296/v1/ai/nl-to-gridify \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_AUTH_TOKEN" \
  -d '{
    "entityName": "Order",
    "query": "Mostrar chamados abertos do departamento de TI"
  }'
```

**Opção B: Usando test script**

```bash
cd /mnt/e/TI/git/e-chamado
./test-ai-nlquery.sh
```

**Resposta Esperada:**

```json
{
  "success": true,
  "gridifyQuery": "StatusName *= 'Aberto' & DepartmentName *= 'TI'",
  "originalQuery": "Mostrar chamados abertos do departamento de TI",
  "entityName": "Order",
  "provider": "OpenAI",
  "model": "gpt-4o-mini",
  "fromCache": false,
  "responseTimeMs": 450,
  "tokensUsed": 285
}
```

---

### Passo 5: Usar no Frontend (< 1 minuto)

1. Execute o Client Blazor:
```bash
cd /mnt/e/TI/git/e-chamado/src/EChamado/Client/EChamado.Client
dotnet run
```

2. Acesse: `https://localhost:7274/orders/ai`

3. Digite: "Mostrar chamados abertos"

4. Clique em "Converter para Gridify"

5. Veja a query gerada!

---

## 🎯 Exemplos Rápidos

### Chamados (Orders)

```
"Chamados abertos"
→ StatusName *= 'Aberto'

"Tickets urgentes do TI criados hoje"
→ DepartmentName *= 'TI' & Priority >= 3 & CreatedAt >= 2025-01-27

"Orders atrasadas não fechadas"
→ DueDate < 2025-01-27 & StatusName != 'Fechado'
```

### Categorias

```
"Categorias ativas"
→ IsActive = true & IsDeleted = false

"Categorias de Hardware criadas em 2024"
→ Name *= 'Hardware' & CreatedAt >= 2024-01-01
```

### Departamentos

```
"Departamento de TI"
→ Name *= 'TI'

"Departamentos com gerente ativo"
→ ManagerId != null & IsActive = true
```

---

## 🐛 Troubleshooting Rápido

### Erro: "AI provider is not available"
**Solução:** Verifique se `Enabled: true` e API key está correta

### Erro: "Unauthorized"
**Solução:** Faça login primeiro em `/authentication/login`

### Erro: "Conversion failed"
**Solução:** Reformule a query em português mais simples

---

## 💡 Dicas Pro

### 1. Seja Específico
✅ "Chamados abertos do departamento de TI com prioridade alta"
❌ "Chamados"

### 2. Use Nomes Conhecidos
✅ "StatusName contém 'Aberto'"
❌ "Estado é livre"

### 3. Combine Filtros
✅ "Tickets urgentes criados hoje do TI"
= 3 filtros em uma query!

### 4. Aproveite o Cache
- Mesma query = resposta instantânea (2ms)
- Cache dura 60 minutos
- Economia de custos!

---

## 📊 Monitoramento

### Verificar Logs

```bash
# Ver conversões AI em tempo real
tail -f logs/echamado-*.log | grep "NL to Gridify"
```

### Métricas Importantes

Cada resposta inclui:
- `responseTimeMs`: Tempo de conversão
- `tokensUsed`: Tokens consumidos
- `fromCache`: Se veio do cache
- `provider`: Qual AI foi usada

---

## 🔐 Segurança

### ❌ NÃO faça

```bash
# NÃO commite API keys
git add appsettings.json  # PERIGO!
```

### ✅ FAÇA

```bash
# Use environment variables em produção
export AISettings__OpenAI__ApiKey="sk-proj-..."
```

Ou use `appsettings.Production.json` (não commitado):

```json
{
  "AISettings": {
    "OpenAI": {
      "ApiKey": "sk-proj-..."
    }
  }
}
```

---

## 💰 Custos

### OpenAI gpt-4o-mini

| Uso | Conversões | Custo |
|-----|------------|-------|
| Teste | 10 | $0.001 |
| Dev | 100 | $0.01 |
| Produção Light | 1.000 | $0.10 |
| Produção Heavy | 10.000 | $1.00 |

**Com cache**: Reduza custos em ~70%!

---

## 🚀 Próximo Passo

📚 Leia a [Documentação Completa](AI-NATURAL-LANGUAGE-QUERY.md)

🎯 Explore as [5 Entidades Suportadas](AI-NATURAL-LANGUAGE-QUERY.md#entidades-suportadas)

💻 Veja [Exemplos Avançados](AI-NATURAL-LANGUAGE-QUERY.md#exemplos-de-consultas)

---

**Pronto para usar! 🎉**
