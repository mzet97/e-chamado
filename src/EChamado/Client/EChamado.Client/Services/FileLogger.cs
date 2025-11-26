using Microsoft.JSInterop;
using System.Text;

namespace EChamado.Client.Services;

public class FileLogger
{
    private readonly IJSRuntime _js;
    private readonly StringBuilder _logBuilder = new();

    public FileLogger(IJSRuntime js)
    {
        _js = js;
    }

    public void Log(string level, string category, string message, Exception? exception = null)
    {
        var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        var logEntry = $"[{timestamp}] [{level}] [{category}] {message}";
        
        if (exception != null)
        {
            logEntry += $" | Exception: {exception.Message}\nStack: {exception.StackTrace}";
        }
        
        _logBuilder.AppendLine(logEntry);
        
        // También log to console for immediate visibility
        Console.WriteLine(logEntry);
    }

    public void Info(string category, string message)
    {
        Log("INFO", category, message);
    }

    public void Warning(string category, string message)
    {
        Log("WARN", category, message);
    }

    public void Error(string category, string message, Exception? exception = null)
    {
        Log("ERROR", category, message, exception);
    }

    public void Debug(string category, string message)
    {
        Log("DEBUG", category, message);
    }

    public void Auth(string message)
    {
        Info("AUTH", message);
    }

    public void AuthError(string message, Exception? exception = null)
    {
        Error("AUTH", message, exception);
    }

    public async Task SaveLogToFileAsync(string fileName = "echamado-debug.log")
    {
        try
        {
            var logContent = _logBuilder.ToString();
            var dataUri = $"data:text/plain;charset=utf-8,{Uri.EscapeDataString(logContent)}";
            
            await _js.InvokeVoidAsync("downloadFile", fileName, dataUri);
            
            Info("FILE_LOGGER", $"Log salvo em arquivo: {fileName}");
        }
        catch (Exception ex)
        {
            Error("FILE_LOGGER", "Erro ao salvar log em arquivo", ex);
        }
    }

    public void Clear()
    {
        _logBuilder.Clear();
    }

    public string GetLogs()
    {
        return _logBuilder.ToString();
    }
}

// JavaScript interop functions to be defined in a separate JS file
// We'll add these to a JS file that gets loaded with the application