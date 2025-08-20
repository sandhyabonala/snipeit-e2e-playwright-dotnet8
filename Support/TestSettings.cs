using System;

namespace Global360.SnipeIT.Playwright.Support;

public class TestSettings
{
    public string BaseUrl { get; } =
        Environment.GetEnvironmentVariable("BASE_URL")?.TrimEnd('/') ?? "https://demo.snipeitapp.com";

    public string Username { get; } =
        Environment.GetEnvironmentVariable("SNIPE_USERNAME") ?? "admin";

    public string Password { get; } =
        Environment.GetEnvironmentVariable("SNIPE_PASSWORD") ?? "password";

    public bool Headless =>
        !string.Equals(Environment.GetEnvironmentVariable("HEADLESS"), "false", StringComparison.OrdinalIgnoreCase);

    public TimeSpan Timeout => TimeSpan.FromSeconds(45);
}
