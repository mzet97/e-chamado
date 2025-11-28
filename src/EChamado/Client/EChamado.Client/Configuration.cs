using MudBlazor;

namespace EChamado.Client;

public static class Configuration
{
    public const string HttpClientName = "EChamado";

    public static string BackendUrl { get; set; } = "https://localhost:7296";

    public static MudTheme Theme = new()
    {
        PaletteLight = new PaletteLight
        {
            Primary = "#2563EB", // Azul moderno (Tailwind Blue 600)
            Secondary = "#4F46E5", // Indigo (Tailwind Indigo 600)
            AppbarBackground = "#2563EB",
            Background = "#F3F4F6", // Cinza muito claro para fundo
            DrawerBackground = "#FFFFFF",
            Surface = "#FFFFFF",
            TextPrimary = "#111827", // Cinza quase preto
            TextSecondary = "#4B5563", // Cinza médio
            Success = "#10B981",
            Warning = "#F59E0B",
            Error = "#EF4444",
            Info = "#3B82F6"
        },
        PaletteDark = new PaletteDark
        {
            Primary = "#3B82F6", // Azul mais claro para contraste
            Secondary = "#6366F1",
            AppbarBackground = "#1F2937", // Cinza escuro (Gray 800)
            Background = "#111827", // Gray 900
            DrawerBackground = "#1F2937",
            Surface = "#1F2937",
            TextPrimary = "#F9FAFB",
            TextSecondary = "#9CA3AF",
            Success = "#34D399",
            Warning = "#FBBF24",
            Error = "#F87171",
            Info = "#60A5FA"
        },
        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = "6px",
            DrawerWidthLeft = "260px"
        },
        /*
        Typography = new Typography
        {
            Default = new()
            {
                FontFamily = new[] { "Inter", "Helvetica", "Arial", "sans-serif" },
                FontSize = ".875rem",
                FontWeight = 400,
                LineHeight = 1.43,
                LetterSpacing = ".01071em"
            },
            H1 = new() { FontFamily = new[] { "Inter", "Helvetica", "Arial", "sans-serif" }, FontWeight = 600 },
            H2 = new() { FontFamily = new[] { "Inter", "Helvetica", "Arial", "sans-serif" }, FontWeight = 600 },
            H3 = new() { FontFamily = new[] { "Inter", "Helvetica", "Arial", "sans-serif" }, FontWeight = 600 },
            H4 = new() { FontFamily = new[] { "Inter", "Helvetica", "Arial", "sans-serif" }, FontWeight = 600 },
            H5 = new() { FontFamily = new[] { "Inter", "Helvetica", "Arial", "sans-serif" }, FontWeight = 600 },
            H6 = new() { FontFamily = new[] { "Inter", "Helvetica", "Arial", "sans-serif" }, FontWeight = 600 },
            Button = new() { FontFamily = new[] { "Inter", "Helvetica", "Arial", "sans-serif" }, FontWeight = 500, TextTransform = "none" }
        }
        */
    };
}
