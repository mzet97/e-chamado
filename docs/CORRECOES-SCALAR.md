# Correções - Implementação Scalar

## Resumo
Este documento descreve as correções aplicadas na implementação do Scalar API Documentation para resolver erros de compilação.

## Erros Identificados

### 1. Duplicação da Classe `SwaggerDefaultValues` (CS0101, CS0111)

**Erro Completo:**
```
CS0101: The namespace 'EChamado.Server.Configuration' already contains a definition for 'SwaggerDefaultValues'
CS0111: Type 'SwaggerDefaultValues' already defines a member called 'Apply' with the same parameter types
```

**Causa:**
A classe `SwaggerDefaultValues` foi definida duas vezes:
- `SwaggerConfig.cs:62` (original)
- `ScalarConfig.cs:257` (duplicada acidentalmente)

**Solução:**
Removida a classe duplicada do `ScalarConfig.cs`. A classe original em `SwaggerConfig.cs` é reutilizada por ambos os arquivos de configuração através do namespace compartilhado.

**Arquivos Modificados:**
- `/src/EChamado/Server/EChamado.Server/Configuration/ScalarConfig.cs` (linhas 256-297 removidas)

---

### 2. Tipo Incorreto no Método de Extensão (CS1929)

**Erro Completo:**
```
CS1929: 'IApplicationBuilder' does not contain a definition for 'MapScalarApiReference'
and the best extension method overload 'ScalarEndpointRouteBuilderExtensions.MapScalarApiReference(IEndpointRouteBuilder, Action<ScalarOptions>)'
requires a receiver of type 'Microsoft.AspNetCore.Routing.IEndpointRouteBuilder'
```

**Causa:**
O método `UseApiDocumentation()` estava definido para retornar `IApplicationBuilder`, mas `MapScalarApiReference()` requer um `IEndpointRouteBuilder`.

**Problema de Código Original:**
```csharp
public static IApplicationBuilder UseApiDocumentation(this IApplicationBuilder app)
{
    app.UseSwagger(...);
    app.MapScalarApiReference(...); // ❌ Erro: IApplicationBuilder não tem MapScalarApiReference
    return app;
}
```

**Solução:**
Alterado o tipo do parâmetro e retorno para `WebApplication`, que implementa ambos `IApplicationBuilder` e `IEndpointRouteBuilder`.

**Código Corrigido:**
```csharp
public static WebApplication UseApiDocumentation(this WebApplication app)
{
    app.UseSwagger(...);
    app.MapScalarApiReference(...); // ✅ OK: WebApplication implementa IEndpointRouteBuilder
    return app;
}
```

**Arquivos Modificados:**
- `/src/EChamado/Server/EChamado.Server/Configuration/ScalarConfig.cs:218`

---

### 3. Ordem Incorreta de Middlewares

**Problema:**
O método `UseApiDocumentation()` estava sendo chamado ANTES de `UseRouting()`, causando potenciais problemas com `MapScalarApiReference()`.

**Ordem Incorreta:**
```csharp
// ❌ Ordem ERRADA
app.UseApiDocumentation(); // MapScalarApiReference precisa de UseRouting primeiro
app.UseCors("AllowBlazorClient");
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
```

**Ordem Correta:**
```csharp
// ✅ Ordem CORRETA
app.UseCors("AllowBlazorClient");
app.UseRouting();              // 1. Configurar roteamento primeiro
app.UseAuthentication();       // 2. Autenticação
app.UseAuthorization();        // 3. Autorização
app.UseApiDocumentation();     // 4. Scalar (precisa do roteamento configurado)
app.MapEndpoints();            // 5. Mapear endpoints da API
```

**Arquivos Modificados:**
- `/src/EChamado/Server/EChamado.Server/Program.cs:95-108`

---

### 4. Conversão Inválida de String para IEnumerable<string> (CS0029)

**Erro Completo:**
```
CS0029: Cannot implicitly convert type 'string' to 'System.Collections.Generic.IEnumerable<string>'
```

**Causa:**
A propriedade `Scopes` no método `WithOAuth2Authentication()` espera uma coleção de strings (`IEnumerable<string>`), mas estava recebendo uma string única.

**Problema de Código Original:**
```csharp
.WithOAuth2Authentication(x =>
{
    x.ClientId = "mobile-client";
    x.Scopes = "openid profile email roles api chamados"; // ❌ Erro: string em vez de array
})
```

**Solução:**
Alterado para usar uma coleção de strings usando a sintaxe de collection expressions do C# 12:

**Código Corrigido:**
```csharp
.WithOAuth2Authentication(x =>
{
    x.ClientId = "mobile-client";
    x.Scopes = ["openid", "profile", "email", "roles", "api", "chamados"]; // ✅ OK: array de strings
})
```

**Alternativas Válidas:**
```csharp
// Opção 1: Array tradicional
x.Scopes = new[] { "openid", "profile", "email", "roles", "api", "chamados" };

// Opção 2: List<string>
x.Scopes = new List<string> { "openid", "profile", "email", "roles", "api", "chamados" };

// Opção 3: Collection expressions (C# 12+ - usado na correção)
x.Scopes = ["openid", "profile", "email", "roles", "api", "chamados"];
```

**Arquivos Modificados:**
- `/src/EChamado/Server/EChamado.Server/Configuration/ScalarConfig.cs:246`

---

### 5. EndpointPathPrefix Requer {documentName} (ArgumentException)

**Erro Completo:**
```
System.ArgumentException: 'EndpointPathPrefix' must define '{documentName}'.
  at Scalar.AspNetCore.ScalarEndpointRouteBuilderExtensions.MapScalarApiReference(IEndpointRouteBuilder endpoints, Action`1 configureOptions)
  at EChamado.Server.Configuration.ScalarConfig.UseApiDocumentation(WebApplication app) in ScalarConfig.cs:line 226
```

**Causa:**
O método `WithEndpointPrefix()` do Scalar exige que o caminho contenha a variável `{documentName}` para suportar múltiplas versões da API.

**Problema de Código Original:**
```csharp
.WithOpenApiRoutePattern("/openapi/{documentName}.json")
.WithEndpointPrefix("/api-docs")  // ❌ Erro: falta {documentName}
```

**Solução:**
Adicionado `{documentName}` ao endpoint prefix:

**Código Corrigido:**
```csharp
.WithOpenApiRoutePattern("/openapi/{documentName}.json")
.WithEndpointPrefix("/api-docs/{documentName}")  // ✅ OK: inclui {documentName}
```

**Resultado:**
- **Antes**: https://localhost:7296/api-docs (erro)
- **Depois**: https://localhost:7296/api-docs/v1 (funciona)

A variável `{documentName}` é substituída pela versão da API (definida em `SwaggerDoc("v1", ...)`) automaticamente.

**Arquivos Modificados:**
- `/src/EChamado/Server/EChamado.Server/Configuration/ScalarConfig.cs:237`
- `/docs/GUIA-USO-SCALAR.md` (URLs atualizadas)

---

### 6. Metadata File Not Found (CS0006)

**Erro Completo:**
```
CS0006: Metadata file 'E:\TI\git\e-chamado\src\EChamado\Server\EChamado.Server\obj\Debug\net9.0\ref\EChamado.Server.dll' could not be found
```

**Causa:**
Erro secundário causado pelos erros de compilação anteriores (CS0101, CS0111, CS1929, CS0029). Quando o projeto principal não compila, os testes de integração não conseguem encontrar a DLL de referência.

**Solução:**
Resolvido automaticamente ao corrigir os erros de compilação anteriores.

---

## Resumo das Mudanças

### Arquivos Modificados

| Arquivo | Linhas | Mudança | Motivo |
|---------|--------|---------|--------|
| `ScalarConfig.cs` | 256-297 | Removidas | Eliminar duplicação de classe |
| `ScalarConfig.cs` | 218 | Alterado tipo | Corrigir assinatura do método |
| `ScalarConfig.cs` | 237 | Adicionar `{documentName}` | Corrigir EndpointPrefix |
| `ScalarConfig.cs` | 246 | String → Array | Corrigir tipo de Scopes |
| `Program.cs` | 95-108 | Reordenadas | Ordem correta de middlewares |
| `GUIA-USO-SCALAR.md` | 11, 148, 691 | URLs atualizadas | Refletir endpoint correto |
| `CORRECOES-SCALAR.md` | 148-183 | Adicionada seção | Documentar correção ArgumentException |

### Resultado

✅ **Todos os erros foram resolvidos**
- CS0101: Resolvido (classe duplicada removida)
- CS0111: Resolvido (método duplicado removido)
- CS1929: Resolvido (tipo correto no método de extensão)
- CS0029: Resolvido (string convertida para array de strings)
- ArgumentException: Resolvido (adicionado {documentName} ao endpoint prefix)
- CS0006: Resolvido (consequência dos anteriores)

---

## Pipeline de Middlewares (Ordem Correta)

A ordem correta dos middlewares no ASP.NET Core é crítica:

```csharp
// ✅ ORDEM CORRETA DOS MIDDLEWARES
var app = builder.Build();

// 1. CORS (antes de routing)
app.UseCors("AllowBlazorClient");

// 2. Logging (monitorar todas as requisições)
app.UseRequestLogging();
app.UsePerformanceLogging(slowRequestThresholdMs: 3000);

// 3. ROUTING (define as rotas)
app.UseRouting();

// 4. AUTENTICAÇÃO (identifica o usuário)
app.UseAuthentication();

// 5. AUTORIZAÇÃO (verifica permissões)
app.UseAuthorization();

// 6. SCALAR DOCUMENTATION (usa IEndpointRouteBuilder)
app.UseApiDocumentation();

// 7. HEALTH CHECKS
app.UseHealthCheckConfiguration();

// 8. MAPEAR ENDPOINTS (registra as rotas)
app.MapEndpoints();
app.MapUserReadEndpoints();
app.MapControllers();

app.Run();
```

### Por Que a Ordem Importa?

1. **CORS antes de Routing**: CORS precisa interceptar requisições antes do routing para aplicar headers corretos
2. **Routing antes de Endpoints**: O sistema de roteamento deve ser configurado antes de mapear endpoints
3. **Authentication antes de Authorization**: A identidade do usuário deve ser estabelecida antes de verificar permissões
4. **Scalar depois de Routing**: `MapScalarApiReference` usa `IEndpointRouteBuilder` que requer routing configurado
5. **Map* no final**: Registro de rotas deve vir depois de todos os middlewares de pipeline

---

## Testando as Correções

### 1. Compilar o Projeto

```bash
cd /mnt/e/TI/git/e-chamado/src/EChamado
dotnet build
```

**Resultado Esperado:**
```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

### 2. Executar a API

```bash
# Terminal 1 - Auth Server
cd src/EChamado/Echamado.Auth
dotnet run

# Terminal 2 - API Server
cd src/EChamado/Server/EChamado.Server
dotnet run
```

### 3. Acessar a Documentação

Abrir no navegador:
- **Scalar UI**: https://localhost:7296/api-docs
- **OpenAPI JSON**: https://localhost:7296/openapi/v1.json

**Resultado Esperado:**
- Interface Scalar carregada com tema Purple
- Endpoints visíveis e navegáveis
- Botão "Authorize" funcional
- Modelos de dados exibidos na sidebar

---

## Lições Aprendidas

### 1. Evitar Duplicação de Código
- Classes utilitárias devem existir em apenas um lugar
- Reutilizar classes existentes através de namespaces compartilhados

### 2. Tipos Corretos em Métodos de Extensão
- `IApplicationBuilder`: Para middlewares do pipeline (UseXxx)
- `IEndpointRouteBuilder`: Para mapeamento de endpoints (MapXxx)
- `WebApplication`: Implementa ambos, usar quando necessário

### 3. Ordem de Middlewares é Crítica
- CORS → Routing → Auth → Authorization → Custom → Endpoints
- Documentar a ordem no código com comentários
- Testar mudanças na ordem cuidadosamente

### 4. Erros Secundários
- Um erro de compilação pode causar vários outros (efeito cascata)
- Resolver erros primários primeiro (CS0101, CS0111) antes dos secundários (CS0006)
- Build limpo após correções: `dotnet clean && dotnet build`

---

## Próximos Passos

1. **Documentar Endpoints Restantes**: Adicionar XML comments aos demais endpoints seguindo o padrão de `SearchCategoriesEndpoint.cs`

2. **Testar Autenticação no Scalar**:
   - Obter token via `./test-openiddict-login.sh`
   - Autenticar no Scalar usando o token
   - Testar endpoints protegidos

3. **Personalizar Scalar**:
   - Ajustar cores do tema se necessário
   - Adicionar exemplos de requisições personalizados
   - Configurar rate limiting na documentação

4. **Integração com CI/CD**:
   - Adicionar validação de XML comments no pipeline
   - Gerar documentação OpenAPI no build
   - Publicar documentação em ambiente de staging

---

## Referências

- **Guia de Uso Scalar**: `/docs/GUIA-USO-SCALAR.md`
- **Documentação Oficial Scalar**: https://github.com/scalar/scalar
- **ASP.NET Core Middleware Pipeline**: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/
- **OpenAPI Specification**: https://swagger.io/specification/

---

**Data**: 2025-01-19
**Versão**: 1.0.0
**Status**: ✅ Resolvido
