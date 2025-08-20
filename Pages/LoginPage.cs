using System.Threading.Tasks;
using Microsoft.Playwright;

namespace Global360.SnipeIT.Playwright.Pages;

public class LoginPage
{
    private readonly IPage _page;
    private readonly string _baseUrl;

    public LoginPage(IPage page, string baseUrl)
    {
        _page = page;
        _baseUrl = baseUrl.TrimEnd('/');
    }

    public async Task GotoAsync() => await _page.GotoAsync($"{_baseUrl}/login");

    public async Task LoginAsync(string username, string password)
    {
        await _page.GetByLabel("Username").FillAsync(username);
        await _page.GetByLabel("Password").FillAsync(password);
        await _page.GetByRole(AriaRole.Button, new() { Name = "Login" }).ClickAsync();

        // Post-login sanity: nav visible
        await _page.GetByRole(AriaRole.Navigation).WaitForAsync();
    }
}