using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Localization;
using SEAR_WEB.Session;

namespace SEAR_WEB.Controllers
{
    public class HomeController : Controller
    {
        private readonly SessionCache _sessionCache;
        public HomeController(SessionCache sessionCache)
        {
            _sessionCache = sessionCache;
        }
        public async Task<IActionResult> Index()
        {
            return View();
        }
        public async Task<IActionResult> Privacy()
        {
            return View();
        }
        public async Task<IActionResult> SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddDays(1) }
            );
            return LocalRedirect(returnUrl);
        }
    }
}