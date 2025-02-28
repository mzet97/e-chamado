using EChamado.Client;
using EChamado.Client.Application.Configuration;
using EChamado.Client.Security;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

Configuration.BackendUrl = builder.Configuration.GetValue<string>("BackendUrl") ?? string.Empty;

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices();

builder.Services
    .AddHttpClient(Configuration.HttpClientName, opt => { opt.BaseAddress = new Uri(Configuration.BackendUrl); })
    .AddHttpMessageHandler<CookieHandler>();

builder.Services.AddOidcAuthentication(options =>
{
    builder.Configuration.Bind("OidcConfiguration", options.ProviderOptions);
});

builder.Services.ResolveDependenciesApplication();

await builder.Build().RunAsync();
