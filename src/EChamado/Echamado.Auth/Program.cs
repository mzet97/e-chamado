using MudBlazor.Services;
using Echamado.Auth.Components;
using EChamado.Server.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using EChamado.Server.Domain.Domains.Identities;
using Microsoft.AspNetCore.Identity;
using Echamado.Auth.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Components;

var builder = WebApplication.CreateBuilder(args);

// Add MudBlazor services
builder.Services.AddMudServices();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Configuração Data Protection (compartilhada com EChamado.Server)
var keysPath = Path.Combine(Path.GetTempPath(), "EChamado-DataProtection-Keys");
Directory.CreateDirectory(keysPath);

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(keysPath))
    .SetApplicationName("EChamado");

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    options.SignIn.RequireConfirmedAccount = false; // Alterado para false para facilitar testes

    options.User.AllowedUserNameCharacters =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%&*()_=?. ";
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Configuração de autenticação com cookie "External" (compartilhado com EChamado.Server)
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = "EChamado.External";
    options.Cookie.SameSite = SameSiteMode.None; // Permitir compartilhamento entre diferentes portas
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.LoginPath = "/Account/Login";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.SlidingExpiration = true;
});

// Importante para Blazor acessar HttpContext
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<HttpClient>(sp =>
{
    var navMan = sp.GetRequiredService<NavigationManager>();
    return new HttpClient
    {
        BaseAddress = new Uri(navMan.BaseUri)
    };
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

app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();



app.Run();
