using EChamado.Server.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace EChamado.Server.Configuration;

public static class ScalarConfig
{
    public static IServiceCollection AddApiDocumentation(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddExceptionHandler<CustomExceptionHandler>();

        services.Configure<ApiBehaviorOptions>(options =>
            options.SuppressModelStateInvalidFilter = true);

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "EChamado API",
                Version = "v1.0.0",
                Description = @"
# EChamado - Sistema de Gerenciamento de Chamados

API RESTful completa para gerenciamento de tickets/chamados com autenticação OAuth 2.0 / OpenID Connect.

## 🚀 Funcionalidades

- **Autenticação**: OAuth 2.0 com OpenIddict (Authorization Code + PKCE, Password Grant, Client Credentials)
- **Gestão de Chamados**: CRUD completo com filtros avançados, paginação e pesquisa
- **Categorização**: Categorias, Subcategorias e Departamentos
- **Workflow**: Tipos de pedido e tipos de status customizáveis
- **Comentários**: Sistema de comentários nos chamados
- **Usuários e Roles**: Gerenciamento completo de usuários e permissões

## 🔐 Autenticação

Esta API usa **Bearer Token** (JWT) para autenticação. Para obter um token:

### Opção 1: Password Grant (Apps Mobile/Desktop/Scripts)
```bash
curl -X POST https://localhost:7133/connect/token \
  -H ""Content-Type: application/x-www-form-urlencoded"" \
  -d ""grant_type=password"" \
  -d ""username=admin@admin.com"" \
  -d ""password=Admin@123"" \
  -d ""client_id=mobile-client"" \
  -d ""scope=openid profile email roles api chamados""
```

### Opção 2: Use o botão 'Authorize' acima
1. Clique no botão **Authorize** 🔓
2. Obtenha um token usando o comando acima
3. Cole o **access_token** no campo
4. Clique em **Authorize**

## 📚 Documentação Adicional

- **Guia Completo**: Ver arquivo `CLAUDE.md` na raiz do projeto
- **Autenticação**: Ver `docs/AUTENTICACAO-SISTEMAS-EXTERNOS.md`
- **Exemplos**: Ver `docs/exemplos-autenticacao-openiddict.md`
- **Scripts de Teste**: `test-openiddict-login.sh`, `.ps1`, `.py` na raiz

## ⚙️ Configuração

- **Auth Server**: https://localhost:7133
- **API Server**: https://localhost:7296
- **Client App**: https://localhost:7274

## 👥 Usuários Padrão

- **Admin**: admin@admin.com / Admin@123
- **User**: user@echamado.com / User@123
",
                Contact = new OpenApiContact
                {
                    Name = "EChamado Team",
                    Email = "support@echamado.com",
                    Url = new Uri("https://github.com/mzet97/e-chamado")
                },
                License = new OpenApiLicense
                {
                    Name = "MIT License",
                    Url = new Uri("https://opensource.org/licenses/MIT")
                }
            });

            // Configuração de segurança para Bearer Token (OpenIddict)
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = @"
**Autenticação JWT usando OpenIddict**

Entre com o token JWT no formato: `Bearer {seu_token}`

**Como obter o token:**

1. **Via cURL (Password Grant):**
```bash
curl -X POST https://localhost:7133/connect/token \
  -H 'Content-Type: application/x-www-form-urlencoded' \
  -d 'grant_type=password' \
  -d 'username=admin@admin.com' \
  -d 'password=Admin@123' \
  -d 'client_id=mobile-client' \
  -d 'scope=openid profile email roles api chamados'
```

2. **Via Scripts de Teste:**
   - Bash: `./test-openiddict-login.sh`
   - PowerShell: `.\test-openiddict-login.ps1`
   - Python: `python test-openiddict-login.py`

3. Cole aqui **apenas o access_token** (não inclua 'Bearer')

**Documentação:** Ver `docs/AUTENTICACAO-SISTEMAS-EXTERNOS.md`
"
            });

            // Configuração de segurança para OAuth2 (OpenIddict)
            c.AddSecurityDefinition("OAuth2", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Description = "OAuth 2.0 Authorization Code Flow with PKCE (para SPAs) ou Password Flow (para apps)",
                Flows = new OpenApiOAuthFlows
                {
                    // Password Flow (para mobile/desktop/scripts)
                    Password = new OpenApiOAuthFlow
                    {
                        TokenUrl = new Uri("https://localhost:7133/connect/token"),
                        Scopes = new Dictionary<string, string>
                        {
                            { "openid", "OpenID Connect" },
                            { "profile", "Perfil do usuário" },
                            { "email", "Email do usuário" },
                            { "roles", "Roles/Permissões" },
                            { "api", "Acesso à API" },
                            { "chamados", "Acesso aos chamados" }
                        }
                    },
                    // Authorization Code Flow (para SPAs como Blazor)
                    AuthorizationCode = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri("https://localhost:7133/connect/authorize"),
                        TokenUrl = new Uri("https://localhost:7133/connect/token"),
                        Scopes = new Dictionary<string, string>
                        {
                            { "openid", "OpenID Connect" },
                            { "profile", "Perfil do usuário" },
                            { "email", "Email do usuário" },
                            { "roles", "Roles/Permissões" },
                            { "api", "Acesso à API" },
                            { "chamados", "Acesso aos chamados" }
                        }
                    }
                }
            });

            // Aplicar segurança globalmente
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });

            // Incluir XML comments
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
            }

            // Operation filter para valores padrão
            c.OperationFilter<SwaggerDefaultValues>();

            // Tags customizadas - agrupa por prefixo da rota
            c.TagActionsBy(api =>
            {
                // Se a API tem GroupName definido, use-o
                if (api.GroupName != null)
                {
                    return new[] { api.GroupName };
                }

                // Para Minimal APIs, extrair a tag do route pattern
                var routePattern = api.RelativePath?.ToLowerInvariant() ?? "";

                // Verificar se é rota de versão (v1, v2, etc)
                if (routePattern.StartsWith("v1/"))
                {
                    var segments = routePattern.Split('/');
                    if (segments.Length >= 2)
                    {
                        var entitySegment = segments[1]; // Ex: "category", "categories"

                        // Normalizar para singular e capitalizar
                        var tagName = NormalizeTagName(entitySegment);
                        return new[] { tagName };
                    }
                }

                // Fallback para controller-based APIs
                if (api.ActionDescriptor.RouteValues.TryGetValue("controller", out var controllerName))
                {
                    return new[] { controllerName };
                }

                return new[] { "Default" };
            });

            // Ordenar tags alfabeticamente
            c.OrderActionsBy(api => $"{api.GroupName}_{api.HttpMethod}_{api.RelativePath}");

            c.DocInclusionPredicate((name, api) => true);
        });

        return services;
    }

    private static string NormalizeTagName(string entitySegment)
    {
        // Remove 's' do plural se existir
        var singular = entitySegment.TrimEnd('s');

        // Capitalizar primeira letra
        if (string.IsNullOrEmpty(singular))
            return "Default";

        return char.ToUpperInvariant(singular[0]) + singular.Substring(1);
    }

    public static WebApplication UseApiDocumentation(this WebApplication app)
    {
        app.UseSwagger(options =>
        {
            options.RouteTemplate = "openapi/{documentName}.json";
        });

        // Configuração do Scalar - UI moderna para documentação
        app.MapScalarApiReference(options =>
        {
            options
                .WithTitle("EChamado API Documentation")
                .WithTheme(ScalarTheme.Purple)
                .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient)
                .WithSidebar(true)
                .WithModels(true)
                .WithDarkMode(true)
                .WithSearchHotKey("k")
                .WithOpenApiRoutePattern("/openapi/{documentName}.json")
                .WithEndpointPrefix("/api-docs/{documentName}")
                .WithProxyUrl("https://proxy.scalar.com")
                .WithHttpBearerAuthentication(x =>
                {
                    x.Token = "your-bearer-token-here";
                })
                .WithOAuth2Authentication(x =>
                {
                    x.ClientId = "mobile-client";
                    x.Scopes = ["openid", "profile", "email", "roles", "api", "chamados"];
                })
                .WithFavicon("/favicon.ico")
                .WithCdnUrl("https://cdn.jsdelivr.net/npm/@scalar/api-reference");
        });

        return app;
    }
}
