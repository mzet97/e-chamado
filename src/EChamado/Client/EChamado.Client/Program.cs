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
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddMudServices();

            // URLs de configuração
            var authServerUrl = builder.Configuration["AuthServerUrl"] ?? "https://localhost:7132";
            var backendUrl = builder.Configuration["BackendUrl"] ?? "https://localhost:7296";

            // HttpClient principal (para o Auth Server)
            builder.Services.AddScoped(sp => new HttpClient
            {
                BaseAddress = new Uri(authServerUrl)
            });

            // Autenticação customizada baseada em cookie
            builder.Services.AddAuthorizationCore();
            builder.Services.AddScoped<AuthenticationStateProvider, CookieAuthenticationStateProvider>();

            // HttpClients para consumir a API
            builder.Services.AddHttpClient<OrderService>(client =>
            {
                client.BaseAddress = new Uri(backendUrl!);
            });

            builder.Services.AddHttpClient<CategoryService>(client =>
            {
                client.BaseAddress = new Uri(backendUrl!);
            });

            builder.Services.AddHttpClient<DepartmentService>(client =>
            {
                client.BaseAddress = new Uri(backendUrl!);
            });

            builder.Services.AddHttpClient<LookupService>(client =>
            {
                client.BaseAddress = new Uri(backendUrl!);
            });

            builder.Services.AddHttpClient<CommentService>(client =>
            {
                client.BaseAddress = new Uri(backendUrl!);
            });

            await builder.Build().RunAsync();
        }
    }
}
