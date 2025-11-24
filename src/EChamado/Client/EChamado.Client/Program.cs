using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components.Authorization;
using EChamado.Client.Services;
using EChamado.Client.Authentication;
using MudBlazor.Services;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace EChamado.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                Console.WriteLine("=== Starting EChamado Client ===");

                var builder = WebAssemblyHostBuilder.CreateDefault(args);
                builder.RootComponents.Add<App>("#app");

                // Services configuration
                builder.Services.AddMudServices();

                var authServerUrl = builder.Configuration["AuthServerUrl"] ?? "https://localhost:7133";
                var backendUrl = builder.Configuration["BackendUrl"] ?? "https://localhost:7296";

                Console.WriteLine($"Auth Server URL: {authServerUrl}");
                Console.WriteLine($"Backend URL: {backendUrl}");

                // HTTP Clients
                builder.Services.AddScoped(sp => new HttpClient
                {
                    BaseAddress = new Uri(authServerUrl),
                    Timeout = TimeSpan.FromSeconds(30)
                });

                // Authentication
                builder.Services.AddAuthorizationCore();
                builder.Services.AddScoped<AuthenticationStateProvider, CookieAuthenticationStateProvider>();

                // API Clients
                builder.Services.AddHttpClient<OrderService>(client =>
                {
                    client.BaseAddress = new Uri(backendUrl);
                    client.Timeout = TimeSpan.FromSeconds(30);
                });

                builder.Services.AddHttpClient<CategoryService>(client =>
                {
                    client.BaseAddress = new Uri(backendUrl);
                    client.Timeout = TimeSpan.FromSeconds(30);
                });

                builder.Services.AddHttpClient<DepartmentService>(client =>
                {
                    client.BaseAddress = new Uri(backendUrl);
                    client.Timeout = TimeSpan.FromSeconds(30);
                });

                builder.Services.AddHttpClient<LookupService>(client =>
                {
                    client.BaseAddress = new Uri(backendUrl);
                    client.Timeout = TimeSpan.FromSeconds(30);
                });

                builder.Services.AddHttpClient<CommentService>(client =>
                {
                    client.BaseAddress = new Uri(backendUrl);
                    client.Timeout = TimeSpan.FromSeconds(30);
                });

                Console.WriteLine("Building client host...");
                var host = builder.Build();
                
                Console.WriteLine("Starting client application...");
                await host.RunAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fatal error starting Client: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }
    }
}