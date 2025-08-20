using System;
using System.Threading.Tasks;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Global360.SnipeIT.Playwright.Pages;

public class AssetCreatePage
{
    private readonly IPage _page;
    public AssetCreatePage(IPage page) => _page = page;

    // --- MODEL: "Laptops - Macbook Pro 13\"" ---
    public async Task SelectModelByUniqueIdAsync()
    {
        // Click the "Model" dropdown
        await _page.Locator("#select2-model_select_id-container").ClickAsync();

        // Wait for dropdown options to load
        var options = _page.Locator("li.select2-results__option");

        // Filter by unique number inside text
        var targetOption = options.Filter(new() { HasText = "Macbook Pro" });

        // Wait until visible and clickable
        await targetOption.First.WaitForAsync();

        // Click the option
        await targetOption.First.ClickAsync();

        // ✅ Verify it got rendered in the UI
        var selectedValue = _page.Locator("#select2-model_select_id-container");
        await Expect(selectedValue).ToContainTextAsync("Macbook Pro");
    }






    // --- STATUS: "Ready to Deploy" ---
    public async Task SelectStatusAsync()
    {
        await _page.ClickAsync("#select2-status_select_id-container");

        const string status = "Ready to Deploy";
        await _page.FillAsync("input.select2-search__field", status);

        var option = _page
            .Locator("ul.select2-results__options li.select2-results__option[role='option']:not(.loading-results)")
            .Filter(new() { HasText = status });

        await option.First.WaitForAsync();
        await option.First.ClickAsync();
    }




    // --- USERS: pick any random visible option (appears only after Model & Status are set) ---
    public async Task SelectRandomUserAsync()
    {
        // Becomes available only after Model & Status are set
        var usersContainer = _page.Locator("#select2-assigned_user_select-container");
        await usersContainer.WaitForAsync();
        await usersContainer.ClickAsync();

        // Trigger results and choose a random visible option
        await _page.FillAsync("input.select2-search__field", "a");
        var options = _page.Locator("ul.select2-results__options li.select2-results__option[role='option']:not(.loading-results)");
        await options.First.WaitForAsync();

        var count = await options.CountAsync();
        var pick = new Random().Next(0, Math.Min(count, 6));
        await options.Nth(pick).ClickAsync();
    }


    // --- SAVE (top-right button) ---
    public async Task SaveAsync()
    {
        await _page.Locator("button.btn.btn-primary.pull-right[name='submit']").First.ClickAsync();
    }
    //public async Task ClickSuccessLinkAsync()
    //{
    //    // Wait for success banner
    //    await _page.Locator("#success-notification").WaitForAsync();

    //    // Click on "Click here to view"
    //    await _page.Locator("#success-notification a", new() { HasTextString = "Click here to view" }).ClickAsync();

    //    // Optional validation: ensure asset details page is opened
    //    await Expect(_page.Locator("h1, h2")).ToContainTextAsync("Macbook Pro");
    //}
    public async Task ClickSuccessLinkAsync()
    {
        var banner = _page.Locator("div.alert-success");
        await Assertions.Expect(banner).ToBeVisibleAsync();

        var view = banner.GetByRole(AriaRole.Link, new() { Name = "Click here to view" });
        await Assertions.Expect(view).ToBeVisibleAsync();
        await view.ClickAsync();

        await _page.WaitForURLAsync("**/hardware/*");
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }
}
