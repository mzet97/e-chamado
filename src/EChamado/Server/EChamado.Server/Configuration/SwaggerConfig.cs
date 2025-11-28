// ============================================================================
// ESTE ARQUIVO FOI SUBSTITUÍDO POR ScalarConfig.cs
// ============================================================================
// Este arquivo é mantido apenas para referência e compatibilidade com
// ApiConfig.cs (que não está sendo usado no Program.cs atual).
//
// A documentação da API agora usa SCALAR ao invés de Swagger UI.
// Veja: ScalarConfig.cs para a configuração atual.
//
// IMPORTANTE: Os Operation Filters abaixo (GridifyExamplesOperationFilter
// e ODataExamplesOperationFilter) são ainda úteis e estão documentados aqui
// para referência futura caso precise adicionar ao ScalarConfig.
// ============================================================================

using EChamado.Server.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace EChamado.Server.Configuration;

/// <summary>
/// [OBSOLETO] Configuração antiga do Swagger UI.
/// Use ScalarConfig.cs para a configuração atual da documentação.
/// </summary>
[Obsolete("Use ScalarConfig.AddApiDocumentation() ao invés deste método. Swagger UI foi substituído por Scalar.")]
public static class SwaggerConfig
{
    public static IServiceCollection AddSwaggerConfig(this IServiceCollection services)
    {

        services.AddHttpContextAccessor();

        services.AddExceptionHandler<CustomExceptionHandler>();

        services.Configure<ApiBehaviorOptions>(options =>
            options.SuppressModelStateInvalidFilter = true);

        services.AddSwaggerGen(c =>
        {
            c.OperationFilter<SwaggerDefaultValues>();
            c.OperationFilter<GridifyExamplesOperationFilter>();
            c.OperationFilter<ODataExamplesOperationFilter>();

            // Inclui comentários XML na documentação
            var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath);
            }

            c.AddSecurityDefinition("jwt_auth", new OpenApiSecurityScheme
            {
                Description = "Insert the JWT token as follows: Bearer {your token}",
                Name = "Authorization",
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "jwt_auth"
                        }
                    },
                    new string[] {}
                }
            });
        });

        return services;
    }

    [Obsolete("Use ScalarConfig.UseApiDocumentation() ao invés deste método. Swagger UI foi substituído por Scalar.")]
    public static IApplicationBuilder UseSwaggerConfig(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        return app;
    }
}

public class SwaggerDefaultValues : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.Parameters == null)
        {
            return;
        }

        foreach (var parameter in operation.Parameters)
        {
            var description = context.ApiDescription
                .ParameterDescriptions
                .First(p => p.Name == parameter.Name);

            var routeInfo = description.RouteInfo;

            operation.Deprecated = OpenApiOperation.DeprecatedDefault;

            if (parameter.Description == null)
            {
                parameter.Description = description.ModelMetadata?.Description;
            }

            if (routeInfo == null)
            {
                continue;
            }

            if (parameter.In != ParameterLocation.Path && parameter.Schema.Default == null)
            {
                parameter.Schema.Default = new OpenApiString(routeInfo.DefaultValue.ToString());
            }

            parameter.Required |= !routeInfo.IsOptional;
        }
    }
}

public class ODataExamplesOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // Detecta se é um endpoint OData
        if (context.ApiDescription.RelativePath?.StartsWith("odata/") != true)
            return;

        var entityName = context.ApiDescription.RelativePath.Split('/')[1];

        operation.Description = $@"
<b>Endpoint OData para {entityName}</b>

<b>Exemplos de queries OData:</b>

1. Filtrar ($filter):
   odata/{entityName}?$filter=Name eq 'TI'
   odata/{entityName}?$filter=contains(Name, 'Suporte')
   odata/{entityName}?$filter=CreatedAt ge 2024-01-01

2. Ordenar ($orderby):
   odata/{entityName}?$orderby=Name asc
   odata/{entityName}?$orderby=CreatedAt desc

3. Paginação ($top, $skip):
   odata/{entityName}?$top=20&$skip=0
   odata/{entityName}?$top=10&$skip=20

4. Selecionar campos ($select):
   odata/{entityName}?$select=Id,Name,CreatedAt

5. Expandir relações ($expand):
   odata/{entityName}?$expand=Status,Type,Category

6. Contar ($count):
   odata/{entityName}/$count
   odata/{entityName}?$count=true

7. Combinações:
   odata/{entityName}?$filter=IsDeleted eq false&$orderby=CreatedAt desc&$top=50&$select=Id,Name,CreatedAt

<b>Operadores de filtro disponíveis:</b>
- Comparação: eq (igual), ne (diferente), gt (maior), ge (maior ou igual), lt (menor), le (menor ou igual)
- Lógicos: and, or, not
- Funções: contains, startswith, endswith, length, tolower, toupper
- Data/Hora: year, month, day, hour, minute, second

<b>Limites de segurança:</b>
- $top: máximo 100 itens por requisição
- Profundidade de expansão: máximo 5 níveis";
    }
}

public class GridifyExamplesOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.Parameters == null)
            return;

        // Detecta se é um endpoint Gridify pela presença dos parâmetros característicos
        var hasGridifyParams = operation.Parameters.Any(p =>
            p.Name == "Page" || p.Name == "PageSize" || p.Name == "Filter" || p.Name == "OrderBy");

        if (!hasGridifyParams)
            return;

        // Adiciona descrições detalhadas e exemplos para cada parâmetro Gridify
        foreach (var parameter in operation.Parameters)
        {
            switch (parameter.Name)
            {
                case "Page":
                    parameter.Description = "Número da página (1-10000). Exemplo: 1";
                    parameter.Schema.Default = new OpenApiInteger(1);
                    parameter.Example = new OpenApiInteger(1);
                    break;

                case "PageSize":
                    parameter.Description = "Quantidade de itens por página (1-100). Exemplo: 10";
                    parameter.Schema.Default = new OpenApiInteger(10);
                    parameter.Example = new OpenApiInteger(20);
                    break;

                case "Filter":
                    parameter.Description = @"Filtro Gridify (máx 500 caracteres).
Exemplos:
- Title @= ""Suporte"" (contém)
- Name == ""Hardware"" (igual)
- CreatedAt >= 2024-01-01 (data maior ou igual)
- IsDeleted == false (booleano)
- StatusId == guid (GUID)
Operadores: ==, !=, >, >=, <, <=, @= (contém), @=* (começa com), @=^ (termina com)
Combinações: && (AND), || (OR), () (agrupamento)";
                    parameter.Example = new OpenApiString("Title @= \"Suporte\" && IsDeleted == false");
                    break;

                case "OrderBy":
                    parameter.Description = @"Ordenação Gridify (máx 200 caracteres).
Exemplos:
- CreatedAt desc (decrescente)
- Name asc (crescente)
- CreatedAt desc, Title asc (múltiplos campos)";
                    parameter.Example = new OpenApiString("CreatedAt desc");
                    break;
            }
        }

        // Adiciona exemplos de resposta
        if (context.ApiDescription.RelativePath?.Contains("gridify") == true)
        {
            operation.Summary ??= "Endpoint Gridify com filtros dinâmicos, ordenação e paginação";

            var entityName = context.ApiDescription.RelativePath.Split('/')[1];
            operation.Description = $@"
<b>Endpoint Gridify para {entityName}</b>

<b>Exemplos de uso:</b>

1. Paginação simples:
   ?Page=1&PageSize=20

2. Filtro por texto:
   ?Filter=Name @= ""TI""&Page=1&PageSize=10

3. Filtro com data:
   ?Filter=CreatedAt >= 2024-01-01&OrderBy=CreatedAt desc

4. Filtros combinados:
   ?Filter=(Name @= ""Suporte"" || Name @= ""Hardware"") && IsDeleted == false&OrderBy=Name asc&Page=1&PageSize=50

<b>Campos disponíveis para filtro/ordenação:</b>
- Id (Guid)
- Name (string) - quando aplicável
- Title (string) - quando aplicável
- Description (string)
- CreatedAt (DateTime)
- UpdatedAt (DateTime?)
- IsDeleted (bool)

<b>Validações:</b>
- Page: 1 a 10.000
- PageSize: 1 a 100
- Filter: máximo 500 caracteres
- OrderBy: máximo 200 caracteres";
        }
    }
}

public class SwaggerAuthorizedMiddleware
{
    private readonly RequestDelegate _next;

    public SwaggerAuthorizedMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/swagger")
            && !context.User.Identity.IsAuthenticated)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        await _next.Invoke(context);
    }
}
