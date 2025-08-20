using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Playwright;
using NUnit.Framework;

namespace Global360.SnipeIT.Playwright.Support;

public abstract class TestBase
{
    protected IPlaywright? _playwright;
    protected IBrowser? _browser;
    protected IBrowserContext? _context;
    protected IPage? Page;
    protected TestSettings Settings = new();

    [SetUp]
    public async Task SetUp()
    {
        _playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = Settings.Headless,
            Args = new[] { "--start-maximized" }
        });

        _context = await _browser.NewContextAsync(new BrowserNewContextOptions { ViewportSize = null });
        Page = await _context.NewPageAsync();
        Page.SetDefaultTimeout((float)Settings.Timeout.TotalMilliseconds);
    }

    [TearDown]
    public async Task TearDown()
    {
        try
        {
            if (TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Failed && Page is not null)
            {
                Directory.CreateDirectory("artifacts");
                var file = Path.Combine("artifacts", $"{Sanitize(TestContext.CurrentContext.Test.Name)}.png");
                await Page.ScreenshotAsync(new PageScreenshotOptions { Path = file, FullPage = true });
                TestContext.AddTestAttachment(file);
            }
        }
        finally
        {
            await _context?.CloseAsync()!;
            await _browser?.CloseAsync()!;
            _playwright?.Dispose();
        }
    }

    private static string Sanitize(string name) =>
        string.Join("_", name.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries)).TrimEnd('.');
}