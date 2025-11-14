using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using EChamado.Server.Domain.Domains.Identities;

namespace Echamado.Auth.Controllers;

[Route("[controller]")]
public class AccountController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<AccountController> _logger;

    // Cache em memória para session tokens (em produção, usar Redis)
    private static readonly Dictionary<string, SessionInfo> _sessionCache = new();

    public AccountController(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        ILogger<AccountController> logger)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _logger = logger;
    }

    private class SessionInfo
    {
        public string UserId { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
    }

    // Limpa sessões expiradas periodicamente
    private void CleanExpiredSessions()
    {
        var now = DateTime.UtcNow;
        var expiredKeys = _sessionCache.Where(kvp => kvp.Value.ExpiresAt < now).Select(kvp => kvp.Key).ToList();
        foreach (var key in expiredKeys)
        {
            _sessionCache.Remove(key);
        }
    }

    [HttpPost("DoLogin")]
    public async Task<IActionResult> DoLogin([FromForm] string email, [FromForm] string password, [FromForm] string? returnUrl)
    {
        _logger.LogInformation("🔐 Login attempt for {Email} with returnUrl: {ReturnUrl}", email, returnUrl);

        // Valida credenciais
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            _logger.LogWarning("❌ User not found for email {Email}", email);
            var r = Uri.EscapeDataString(returnUrl ?? string.Empty);
            return Redirect($"/Account/Login?error=Invalid%20email%20or%20password&returnUrl={r}");
        }

        var result = await _signInManager.PasswordSignInAsync(
            user.UserName!,
            password,
            isPersistent: false,
            lockoutOnFailure: true);

        if (!result.Succeeded)
        {
            _logger.LogWarning("❌ Login failed for {Email}. Reason: {Reason}",
                email,
                result.RequiresTwoFactor ? "2FA" : result.IsLockedOut ? "Locked" : "Invalid credentials");

            var r = Uri.EscapeDataString(returnUrl ?? string.Empty);
            var errorMsg = result.RequiresTwoFactor
                ? "Two-factor%20authentication%20is%20required"
                : result.IsLockedOut
                    ? "Account%20is%20locked.%20Please%20try%20again%20later."
                    : "Invalid%20email%20or%20password";
            return Redirect($"/Account/Login?error={errorMsg}&returnUrl={r}");
        }

        _logger.LogInformation("✅ User {Email} authenticated successfully", email);

        // Cria o cookie de autenticação no Auth server
        var principal = await _signInManager.CreateUserPrincipalAsync(user);
        await HttpContext.SignInAsync("External", principal);

        // Gera um token simples (session ID) para o cliente
        var sessionId = Guid.NewGuid().ToString("N");

        // Armazena o session ID associado ao usuário no cache em memória
        CleanExpiredSessions(); // Limpa sessões expiradas
        _sessionCache[sessionId] = new SessionInfo
        {
            UserId = user.Id.ToString(),
            ExpiresAt = DateTime.UtcNow.AddMinutes(30)
        };

        _logger.LogInformation("🔑 Session token created: {Token} for user: {UserId}", sessionId, user.Id);

        // FLUXO SIMPLIFICADO: Redireciona para o returnUrl do cliente COM O SESSION ID
        if (string.IsNullOrEmpty(returnUrl))
        {
            _logger.LogWarning("⚠️ No returnUrl provided. Redirecting to root.");
            return Redirect("/");
        }

        try
        {
            var decodedReturnUrl = DecodeDeep(returnUrl);
            _logger.LogInformation("📍 Decoded returnUrl: {DecodedReturnUrl}", decodedReturnUrl);

            // Valida se a URL é do cliente
            if (Uri.TryCreate(decodedReturnUrl, UriKind.Absolute, out var uri))
            {
                _logger.LogInformation("🔍 Parsed URI - Host: {Host}, Port: {Port}, Path: {Path}", uri.Host, uri.Port, uri.AbsolutePath);

                // Aceita localhost nas portas do cliente: 5199, 7274
                if (uri.Host == "localhost" && (uri.Port == 5199 || uri.Port == 7274 || uri.Port == 7296))
                {
                    // Adiciona o token à URL
                    var separator = decodedReturnUrl.Contains('?') ? "&" : "?";
                    var urlWithToken = $"{decodedReturnUrl}{separator}token={sessionId}";

                    _logger.LogInformation("✅ Redirecting to: {ReturnUrl}", urlWithToken);
                    return Redirect(urlWithToken);
                }
                else
                {
                    _logger.LogWarning("❌ Invalid port {Port}. Expected 5199, 7274, or 7296", uri.Port);
                }
            }
            else
            {
                _logger.LogError("❌ Failed to parse returnUrl as URI: {DecodedReturnUrl}", decodedReturnUrl);
            }

            _logger.LogWarning("❌ Invalid returnUrl '{DecodedReturnUrl}'", decodedReturnUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Exception while processing returnUrl: {ReturnUrl}", returnUrl);
        }

        return Redirect("/");
    }

    [HttpGet("User")]
    public async Task<IActionResult> GetCurrentUser([FromQuery] string? token)
    {
        try
        {
            ApplicationUser? user = null;

            // Se veio um token, valida ele
            if (!string.IsNullOrEmpty(token))
            {
                _logger.LogInformation("Validating token: {Token}", token);

                // Limpa sessões expiradas
                CleanExpiredSessions();

                // Busca o userId pelo token no cache em memória
                if (_sessionCache.TryGetValue(token, out var sessionInfo))
                {
                    // Verifica se a sessão não expirou
                    if (sessionInfo.ExpiresAt > DateTime.UtcNow)
                    {
                        _logger.LogInformation("✅ Valid session found for token: {Token}, userId: {UserId}", token, sessionInfo.UserId);
                        user = await _userManager.FindByIdAsync(sessionInfo.UserId);
                    }
                    else
                    {
                        _logger.LogWarning("⚠️ Session expired for token: {Token}", token);
                        _sessionCache.Remove(token);
                    }
                }
                else
                {
                    _logger.LogWarning("❌ Session not found for token: {Token}", token);
                }
            }
            else
            {
                // Se não veio token, tenta pegar do cookie de autenticação padrão
                user = await _userManager.GetUserAsync(User);
            }

            if (user == null)
            {
                _logger.LogInformation("No authenticated user found");
                return Ok(new
                {
                    IsAuthenticated = false
                });
            }

            var roles = await _userManager.GetRolesAsync(user);

            _logger.LogInformation("Returning user info for: {UserName}", user.UserName);

            return Ok(new
            {
                IsAuthenticated = true,
                UserName = user.UserName,
                Email = user.Email,
                UserId = user.Id.ToString(),
                Roles = roles.ToArray()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting current user");
            return Ok(new
            {
                IsAuthenticated = false
            });
        }
    }

    [HttpGet("Logout")]
    [HttpPost("Logout")]
    public async Task<IActionResult> Logout(string? returnUrl)
    {
        await _signInManager.SignOutAsync();
        await HttpContext.SignOutAsync("External");

        if (!string.IsNullOrEmpty(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return Redirect("/");
    }

    private static bool IsValidReturnUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url)) return false;
        if (Uri.TryCreate(url, UriKind.Absolute, out var abs))
        {
            // Aceita returnUrl para o servidor OpenIddict (porta 7296)
            if (string.Equals(abs.Scheme, "https", StringComparison.OrdinalIgnoreCase)
                && string.Equals(abs.Host, "localhost", StringComparison.OrdinalIgnoreCase)
                && abs.Port == 7296
                && abs.AbsolutePath.StartsWith("/connect/authorize", StringComparison.Ordinal))
            {
                return true;
            }
            return false;
        }
        if (Uri.IsWellFormedUriString(url, UriKind.Relative))
        {
            return url.StartsWith("/connect/authorize", StringComparison.Ordinal);
        }
        return false;
    }

    private static string BuildAuthorizeUrl(string url)
    {
        if (Uri.TryCreate(url, UriKind.Absolute, out var abs))
        {
            // Se já é uma URL absoluta válida, retorna como está
            return abs.ToString();
        }
        if (Uri.IsWellFormedUriString(url, UriKind.Relative)
            && url.StartsWith("/connect/authorize", StringComparison.Ordinal))
        {
            // Se é relativa, constrói URL completa para o servidor OpenIddict (7296)
            return $"https://localhost:7296{url}";
        }
        // Fallback: redireciona para o servidor OpenIddict
        return "https://localhost:7296";
    }

    private static string DecodeDeep(string value)
    {
        var current = value ?? string.Empty;
        for (int i = 0; i < 3; i++)
        {
            var next = Uri.UnescapeDataString(current);
            if (next == current) break;
            current = next;
        }
        return current;
    }
}
