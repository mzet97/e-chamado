using MudBlazor;

namespace EChamado.Client;

public static class Configuration
{
    public const string HttpClientName = "EChamado";

    public static string BackendUrl { get; set; } = "https://localhost:7296";

    public static MudTheme Theme = new()
    {
        Typography = new Typography
        {
            Default = new Default
            {
                FontFamily = ["Inter", "sans-serif"]
            }
        }
    };
}
