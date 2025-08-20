using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Global360.SnipeIT.Playwright.Pages
{
    public class AssetDetailsPage
    {
        private readonly IPage _page;
        public AssetDetailsPage(IPage page) => _page = page;

        private ILocator Header =>
            _page.GetByRole(AriaRole.Heading, new() { NameRegex = new(@"^Assets\s*\(") });

        private async Task EnsureInfoTabAsync()
        {
            // Click Info if not active
            if (await _page.Locator(".nav-tabs li.active a:has-text('Info')").CountAsync() == 0)
                await _page.Locator(".nav-tabs a:has-text('Info')").First.ClickAsync();
        }

        public async Task<string> ReadHeaderTextAsync()
        {
            await Expect(Header).ToBeVisibleAsync();
            return (await Header.InnerTextAsync()).Trim();
        }

        public async Task<string> ReadAssetTagAsync()
        {
            var h = await ReadHeaderTextAsync();                // Assets (FFE-SUPPLIES-686213) - Macbook Pro 13"
            var m = Regex.Match(h, @"\(([^)]+)\)");
            return m.Success ? m.Groups[1].Value.Trim() : string.Empty;
        }

        public async Task<string> ReadModelAsync()
        {
            var h = await ReadHeaderTextAsync();
            var m = Regex.Match(h, @"-\s*(.+)$");
            return m.Success ? m.Groups[1].Value.Trim() : h;
        }

        public async Task<string> ReadStatusAsync()
        {
            await EnsureInfoTabAsync();
            // Your exact markup: <div class="row"> <div class="col-md-3"><strong>Status</strong></div> <div class="col-md-9">…</div>
            var valueCol = _page.Locator("div.info-stack .row:has(.col-md-3:has-text('Status')) .col-md-9").First;
            await Expect(valueCol).ToBeVisibleAsync();
            var text = (await valueCol.InnerTextAsync()).Trim();
            return Regex.Replace(text, @"\s+", " ").Trim();     // normalize whitespace
        }

        public async Task<string> ReadAssignedToAsync()
        {
            await EnsureInfoTabAsync();
            // In the same Status row’s right column
            var userLink = _page.Locator("div.info-stack .row:has(.col-md-3:has-text('Status')) .col-md-9 a[href*='/users/']").First;
            await Expect(userLink).ToBeVisibleAsync();
            return (await userLink.InnerTextAsync()).Trim();
        }

        public async Task OpenHistoryTabAsync()
        {
            await _page.Locator(".nav-tabs a:has-text('History')").First.ClickAsync();
            await Expect(_page.Locator("#history table tbody").First).ToBeVisibleAsync();
        }

    }
}
