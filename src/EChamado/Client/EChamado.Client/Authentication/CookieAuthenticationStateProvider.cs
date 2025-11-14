using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EChamado.Client.Authentication;

public class CookieAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CookieAuthenticationStateProvider> _logger;
    private readonly IJSRuntime _js;

    public CookieAuthenticationStateProvider(
        HttpClient httpClient,
        ILogger<CookieAuthenticationStateProvider> logger,
        IJSRuntime js)
    {
        _httpClient = httpClient;
        _logger = logger;
        _js = js;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            _logger.LogInformation("Checking authentication state from Auth server");

            // Tenta pegar o token do localStorage
            string? token = null;
            try
            {
                token = await _js.InvokeAsync<string?>("localStorage.getItem", "authToken");
                if (!string.IsNullOrEmpty(token))
                {
                    _logger.LogInformation("Token found in localStorage: {Token}", token);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to get token from localStorage");
            }

            // Cria request incluindo o token se existir
            var url = string.IsNullOrEmpty(token) ? "Account/User" : $"Account/User?token={token}";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            var response = await _httpClient.SendAsync(request);

            _logger.LogInformation("Auth response status: {StatusCode}", response.StatusCode);

            if (response.IsSuccessStatusCode)
            {
                var userInfo = await response.Content.ReadFromJsonAsync<UserInfo>();

                if (userInfo != null && userInfo.IsAuthenticated)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, userInfo.UserName ?? ""),
                        new Claim(ClaimTypes.Email, userInfo.Email ?? ""),
                        new Claim(ClaimTypes.NameIdentifier, userInfo.UserId ?? "")
                    };

                    foreach (var role in userInfo.Roles ?? Array.Empty<string>())
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role));
                    }

                    var identity = new ClaimsIdentity(claims, "Token");
                    var user = new ClaimsPrincipal(identity);

                    _logger.LogInformation("User authenticated: {UserName}", userInfo.UserName);

                    return new AuthenticationState(user);
                }
                else
                {
                    _logger.LogInformation("User not authenticated");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get authentication state");
        }

        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
    }

    public void NotifyAuthenticationStateChanged()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}

public class UserInfo
{
    public bool IsAuthenticated { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? UserId { get; set; }
    public string[]? Roles { get; set; }
}
