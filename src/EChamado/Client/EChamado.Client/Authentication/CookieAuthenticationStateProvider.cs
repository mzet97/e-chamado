using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using EChamado.Client.Services;

namespace EChamado.Client.Authentication;

public class CookieAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _js;
    private readonly AuthService _authService;
    private readonly PersistentLogger _persistentLog;

    public CookieAuthenticationStateProvider(
        HttpClient httpClient,
        IJSRuntime js,
        AuthService authService,
        PersistentLogger persistentLog)
    {
        _httpClient = httpClient;
        _js = js;
        _authService = authService;
        _persistentLog = persistentLog;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        await _persistentLog.DebugAsync("AUTH_STATE", "🔍 GetAuthenticationStateAsync() called");
        Console.WriteLine("🔍 Checking authentication state...");

        try
        {
            var token = await _js.InvokeAsync<string?>("localStorage.getItem", "authToken");

            if (!string.IsNullOrEmpty(token))
            {
                await _persistentLog.AuthAsync("[AUTH_STATE] Token found",
                    $"Length: {token.Length} chars, Preview: {token.Substring(0, Math.Min(30, token.Length))}...");
                Console.WriteLine($"🔑 Token found: {token.Substring(0, Math.Min(20, token.Length))}...");

                // Configura o token no header do HttpClient
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                await _persistentLog.DebugAsync("AUTH_STATE", "Calling ParseJwtToken()...");

                // Parse do JWT (agora não-criptografado)
                var claims = ParseJwtToken(token);

                await _persistentLog.AuthAsync($"[AUTH_STATE] ParseJwtToken returned {claims.Count} claims");

                if (claims.Count > 0)
                {
                    Console.WriteLine($"✅ JWT parsed successfully: {claims.Count} claims");

                    // Log all claims
                    foreach (var claim in claims)
                    {
                        await _persistentLog.DebugAsync("JWT_CLAIM", $"{claim.Type} = {claim.Value}");
                    }

                    var identity = new ClaimsIdentity(claims, "jwt");
                    var user = new ClaimsPrincipal(identity);

                    await _persistentLog.AuthAsync($"[AUTH_STATE] ✅ User authenticated: {user.Identity?.Name}, IsAuthenticated: {user.Identity?.IsAuthenticated}");
                    Console.WriteLine($"✅ User authenticated: {user.Identity?.Name}");
                    return new AuthenticationState(user);
                }
                else
                {
                    await _persistentLog.AuthErrorAsync("[AUTH_STATE] ❌ No claims extracted from JWT");
                    Console.WriteLine("❌ Invalid or expired token - no claims extracted");
                }
            }
            else
            {
                await _persistentLog.AuthErrorAsync("[AUTH_STATE] ❌ No token in localStorage");
                Console.WriteLine("❌ No token found in localStorage");
            }
        }
        catch (Exception ex)
        {
            await _persistentLog.AuthErrorAsync($"[AUTH_STATE] ❌ EXCEPTION: {ex.Message}", ex.StackTrace);
            Console.WriteLine($"❌ Error getting authentication state: {ex.Message}");
        }

        await _persistentLog.AuthErrorAsync("[AUTH_STATE] ❌ Returning UNAUTHENTICATED state");
        Console.WriteLine("❌ Returning unauthenticated state");
        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
    }

    private List<Claim> ParseJwtToken(string token)
    {
        var claims = new List<Claim>();
        
        try
        {
            var parts = token.Split('.');
            if (parts.Length != 3)
                return claims;

            var payload = parts[1];
            // Adiciona padding se necessário
            while (payload.Length % 4 != 0)
                payload += "=";
                
            var payloadBytes = Convert.FromBase64String(payload);
            var payloadJson = Encoding.UTF8.GetString(payloadBytes);
            
            var payloadDoc = JsonDocument.Parse(payloadJson);
            var root = payloadDoc.RootElement;

            // Extrai claims padrão do JWT
            if (root.TryGetProperty("sub", out var sub))
                claims.Add(new Claim(ClaimTypes.NameIdentifier, sub.GetString() ?? ""));
                
            if (root.TryGetProperty("email", out var email))
                claims.Add(new Claim(ClaimTypes.Email, email.GetString() ?? ""));
                
            if (root.TryGetProperty("name", out var name))
                claims.Add(new Claim(ClaimTypes.Name, name.GetString() ?? ""));
            else if (root.TryGetProperty("preferred_username", out var preferredUsername))
                claims.Add(new Claim(ClaimTypes.Name, preferredUsername.GetString() ?? ""));
            else if (root.TryGetProperty("unique_name", out var uniqueName))
                claims.Add(new Claim(ClaimTypes.Name, uniqueName.GetString() ?? ""));

            // Extrai roles se existirem
            if (root.TryGetProperty("role", out var role))
            {
                if (role.ValueKind == JsonValueKind.String)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role.GetString() ?? ""));
                }
                else if (role.ValueKind == JsonValueKind.Array)
                {
                    foreach (var roleElement in role.EnumerateArray())
                    {
                        claims.Add(new Claim(ClaimTypes.Role, roleElement.GetString() ?? ""));
                    }
                }
            }

            // Extrai roles do array "roles" se existir
            if (root.TryGetProperty("roles", out var roles))
            {
                if (roles.ValueKind == JsonValueKind.Array)
                {
                    foreach (var roleElement in roles.EnumerateArray())
                    {
                        claims.Add(new Claim(ClaimTypes.Role, roleElement.GetString() ?? ""));
                    }
                }
            }

            // Verifica se o token não expirou
            if (root.TryGetProperty("exp", out var exp))
            {
                var expUnix = exp.GetInt64();
                var expirationDate = DateTimeOffset.FromUnixTimeSeconds(expUnix);
                
                if (DateTimeOffset.UtcNow > expirationDate)
                {
                    Console.WriteLine($"❌ Token expired at {expirationDate}");
                    return new List<Claim>(); // Token expirado
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error parsing JWT token: {ex.Message}");
        }

        return claims;
    }

    public async Task<bool> LoginAsync(string email, string password)
    {
        var result = await _authService.LoginAsync(new Models.LoginModel { Email = email, Password = password });
        return result.Successful;
    }

    public async Task LogoutAsync()
    {
        await _authService.LogoutAsync();
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public void NotifyUserAuthenticated()
    {
        Console.WriteLine("🔔 Notifying authentication state changed");
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}