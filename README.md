
# Global360 QA Assessment — .NET 8 + Playwright

This repo automates the Snipe-IT demo flow:
1) Login  
2) Create a **MacBook Pro 13"** asset with **Ready to Deploy** status  
3) **Check out** the asset to a random user  
4) Find & open the asset from the **Assets** list to verify it exists  
5) Validate key details on the asset page  
6) Validate entries in the **History** tab

> Demo URL: https://demo.snipeitapp.com/login (Use `admin` / `password`)  
> (Official demo credentials listed by Snipe‑IT.)

## Tech & Versions
- **.NET**: 8.0 (LTS)
- **Playwright for .NET**: Microsoft.Playwright (1.49+)
- **Test framework**: NUnit

## Quick Start
```bash
# 1) Prereqs: .NET 8 SDK + PowerShell Core (pwsh) + Git
dotnet --version

# 2) Restore & build
dotnet restore
dotnet build -c Release

# 3) Install Playwright browsers
pwsh ./bin/Release/net8.0/playwright.ps1 install

# 4) (optional) Run headful for demo
$env:HEADLESS="false"  # on Windows PowerShell
# export HEADLESS=false   # on bash/zsh

# 5) Run the test
dotnet test -c Release
```

## Configuration
The test reads **environment variables** (with sensible defaults):
- `BASE_URL` (default: `https://demo.snipeitapp.com`)
- `SNIPE_USERNAME` (default: `admin`)
- `SNIPE_PASSWORD` (default: `password`)
- `HEADLESS` (`true` by default; set to `false` for visible browser)

Example:
```bash
# Windows PowerShell
$env:BASE_URL="https://demo.snipeitapp.com"
$env:SNIPE_USERNAME="admin"
$env:SNIPE_PASSWORD="password"
$env:HEADLESS="true"

dotnet test
```

## What the Test Does (High Level)
- Logs in using credentials on the login page.
- Navigates to **Assets** and clicks **Create New**.
- Fills **Asset Tag** with a unique value, selects **Model** "MacBook Pro 13", selects **Status** "Ready to Deploy".
- Creates the asset, then **Checks Out** to a random existing user (selected from the user picker’s dropdown).
- Navigates back to **Assets** list, searches by the asset tag, opens the asset details.
- Validates that **Model**, **Status**, and **Assigned To** match expectations.
- Goes to the **History** tab and validates that **Created** and **Checked Out** entries are present.

> ⚠️ Notes:
> - The public demo DB is shared and changes constantly. The test uses robust selectors (labels/roles) and defensive logic, but if a label changes, minor tweaks may be needed.
> - The **Model** field in Snipe‑IT uses a Select2 widget; the code searches for “MacBook Pro 13” and falls back to the first **MacBook** model if an exact match isn’t present.

## Repo Layout
```
.
├─ Pages/
│  ├─ LoginPage.cs
│  ├─ AssetsListPage.cs
│  ├─ AssetCreatePage.cs
│  └─ AssetDetailsPage.cs
├─ Support/
│  ├─ TestBase.cs
│  └─ TestSettings.cs
├─ SnipeITFlowTests.cs
├─ Global360.SnipeIT.Playwright.csproj
├─ .gitignore
└─ README.md
```

## Running in CI (optional)
A sample GitHub Actions workflow is included at `.github/workflows/dotnet.yml`.  
It restores, builds, installs Playwright browsers, and runs tests in headless mode.

## How to Submit
1. Push this project to your GitHub (or GitLab/Bitbucket) account.
2. Share the repository link via email.
3. If Global360 needs any special instructions, keep them in this README.
