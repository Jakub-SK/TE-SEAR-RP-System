using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using System.Diagnostics;
using SEAR_DataContract.Misc;
using SEAR_WEB.RedirectViewModels;
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
        public async Task<IActionResult> Privacy()
        {
            return View();
        }
        public async Task<IActionResult> SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );
            return LocalRedirect(returnUrl);
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Error()
        {
            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            var exception = exceptionHandlerPathFeature?.Error;
            var uuid = Activity.Current?.Id;
            ShowExceptionMessage display = new ShowExceptionMessage();
            if (exception != null)
            {
                display = await SEAR_DataContract.Misc.Misc.LogException(exception, "SEAR WEB", uuid);
                return View(new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                    UUID = display.UUID,
                    ErrorType = display.ErrorType
                });
            }
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
            });
        }
        public async Task<IActionResult> SubmitExceptionSteps(ErrorViewModel model)
        {
            SEAR_DataContract.Misc.Misc.UpdateLogExceptionWithSteps(model.UUID!, model.ErrorSteps!);
            return RedirectToAction("Index", "Home");
        }
    }
}