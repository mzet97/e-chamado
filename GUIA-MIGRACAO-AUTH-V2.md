# Guia de Migração - Endpoints de Autenticação

## 📋 Objetivo

Este guia mostra como migrar dos endpoints `/v1/auth/login` e `/v1/auth/register` (atuais, com DTOs inflados) para `/v2/auth/login-v2` e `/v2/auth/register-v2` (otimizados, com DTOs limpos).

---

## 🎯 Benefícios da Migração

### Antes (Problema Atual)
```json
{
  "result": { /* objeto complexo desnecessário */ },
  "id": { /* UUID técnico */ },
  "correlationId": { /* Trace ID técnico */ },
  "email": "usuario@exemplo.com",
  "password": "senha123"
}
```

### Depois (Solução Otimizada)
```json
{
  "email": "usuario@exemplo.com", 
  "password": "senha123"
}
```

---

## 📁 Estrutura de Arquivos Criados

```
/Server/Endpoints/Auth/DTOs/
├── LoginRequestDto.cs
├── RegisterRequestDto.cs
└── AuthDTOSExtensions.cs

/Server/Endpoints/Auth/
├── LoginUserEndpointV2.cs
└── RegisterUserEndpointV2.cs
```

---

## 🚀 Passo a Passo da Implementação

### Passo 1: Registrar os Novos Endpoints

Adicione no `Endpoint.cs` (ou `Program.cs`):

```csharp
// NOVO: Endpoints otimizados
endpoints.MapGroup("v2/auth")
    .WithTags("auth-v2")
    .MapEndpoint<RegisterUserEndpointV2>()
    .MapEndpoint<LoginUserEndpointV2>();
```

### Passo 2: Testar os Novos Endpoints

#### POST /v2/auth/register-v2
```bash
curl -X POST "https://localhost:5001/v2/auth/register-v2" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "usuario@exemplo.com",
    "password": "senha123456"
  }'
```

#### POST /v2/auth/login-v2
```bash
curl -X POST "https://localhost:5001/v2/auth/login-v2" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "usuario@exemplo.com",
    "password": "senha123"
  }'
```

### Passo 3: Atualizar Cliente (Frontend/Apps)

#### Para React/JavaScript:
```javascript
// ANTES (enviava campos desnecessários)
const loginOld = async (email, password) => {
  const response = await fetch('/v1/auth/login', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({
      result: null,
      id: null,
      correlationId: null,
      email,
      password
    })
  });
  return response.json();
};

// DEPOIS (envia apenas o necessário)
const loginNew = async (email, password) => {
  const response = await fetch('/v2/auth/login-v2', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({
      email,    // ✅ Apenas os campos necessários
      password  // ✅ Interface limpa
    })
  });
  return response.json();
};
```

#### Para C#/.NET Cliente:
```csharp
// ANTES
public class OldLoginDto
{
    public LoginResponseViewModelBaseResult? Result { get; set; }
    public Id? Id { get; set; }
    public Id? CorrelationId { get; set; }
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
}

// DEPOIS
public class NewLoginDto
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
}
```

---

## 🧪 Validação das Mudanças

### 1. Verificar Swagger
- Acesse `/swagger`
- Confirme que os endpoints v2 mostram apenas `email` e `password`
- Confirme que endpoints v1 ainda existem (compatibilidade)

### 2. Testar Manualmente

#### Registro Válido
```json
// Request
{
  "email": "teste@exemplo.com",
  "password": "senha123456"
}

// Response
{
  "success": true,
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "expiresIn": 3600,
    "userToken": {
      "id": "123",
      "email": "teste@exemplo.com",
      "claims": [...]
    }
  }
}
```

#### Falhas de Validação
```json
// Request (email inválido)
{
  "email": "email-invalido",
  "password": "senha123"
}

// Response
{
  "success": false,
  "message": "Email inválido",
  "data": null
}
```

---

## 📊 Comparação Antes vs Depois

### Análise do Swagger

| Aspecto | Versão Atual (v1) | Versão Otimizada (v2) |
|---------|-------------------|----------------------|
| **Campos no Request** | 5 (incluindo técnicos) | 2 (apenas necessários) |
| **Validação Clara** | ❌ Confusa | ✅ Direta |
| **Experiência Dev** | ❌ Frustante | ✅ Limpa |
| **Documentação** | ❌ Poluída | ✅ Profissional |

### Payload Comparison

#### v1/auth/login (ATUAL)
```json
{
  "result": {
    "success": true,
    "message": "Login realizado com sucesso",
    "data": { /* token details */ }
  },
  "id": { "value": "12345678-1234-1234-1234-123456789012" },
  "correlationId": { "value": "87654321-4321-4321-4321-210987654321" },
  "email": "usuario@exemplo.com",
  "password": "senha123"
}
```

#### v2/auth/login-v2 (NOVO)
```json
{
  "email": "usuario@exemplo.com",
  "password": "senha123"
}
```

---

## ⚡ Scripts de Teste

### Teste de Carga
```bash
#!/bin/bash

echo "Testando endpoints de autenticação..."

# Teste de registro
echo "1. Testando registro..."
curl -X POST "https://localhost:5001/v2/auth/register-v2" \
  -H "Content-Type: application/json" \
  -d '{"email":"teste@exemplo.com","password":"senha123456"}' \
  | jq '.'

# Teste de login
echo "2. Testando login..."
curl -X POST "https://localhost:5001/v2/auth/login-v2" \
  -H "Content-Type: application/json" \
  -d '{"email":"teste@exemplo.com","password":"senha123456"}' \
  | jq '.'

echo "✅ Testes concluídos"
```

---

## 🔄 Estratégia de Migração

### Fase 1: Paralelo (Recomendada)
1. **Manter** endpoints v1 funcionando
2. **Adicionar** endpoints v2 otimizados
3. **Comunicar** à equipe sobre os novos endpoints
4. **Testar** funcionalidades dos novos endpoints

### Fase 2: Transição
1. **Atualizar** clientes que usam v1 para v2
2. **Monitorar** uso dos endpoints
3. **Documentar** mudanças necessárias

### Fase 3: Depreciação
1. **Marcar** endpoints v1 como deprecated no Swagger
2. **Manter** funcionando por período determinado
3. **Comunicação** sobre data de remoção

---

## 🎯 Conclusão

A migração para os endpoints v2 oferece:
- ✅ **Interface mais limpa** para desenvolvedores
- ✅ **Documentação mais profissional** no Swagger
- ✅ **Validações mais claras** e específicas
- ✅ **Melhor experiência** ao usar a API
- ✅ **Preparação** para versões futuras

**Tempo estimado de implementação**: 2-4 horas
**Impacto**: Alto (melhoria significativa na usabilidade)
**Risco**: Baixo (mantém compatibilidade com v1)

---

*Documento criado em: $(Get-Date -Format "dd/MM/yyyy")*
*Especialista C#/.NET - EChamado Team*
