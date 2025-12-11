using Microsoft.JSInterop;
using System.Text.Json;

namespace EChamado.Client.Services;

/// <summary>
/// Logger que persiste em localStorage para sobreviver a redirects OAuth
/// </summary>
public class PersistentLogger
{
    private readonly IJSRuntime _js;
    private const string LogKey = "echamado_debug_logs";
    private const int MaxLogEntries = 100;

    public PersistentLogger(IJSRuntime js)
    {
        _js = js;
    }

    public async Task LogAsync(string level, string category, string message, string? data = null)
    {
        try
        {
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            var logEntry = new
            {
                timestamp,
                level,
                category,
                message,
                data,
                url = await GetCurrentUrlAsync()
            };

            // Adiciona ao array de logs em localStorage
            await _js.InvokeVoidAsync("eval", $@"
                (function() {{
                    try {{
                        let logs = JSON.parse(localStorage.getItem('{LogKey}') || '[]');
                        logs.push({JsonSerializer.Serialize(logEntry)});

                        // Limita a 100 entradas para não crescer infinitamente
                        if (logs.length > {MaxLogEntries}) {{
                            logs = logs.slice(-{MaxLogEntries});
                        }}

                        localStorage.setItem('{LogKey}', JSON.stringify(logs));
                        console.log('[PERSISTENT LOG] [{timestamp}] [{level}] [{category}] {message}');
                    }} catch(e) {{
                        console.error('Failed to write persistent log:', e);
                    }}
                }})();
            ");
        }
        catch (Exception ex)
        {
            // Se falhar, pelo menos loga no console
            Console.WriteLine($"[PERSISTENT LOG FAILED] {level} - {category}: {message} - Error: {ex.Message}");
        }
    }

    public Task InfoAsync(string category, string message, string? data = null)
        => LogAsync("INFO", category, message, data);

    public Task WarnAsync(string category, string message, string? data = null)
        => LogAsync("WARN", category, message, data);

    public Task ErrorAsync(string category, string message, string? data = null)
        => LogAsync("ERROR", category, message, data);

    public Task DebugAsync(string category, string message, string? data = null)
        => LogAsync("DEBUG", category, message, data);

    public Task AuthAsync(string message, string? data = null)
        => InfoAsync("AUTH", message, data);

    public Task AuthErrorAsync(string message, string? data = null)
        => ErrorAsync("AUTH", message, data);

    public async Task<string> GetLogsAsJsonAsync()
    {
        try
        {
            return await _js.InvokeAsync<string>("eval",
                $"localStorage.getItem('{LogKey}') || '[]'");
        }
        catch
        {
            return "[]";
        }
    }

    public async Task ClearLogsAsync()
    {
        try
        {
            await _js.InvokeVoidAsync("localStorage.removeItem", LogKey);
            await InfoAsync("LOGGER", "Logs cleared");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to clear logs: {ex.Message}");
        }
    }

    private async Task<string> GetCurrentUrlAsync()
    {
        try
        {
            return await _js.InvokeAsync<string>("eval", "window.location.href");
        }
        catch
        {
            return "unknown";
        }
    }
}
