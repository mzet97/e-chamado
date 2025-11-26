using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using EChamado.Client.Services;

namespace EChamado.Client.Authentication;

public class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AuthService> _logger;
    private readonly IJSRuntime _js;
    private readonly PersistentLogger _persistentLog;

    public AuthService(
        HttpClient httpClient,
        ILogger<AuthService> logger,
        IJSRuntime js,
        PersistentLogger persistentLog)
    {
        _httpClient = httpClient;
        _logger = logger;
        _js = js;
        _persistentLog = persistentLog;
    }

    public async Task<bool> LoginAsync(string email, string password)
    {
        try
        {
            var loginRequest = new
            {
                grant_type = "password",
                username = email,
                password = password,
                client_id = "mobile-client",
                scope = "openid profile email roles api chamados"
            };

            var response = await _httpClient.PostAsJsonAsync("connect/token", loginRequest);
            if (response.IsSuccessStatusCode)
            {
                var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
                if (tokenResponse?.AccessToken != null)
                {
                    await StoreTokensAsync(tokenResponse);
                    return true;
                }
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Login failed: {Error}", errorContent);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login failed with exception");
        }

        return false;
    }

    public async Task<bool> ExchangeCodeForTokenAsync(string code, string state, string codeVerifier)
    {
        try
        {
            // ========== AUTHSERVICE ETAPA 1: Início ==========
            await _persistentLog.AuthAsync("[AUTHSERVICE] Starting CODE → TOKEN exchange",
                $"Client: bwa-client, Code: {code?.Length ?? 0} chars, State: {state?.Length ?? 0} chars, Verifier: {codeVerifier?.Length ?? 0} chars");
            _logger.LogInformation("Starting code exchange");

            // OAuth2 token endpoint REQUER application/x-www-form-urlencoded, NÃO JSON
            var formData = new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "client_id", "bwa-client" },
                { "code", code },
                { "redirect_uri", "https://localhost:7274/authentication/login-callback" },
                { "code_verifier", codeVerifier }
            };

            // ========== AUTHSERVICE ETAPA 2: Enviando Request ==========
            await _persistentLog.DebugAsync("AUTHSERVICE", "Sending POST to connect/token with form-urlencoded");

            var response = await _httpClient.PostAsync("connect/token", new FormUrlEncodedContent(formData));

            // ========== AUTHSERVICE ETAPA 3: Resposta Recebida ==========
            await _persistentLog.AuthAsync($"[AUTHSERVICE] Response received: HTTP {(int)response.StatusCode} {response.StatusCode}",
                $"Success: {response.IsSuccessStatusCode}");

            if (response.IsSuccessStatusCode)
            {
                // ========== AUTHSERVICE ETAPA 4: Lendo JSON ==========
                var jsonContent = await response.Content.ReadAsStringAsync();
                await _persistentLog.DebugAsync("AUTHSERVICE", $"Response JSON length: {jsonContent.Length} chars");

                var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();

                if (tokenResponse?.AccessToken != null)
                {
                    // ========== AUTHSERVICE ETAPA 5: Token Válido ==========
                    var tokenPreview = tokenResponse.AccessToken.Substring(0, Math.Min(30, tokenResponse.AccessToken.Length)) + "...";
                    await _persistentLog.AuthAsync($"[AUTHSERVICE] ✅ Token RECEIVED!",
                        $"Length: {tokenResponse.AccessToken.Length} chars, Preview: {tokenPreview}");

                    // ========== AUTHSERVICE ETAPA 6: Armazenando Token ==========
                    await _persistentLog.DebugAsync("AUTHSERVICE", "Calling StoreTokensAsync()...");
                    await StoreTokensAsync(tokenResponse);
                    await _persistentLog.AuthAsync("[AUTHSERVICE] ✅ StoreTokensAsync() completed successfully");

                    return true;
                }
                else
                {
                    // ========== AUTHSERVICE ERRO: Token NULL ==========
                    await _persistentLog.AuthErrorAsync("[AUTHSERVICE] ❌ Token response INVALID - AccessToken is NULL");
                }
            }
            else
            {
                // ========== AUTHSERVICE ERRO: HTTP Error ==========
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Code exchange failed: {Error}", errorContent);
                await _persistentLog.AuthErrorAsync($"[AUTHSERVICE] ❌ HTTP {response.StatusCode}",
                    $"Error: {errorContent}");
            }
        }
        catch (Exception ex)
        {
            // ========== AUTHSERVICE ERRO: Exception ==========
            _logger.LogError(ex, "Code exchange failed with exception");
            await _persistentLog.AuthErrorAsync($"[AUTHSERVICE] ❌ EXCEPTION: {ex.Message}", ex.StackTrace);
        }

        await _persistentLog.AuthErrorAsync("[AUTHSERVICE] ❌ Returning FALSE - Token exchange FAILED");
        return false;
    }

    private async Task StoreTokensAsync(TokenResponse tokenResponse)
    {
        // ========== STORAGE ETAPA 1: Início ==========
        await _persistentLog.DebugAsync("STORAGE", "Starting StoreTokensAsync()");

        // ========== STORAGE ETAPA 2: Armazenando AccessToken ==========
        await _persistentLog.DebugAsync("STORAGE", "Calling localStorage.setItem('authToken', ...)");
        await _js.InvokeVoidAsync("localStorage.setItem", "authToken", tokenResponse.AccessToken);

        await _persistentLog.AuthAsync($"[STORAGE] ✅ AccessToken STORED in localStorage",
            $"Length: {tokenResponse.AccessToken.Length} chars");

        // ========== STORAGE ETAPA 3: Verificando se foi armazenado ==========
        var verifyToken = await _js.InvokeAsync<string?>("localStorage.getItem", "authToken");
        await _persistentLog.DebugAsync("STORAGE",
            $"Verification: localStorage.getItem('authToken') returned {(verifyToken != null ? $"{verifyToken.Length} chars" : "NULL")}");

        // ========== STORAGE ETAPA 4: RefreshToken (se houver) ==========
        if (!string.IsNullOrEmpty(tokenResponse.RefreshToken))
        {
            await _js.InvokeVoidAsync("localStorage.setItem", "refreshToken", tokenResponse.RefreshToken);
            await _persistentLog.DebugAsync("STORAGE",
                $"RefreshToken stored: {tokenResponse.RefreshToken.Length} chars");
        }
        else
        {
            await _persistentLog.DebugAsync("STORAGE", "No RefreshToken in response");
        }

        // ========== STORAGE ETAPA 5: HTTP Authorization Header ==========
        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);

        await _persistentLog.AuthAsync("[STORAGE] ✅ HTTP Authorization header SET with Bearer token");
        await _persistentLog.AuthAsync("[STORAGE] ✅ StoreTokensAsync() COMPLETED");
    }

    public async Task LogoutAsync()
    {
        try
        {
            await _js.InvokeVoidAsync("localStorage.removeItem", "authToken");
            await _js.InvokeVoidAsync("localStorage.removeItem", "refreshToken");
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to clear tokens during logout");
        }
    }

    public async Task<string?> GetCodeVerifierAsync()
    {
        try
        {
            return await _js.InvokeAsync<string?>("localStorage.getItem", "pkce_code_verifier");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get code verifier from localStorage");
            return null;
        }
    }
}