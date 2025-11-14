using EChamado.E2E.Tests.Common.Attributes;
using EChamado.E2E.Tests.Common.Base;
using FluentAssertions;
using Microsoft.Playwright;
using Xunit;

namespace EChamado.E2E.Tests.Authentication;

/// <summary>
/// Testes E2E para funcionalidades de autenticação
/// </summary>
public class LoginTests : PlaywrightTestBase
{
    private const string BaseUrl = "https://localhost:7001";

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        // Configurações específicas para testes de login
    }

    [E2EFact]
    public async Task Login_WithValidCredentials_ShouldSucceed()
    {
        // Arrange
        await Page!.GotoAsync($"{BaseUrl}/login");

        // Act
        await Page.FillAsync("#email", "admin@test.com");
        await Page.FillAsync("#password", "Admin123!");
        await Page.ClickAsync("button[type='submit']");

        // Assert
        await Page.WaitForURLAsync($"{BaseUrl}/dashboard");
        var url = Page.Url;
        Assert.Contains("/dashboard", url);
    }

    [E2EFact]
    public async Task Login_WithInvalidCredentials_ShouldShowError()
    {
        // Arrange
        await Page!.GotoAsync($"{BaseUrl}/login");

        // Act
        await Page.FillAsync("#email", "invalid@test.com");
        await Page.FillAsync("#password", "wrongpassword");
        await Page.ClickAsync("button[type='submit']");

        // Assert
        var errorElement = Page.Locator(".error-message");
        await errorElement.WaitForAsync();
        var isVisible = await errorElement.IsVisibleAsync();
        Assert.True(isVisible);
    }

    [E2EFact]
    public async Task Login_WithEmptyFields_ShouldShowValidationErrors()
    {
        // Arrange
        await Page!.GotoAsync($"{BaseUrl}/login");

        // Act
        await Page.ClickAsync("button[type='submit']");

        // Assert
        var emailError = Page.Locator(".email-error");
        var passwordError = Page.Locator(".password-error");
        
        await emailError.WaitForAsync();
        await passwordError.WaitForAsync();
        
        Assert.True(await emailError.IsVisibleAsync());
        Assert.True(await passwordError.IsVisibleAsync());
    }

    [E2EFact]
    public async Task LoginPage_ShouldLoadCorrectly()
    {
        // Act
        await Page!.GotoAsync($"{BaseUrl}/login");

        // Assert
        var title = await Page.TitleAsync();
        Assert.Contains("Login", title);
        
        var emailField = Page.Locator("#email");
        var passwordField = Page.Locator("#password");
        var submitButton = Page.Locator("button[type='submit']");
        
        Assert.True(await emailField.IsVisibleAsync());
        Assert.True(await passwordField.IsVisibleAsync());
        Assert.True(await submitButton.IsVisibleAsync());
    }
}