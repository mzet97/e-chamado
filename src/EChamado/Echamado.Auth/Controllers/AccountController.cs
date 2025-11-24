using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using EChamado.Server.Domain.Domains.Identities;
using OpenIddict.Server;
using OpenIddict.Core;
using System.Security.Claims;

namespace Echamado.Auth.Controllers;

[Route("[controller]")]
public class AccountController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        ILogger<AccountController> logger)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _logger = logger;
    }

    [HttpPost("DoLogin")]
    public async Task<IActionResult> DoLogin([FromForm] string email, [FromForm] string password, [FromForm] string? returnUrl)
    {
        _logger.LogInformation("🔐 Login attempt for {Email}", email);

        // Valida credenciais
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            _logger.LogWarning("❌ User not found for email {Email}", email);
            return Redirect($"/Account/Login?error=Invalid%20email%20or%20password");
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

            var errorMsg = result.RequiresTwoFactor
                ? "Two-factor%20authentication%20is%20required"
                : result.IsLockedOut
                    ? "Account%20is%20locked.%20Please%20try%20again%20later."
                    : "Invalid%20email%20or%20password";
            return Redirect($"/Account/Login?error={errorMsg}");
        }

        _logger.LogInformation("✅ User {Email} authenticated successfully", email);

        // Cria o cookie de autenticação no Auth server
        var principal = await _signInManager.CreateUserPrincipalAsync(user);
        await HttpContext.SignInAsync("External", principal);

        // Com OpenIddict configurado, o redirecionamento para returnUrl
        // permitirá que o OpenIddict complete o fluxo e gere o token JWT
        if (!string.IsNullOrEmpty(returnUrl))
        {
            _logger.LogInformation("📍 Redirecting to returnUrl: {ReturnUrl}", returnUrl);
            return Redirect(returnUrl);
        }

        // Se não há returnUrl, redireciona para root
        return Redirect("/");
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
}
