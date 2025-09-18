using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace PlaywrightTests
{
    public class PanoptoLogin
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
        public async Task LoginTest()
        {
            // Otvori login stranicu
            await _page.GotoAsync(Url);

            // Popuni username i password
            await _page.FillAsync("#usernameInput", Username);
            await _page.FillAsync("#passwordInput", Password);

            // Klikni na Sign in dugme
            await _page.ClickAsync("#PageContentPlaceholder_loginControl_LoginButton");

            // Sačekaj da se stranica učita (može i WaitForNavigation)
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            // Provjeri da li je URL sada home page
            Assert.AreEqual(Url, _page.Url, "Login failed or redirected to wrong page");
        }

        [TearDown]
        public async Task Teardown()
        {
            await _browser.CloseAsync();
            _playwright.Dispose();
        }
    }
}
