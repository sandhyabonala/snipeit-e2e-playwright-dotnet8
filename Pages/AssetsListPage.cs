using System.Threading.Tasks;
using Microsoft.Playwright;

namespace Global360.SnipeIT.Playwright.Pages;

public class AssetsListPage
{
    private readonly IPage _page;
    private readonly string _baseUrl;

    public AssetsListPage(IPage page, string baseUrl)
    {
        _page = page;
        _baseUrl = baseUrl.TrimEnd('/');
    }

    public async Task GotoAsync() => await _page.GotoAsync($"{_baseUrl}/hardware");

    // Matches your UI: Create New (dropdown) → Asset
    public async Task ClickCreateNewAsync()
    {
        await _page.Locator("a.dropdown-toggle:has-text('Create New')").ClickAsync();
        await _page.Locator("ul.dropdown-menu li a[href$='/hardware/create']").ClickAsync();
    }

    public async Task SearchAsync(string assetTag)
    {
        var searchBox = _page.Locator("div.dataTables_filter input[type='search']");
        await searchBox.FillAsync(assetTag);
        await searchBox.PressAsync("Enter");
    }

    public ILocator RowByAssetTag(string assetTag) =>
        _page.Locator("table tbody tr").Filter(new() { HasText = assetTag });

    public async Task OpenAssetByTagAsync(string assetTag)
    {
        await _page.Locator("table").WaitForAsync();
        await _page.GetByRole(AriaRole.Link, new() { Name = assetTag }).First.ClickAsync();
    }
}
