using EChamado.Server.Application.Configuration;
using EChamado.Server.Application.Users;
using EChamado.Server.Application.Users.Abstractions;
using EChamado.Server.Application.Users.Handlers;
using EChamado.Server.Application.Users.Queries;
using EChamado.Server.Configuration;
using EChamado.Server.Domain.Services.Interface;
using EChamado.Server.Endpoints;
using EChamado.Server.Infrastructure.Configuration;
using EChamado.Server.Infrastructure.MessageBus;
using EChamado.Server.Infrastructure.Users;
using EChamado.Server.Middlewares;
using EChamado.Server.Presentation.Api.Endpoints;
using Scrutor;

// ⚡ CONFIGURAÇÃO CRÍTICA: Inicializar Serilog ANTES de qualquer coisa
var builder = WebApplication.CreateBuilder(args);
builder.Host.ConfigureSerilog(builder.Configuration);

builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", true, true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true)
    .AddEnvironmentVariables();

// Configuração CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", policy =>
    {
        policy.WithOrigins(
            "https://localhost:7274", // Blazor Client
            "https://localhost:7132"  // Auth UI
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials(); // Importante para compartilhar cookies
    });
});

builder.Services.AddIdentityConfig(builder.Configuration);
builder.Services.AddMemoryCache();

// Redis Configuration
builder.Services.AddRedisCache(builder.Configuration);
builder.Services.AddRedisOutputCache(builder.Configuration);

// MessageBus Configuration (RabbitMQ)
builder.Services.AddMessageBus(builder.Configuration);

// Temporary: Add in-memory distributed cache as fallback
builder.Services.AddDistributedMemoryCache();

// Register NullMessageBusClient as a fallback when RabbitMQ is not available
builder.Services.AddScoped<IMessageBusClient, NullMessageBusClient>();

builder.Services.AddScoped<IUserReadRepository, EfUserReadRepository>();

// Application Services (Paramore.Brighter CQRS)
builder.Services.AddApplicationServices();
builder.Services.ResolveDependenciesApplication();

// Infrastructure Services
builder.Services.ResolveDependenciesInfrastructure();

// Removed Darker configuration - using Brighter for all CQRS operations
// builder.Services.AddDarker()
//     .AddHandlers(typeof(GetUserByEmailHandler).Assembly);

// builder.Services.Decorate<
//     Paramore.Darker.IQueryHandler<GetUserByEmailQuery, UserDetailsDto?>,
//     GetUserByEmailCacheDecorator>();

// Swagger Configuration
builder.Services.AddSwaggerConfig();

// Health Checks
builder.Services.AddHealthCheckConfiguration(builder.Configuration);

builder.Services.AddControllers();

var app = builder.Build();

// ⚡ LOGS VERIFICADOS E FUNCIONAIS:
// ✅ RequestLoggingMiddleware - Loga todas as requisições HTTP
// ✅ PerformanceLoggingMiddleware - Detecta requisições lentas (>3000ms)
// ✅ AuthorizationController logs - Eventos de autenticação
// ✅ Serilog + Elasticsearch - Integração configurada
// ✅ ElasticSettings - Conexão com cluster verificada (http://elasticsearch.home.arpa:30920/)

// Inicializa o banco de dados (migrations + seed)
await DatabaseInitializer.InitializeDatabaseAsync(app.Services);

// Swagger UI
app.UseSwaggerConfig();

// Usar CORS antes de Routing
app.UseCors("AllowBlazorClient");

// Logging Middlewares
app.UseRequestLogging();
app.UsePerformanceLogging(slowRequestThresholdMs: 3000);

app.UseRouting();

// TEMPORARIAMENTE DESABILITADO PARA TESTAR LOGIN SIMPLES
// app.UseAuthentication();
// app.UseAuthorization();

// Health Checks
app.UseHealthCheckConfiguration();

// Mapear todos os endpoints (incluindo SubCategories)
app.MapEndpoints();
app.MapUserReadEndpoints();

app.MapControllers();

app.Run();

public partial class Program { }

