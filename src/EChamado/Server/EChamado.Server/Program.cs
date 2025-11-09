using EChamado.Server.Configuration;
using EChamado.Server.Endpoints;
using EChamado.Server.Infrastructure.Configuration;
using EChamado.Server.Middlewares;

var builder = WebApplication.CreateBuilder(args);

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

// Configuração MediatR
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(EChamado.Server.Application.UseCases.Orders.Commands.CreateOrderCommand).Assembly);
});

// Health Checks
builder.Services.AddHealthCheckConfiguration(builder.Configuration);

builder.Services.AddControllers();

var app = builder.Build();

// Inicializa o banco de dados (migrations + seed)
await DatabaseInitializer.InitializeDatabaseAsync(app.Services);

// Usar CORS antes de Routing
app.UseCors("AllowBlazorClient");

// Logging Middlewares
app.UseRequestLogging();
app.UsePerformanceLogging(slowRequestThresholdMs: 3000);

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Health Checks
app.UseHealthCheckConfiguration();

// Mapear todos os endpoints (incluindo SubCategories)
app.MapEndpoints();

app.MapControllers();

app.Run();

public partial class Program { }
