using Microsoft.Playwright;

namespace EChamado.E2E.Tests.PageObjects;

/// <summary>
/// Page Object Model para a página de login
/// </summary>
public class LoginPage
{
    private readonly IPage _page;

    public LoginPage(IPage page)
    {
        _page = page;
    }

    // Seletores
    private const string EmailInput = "[data-test='email']";
    private const string PasswordInput = "[data-test='password']";
    private const string LoginButton = "[data-test='login-button']";
    private const string RememberMeCheckbox = "[data-test='remember-me']";
    private const string ForgotPasswordLink = "[data-test='forgot-password']";
    private const string RegisterLink = "[data-test='register-link']";
    private const string ErrorMessage = "[data-test='error-message']";

    // Ações
    public async Task FillEmailAsync(string email)
    {
        await _page.FillAsync(EmailInput, email);
    }

    public async Task FillPasswordAsync(string password)
    {
        await _page.FillAsync(PasswordInput, password);
    }

    public async Task ClickLoginAsync()
    {
        await _page.ClickAsync(LoginButton);
    }

    public async Task CheckRememberMeAsync()
    {
        await _page.CheckAsync(RememberMeCheckbox);
    }

    public async Task ClickForgotPasswordAsync()
    {
        await _page.ClickAsync(ForgotPasswordLink);
    }

    public async Task ClickRegisterAsync()
    {
        await _page.ClickAsync(RegisterLink);
    }

    public async Task LoginAsync(string email, string password, bool rememberMe = false)
    {
        await FillEmailAsync(email);
        await FillPasswordAsync(password);

        if (rememberMe)
        {
            await CheckRememberMeAsync();
        }

        await ClickLoginAsync();
    }

    // Verificações
    public async Task<bool> IsEmailFieldVisibleAsync()
    {
        return await _page.IsVisibleAsync(EmailInput);
    }

    public async Task<bool> IsPasswordFieldVisibleAsync()
    {
        return await _page.IsVisibleAsync(PasswordInput);
    }

    public async Task<bool> IsLoginButtonEnabledAsync()
    {
        return await _page.IsEnabledAsync(LoginButton);
    }

    public async Task<string?> GetErrorMessageAsync()
    {
        if (await _page.IsVisibleAsync(ErrorMessage))
        {
            return await _page.TextContentAsync(ErrorMessage);
        }
        return null;
    }

    public async Task WaitForPageLoadAsync()
    {
        await _page.WaitForSelectorAsync(EmailInput);
        await _page.WaitForSelectorAsync(PasswordInput);
        await _page.WaitForSelectorAsync(LoginButton);
    }
}