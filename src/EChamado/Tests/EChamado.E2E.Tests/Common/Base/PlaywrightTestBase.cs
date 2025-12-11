using Microsoft.Playwright;
using Xunit;

namespace EChamado.E2E.Tests.Common.Base;

public abstract class PlaywrightTestBase : IAsyncLifetime
{
    protected IPlaywright? Playwright;
    protected IBrowser? Browser;
    protected IPage? Page;

    public virtual async Task InitializeAsync()
    {
        Playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        Browser = await Playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true
        });
        Page = await Browser.NewPageAsync();
    }

    public virtual async Task DisposeAsync()
    {
        if (Page != null)
            await Page.CloseAsync();
        
        if (Browser != null)
            await Browser.CloseAsync();
        
        Playwright?.Dispose();
    }
}