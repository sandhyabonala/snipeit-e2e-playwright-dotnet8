using FluentAssertions;
using Global360.SnipeIT.Playwright.Pages;
using Global360.SnipeIT.Playwright.Support;
using Microsoft.Playwright;
using NUnit.Framework;
using static Microsoft.Playwright.Assertions;
using System.Text.RegularExpressions;

namespace Global360.SnipeIT.Playwright;

[TestFixture]
public class SnipeITFlowTests : TestBase
{
    [Test]
    public async Task Create_MacBookPro13_ReadyToDeploy_Checkout_And_Validate_History()
    {
        var page = Page!;
        var settings = Settings;

        // 1) Login
        var login = new LoginPage(page, Settings.BaseUrl);
        await login.GotoAsync();
        await login.LoginAsync(Settings.Username, Settings.Password);

        // 2) Create New → Asset
        var assets = new AssetsListPage(page, Settings.BaseUrl);
        await assets.GotoAsync();
        await assets.ClickCreateNewAsync();

        // 3) Only set Model, Status, then Users (random). Don't touch any other fields.
        var create = new AssetCreatePage(page);
        await create.SelectModelByUniqueIdAsync();           // "Laptops - Macbook Pro 13\""
        await create.SelectStatusAsync();          // "Ready to Deploy"
        await create.SelectRandomUserAsync();      // any visible user
        await create.SaveAsync();                  // top-right Save
        await create.ClickSuccessLinkAsync();
        // Wait for details and capture ids
        await page.WaitForURLAsync(new Regex(@".*/hardware/\d+(#.*)?$"));
        var hardwareId = Regex.Match(page.Url, @"hardware/(\d+)").Groups[1].Value;

        var details = new AssetDetailsPage(page);
        var headerText = await details.ReadHeaderTextAsync();
        var assetTag = await details.ReadAssetTagAsync();

        // Validate Info/Checked Out To
        (await details.ReadModelAsync()).Should().Contain("Macbook Pro 13");
        (await details.ReadStatusAsync()).ToLowerInvariant().Should().Contain("ready to deploy");
        var assignee = (await details.ReadAssignedToAsync()).Trim();
        assignee.Should().NotBeNullOrEmpty();

        // Go to Assets list and verify row exists, then open it
       
        await assets.GotoAsync();

        var search = page.GetByPlaceholder("Search");           // your Search UI
        await search.FillAsync(assetTag);
        await page.Keyboard.PressAsync("Enter");

        var row = page.Locator($"table tbody tr:has(a[href$='/hardware/{hardwareId}'])").First;
        await Expect(row).ToBeVisibleAsync();
        await row.Locator("a[href*='/hardware/']").First.ClickAsync();

        // Back on details (fresh object)
        details = new AssetDetailsPage(page);

        // History: assert “create new” + “checkout” rows using your exact columns
        await details.OpenHistoryTabAsync();

        var historyBody = page.Locator("#history table tbody");

        // create new row (Action is 4th <td>, Item is 5th)
        var createRow = historyBody.Locator("tr:has(td:nth-child(4):has-text('create new'))").First;
        await Expect(createRow).ToBeVisibleAsync();
        await Expect(createRow.Locator("td").Nth(4).Locator("a[href*='/hardware/']")).ToContainTextAsync(assetTag);
        await Expect(createRow.Locator("td").Nth(4)).ToContainTextAsync("Macbook Pro 13\"");

        // checkout row (Action is 4th, Item 5th, Target 6th)
        var checkoutRow = historyBody.Locator("tr:has(td:nth-child(4):has-text('checkout'))").First;
        await Expect(checkoutRow).ToBeVisibleAsync();
        await Expect(checkoutRow.Locator("td").Nth(4).Locator("a[href*='/hardware/']")).ToContainTextAsync(assetTag);
        await Expect(checkoutRow.Locator("td").Nth(4)).ToContainTextAsync("Macbook Pro 13\"");
        await Expect(checkoutRow.Locator("td").Nth(5).Locator("a[href*='/users/']")).ToContainTextAsync(assignee);
    }
}
