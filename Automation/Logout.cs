using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace PlaywrightTests
{
    public class PanoptoLogout
    {
        private IPlaywright _playwright;
        private IBrowser _browser;
        private IPage _page;

        private const string Url = "https://scratch.staging.panopto.com/Panopto/Pages/Home.aspx";
        private const string Username = "doa1";
        private const string Password = "!Test1234";

        [SetUp]
        public async Task Setup()
        {
            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false
            });
            _page = await _browser.NewPageAsync();
        }

        [Test]
        public async Task LoginAndLogoutTest()
        {
            // --- LOGIN ---
            await _page.GotoAsync(Url);
            await _page.FillAsync("#usernameInput", Username);
            await _page.FillAsync("#passwordInput", Password);
            await _page.ClickAsync("#PageContentPlaceholder_loginControl_LoginButton");

            // Sačekaj da se učita home page
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            Assert.AreEqual(Url, _page.Url, "Login failed or redirected to wrong page");

            // --- LOGOUT ---
            // Klik na avatar button da otvori dropdown meni
            await _page.ClickAsync("button.MuiButtonBase-root");

            // Sačekaj da logout dugme postane vidljivo
            await _page.WaitForSelectorAsync("#UserMenuContent_Logout", new PageWaitForSelectorOptions { State = WaitForSelectorState.Visible });

            // Klikni na logout dugme
            await _page.Locator("#UserMenuContent_Logout").ClickAsync(new LocatorClickOptions { Force = true });

            // Sačekaj da se stranica učita nakon logout-a
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            // Provjera da je user logoutan
            var signInHeader = await _page.TextContentAsync("h1.page-only");
            Assert.AreEqual("Sign in to Panopto", signInHeader, "Logout failed or wrong page");
        }

        [TearDown]
        public async Task Teardown()
        {
            await _browser.CloseAsync();
            _playwright.Dispose();
        }
    }
}
