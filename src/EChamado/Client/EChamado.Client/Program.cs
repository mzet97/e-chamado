using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Services;
using EChamado.Client.Application.Configuration;
using EChamado.Client.Services;

namespace EChamado.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<EChamado.Client.App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            // Configura HttpClient normal
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            // Adicionar serviços MudBlazor
            builder.Services.AddMudServices();

            // Configuração OIDC (Authorization Code + PKCE)
            builder.Services.AddOidcAuthentication(options =>
            {
                // Carrega as configurações de appsettings.json
                builder.Configuration.Bind("oidc", options.ProviderOptions);

                // Garante os escopos necessários
                options.ProviderOptions.DefaultScopes.Clear();
                var scopes = builder.Configuration.GetSection("oidc:DefaultScopes").Get<string[]>();
                if (scopes != null)
                {
                    foreach (var scope in scopes)
                    {
                        options.ProviderOptions.DefaultScopes.Add(scope);
                    }
                }
            });

            // HttpClient que usa tokens para chamadas autenticadas
            builder.Services.AddHttpClient<ChamadoService>(client =>
            {
                var backendUrl = builder.Configuration["BackendUrl"] ?? "https://localhost:7296";
                client.BaseAddress = new Uri(backendUrl);
            })
            .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

            // O AuthenticationStateProvider já é registrado automaticamente pelo AddOidcAuthentication

            // Resolver dependências da aplicação
            builder.Services.ResolveDependenciesApplication();

            await builder.Build().RunAsync();
        }
    }
}
