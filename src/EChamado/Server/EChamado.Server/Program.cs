using EChamado.Server.Configuration;
using EChamado.Server.Endpoints;
using EChamado.Server.Infrastructure.Configuration;
using EChamado.Server.Services;
using Serilog;
using OpenIddict.Server.AspNetCore;
using EChamado.Server.Infrastructure.Persistence;

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Configuration
       .SetBasePath(builder.Environment.ContentRootPath)
       .AddJsonFile("appsettings.json", true, true)
       .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true)
       .AddEnvironmentVariables();

    builder.Services.AddRedisCache(builder.Configuration);
    builder.Services.AddRedisOutputCache(builder.Configuration);
    builder.Host.ConfigureSerilog(builder.Configuration);
    builder.Logging.ClearProviders();
    builder.Logging.AddSerilog();

    builder.Services.AddApiConfig(builder.Configuration);

    builder.Services.AddOpenIddict()
        .AddCore(options =>
        {
            options.UseEntityFrameworkCore()
                   .UseDbContext<ApplicationDbContext>();
        })
        .AddServer(options =>
        {
            options.SetAuthorizationEndpointUris("/connect/authorize")
                   .SetTokenEndpointUris("/connect/token");

            options.AllowAuthorizationCodeFlow().RequireProofKeyForCodeExchange();
            options.AllowRefreshTokenFlow();

            options.AddDevelopmentEncryptionCertificate();
            options.AddDevelopmentSigningCertificate();

            options.UseAspNetCore()
                   .EnableAuthorizationEndpointPassthrough()
                   .EnableTokenEndpointPassthrough();
        });

    // Registrar o seeder
    builder.Services.AddScoped<OpenIddictClientSeeder>();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseCors(builder => builder
            .WithOrigins("https://localhost:7274", "https://localhost:7132")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
        app.UseDeveloperExceptionPage();
    }
    else
    {
        app.UseCors("Production");
        app.UseHsts();
    }

    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseAppConfig();
    app.MapControllers();
    app.MapEndpoints();

    app.UseSwaggerConfig();

    // Seed OpenIddict clients
    using (var scope = app.Services.CreateScope())
    {
        var seeder = scope.ServiceProvider.GetRequiredService<OpenIddictClientSeeder>();
        await seeder.SeedAsync();
    }

    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}

public partial class Program { }