using EChamado.Server.Application.Configuration;
using EChamado.Server.Application.Users.Abstractions;
using EChamado.Server.Domain.Domains.Identities;
using EChamado.Server.Domain.Services.Interface;
using EChamado.Server.Infrastructure.Configuration;
using EChamado.Server.Infrastructure.MessageBus;
using EChamado.Server.Infrastructure.Persistence;
using EChamado.Server.Infrastructure.Users;
using EChamado.Shared.Shared.Settings;
using Echamado.Auth.Components;
using Echamado.Auth;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using MudBlazor.Services;
using OpenIddict.Server.AspNetCore;
using System.Text;
using OpenIddict.Core;
using OpenIddict.EntityFrameworkCore;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

try
{
    Console.WriteLine("=== Starting EChamado Auth Server ===");

    var builder = WebApplication.CreateBuilder(args);
    builder.Configuration
        .SetBasePath(builder.Environment.ContentRootPath)
        .AddJsonFile("appsettings.json", true, true)
        .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true)
        .AddEnvironmentVariables();

    // Core services
    builder.Services.AddMudServices();
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy.WithOrigins("https://localhost:5199", "https://localhost:7274", 
                              "http://localhost:5199", "http://localhost:7274",
                              "https://localhost:7133", "http://localhost:5137")
                  .AllowAnyMethod().AllowAnyHeader().AllowCredentials();
        });
    });

    builder.Services.AddControllers();
    builder.Services.AddRazorComponents().AddInteractiveServerComponents();

    // Database & Identity
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
                         b => b.MigrationsAssembly("Echamado.Auth")));

    builder.Services.AddMemoryCache();
    builder.Services.AddDistributedMemoryCache();
    builder.Services.AddScoped<IMessageBusClient, NullMessageBusClient>();
    builder.Services.AddScoped<IUserReadRepository, EfUserReadRepository>();

    // Application & Infrastructure
    builder.Services.AddApplicationServices();
    builder.Services.ResolveDependenciesApplication();
    builder.Services.ResolveDependenciesInfrastructure();

    // Data Protection
    var keysPath = Path.Combine(Path.GetTempPath(), "EChamado-DataProtection-Keys");
    Directory.CreateDirectory(keysPath);
    builder.Services.AddDataProtection()
        .PersistKeysToFileSystem(new DirectoryInfo(keysPath))
        .SetApplicationName("EChamado");

    // Identity & Authentication
    builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequireUppercase = true;
        options.Password.RequiredLength = 12;
        options.Password.RequiredUniqueChars = 4;
        options.SignIn.RequireConfirmedAccount = false;
        options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%&*()_=?. ";
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

    builder.Services.AddAuthentication().AddCookie("External", options =>
    {
        options.Cookie.Name = "EChamado.External";
        options.Cookie.SameSite = SameSiteMode.None;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
        options.LoginPath = "/Account/Login";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
    });

    builder.Services.ConfigureApplicationCookie(options =>
    {
        options.Cookie.Name = "EChamado.External";
        options.Cookie.SameSite = SameSiteMode.None;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
        options.LoginPath = "/Account/Login";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
    });

    // Rate Limiting Configuration
    builder.Services.AddRateLimiter(options =>
    {
        options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

        // Global limiter
        options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        {
            var username = context.User?.Identity?.Name ?? context.Connection.RemoteIpAddress?.ToString() ?? "anonymous";
            return RateLimitPartition.GetFixedWindowLimiter(username, _ => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1)
            });
        });

        // Login endpoint - proteção contra brute force
        options.AddPolicy("login", context =>
        {
            var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "anonymous";
            return RateLimitPartition.GetFixedWindowLimiter(ipAddress, _ => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1)
            });
        });
    });

    // OpenIddict Configuration
    var appSettingsSection = builder.Configuration.GetSection("AppSettings");
    builder.Services.Configure<AppSettings>(appSettingsSection);
    var appSettings = appSettingsSection.Get<AppSettings>();
    if (appSettings == null) throw new ApplicationException("AppSettings not found");

    var key = Encoding.ASCII.GetBytes(appSettings.Secret);

    builder.Services.AddOpenIddict()
        .AddCore(options => options.UseEntityFrameworkCore().UseDbContext<ApplicationDbContext>())
        .AddServer(options =>
        {
            // Endpoints habilitados
            options.SetAuthorizationEndpointUris("/connect/authorize"); // ✅ ADICIONADO para Authorization Code Flow
            options.SetTokenEndpointUris("/connect/token");
            options.SetIntrospectionEndpointUris("/connect/introspect"); // ✅ CRÍTICO para validação de tokens

            // Grant types permitidos
            options.AllowAuthorizationCodeFlow() // ✅ ADICIONADO para Authorization Code + PKCE
                   .AllowPasswordFlow()
                   .AllowClientCredentialsFlow()
                   .AllowRefreshTokenFlow();

            // Scopes registrados
            options.RegisterScopes("openid", "profile", "email", "roles", "api", "chamados");

            // Certificados e chaves
            options.AddDevelopmentSigningCertificate();
            options.AddEphemeralEncryptionKey();

            // IMPORTANTE: Desabilita criptografia do access_token para permitir parse local no cliente
            // Com isso, o access_token será JWT assinado (não JWE criptografado)
            options.DisableAccessTokenEncryption();

            // Configuração ASP.NET Core
            options.UseAspNetCore()
                   .EnableAuthorizationEndpointPassthrough() // ✅ ADICIONADO para permitir processamento customizado
                   .EnableTokenEndpointPassthrough()
                   .DisableTransportSecurityRequirement();
        })
        .AddValidation(options =>
        {
            options.UseLocalServer();
            options.UseSystemNetHttp();
            options.UseAspNetCore();
        });

    builder.Services.AddHostedService<OpenIddictWorker>();
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddScoped<HttpClient>(sp =>
    {
        var navMan = sp.GetRequiredService<NavigationManager>();
        return new HttpClient { BaseAddress = new Uri(navMan.BaseUri) };
    });

    var app = builder.Build();

    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error", createScopeForErrors: true);
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseRouting();
    app.UseCors();
    app.UseRateLimiter();
    app.UseAntiforgery();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapStaticAssets();
    app.MapControllers();
    app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"Fatal error starting Auth Server: {ex.Message}");
    Console.WriteLine($"Stack trace: {ex.StackTrace}");
    return 1;
}

return 0;

public partial class Program { }