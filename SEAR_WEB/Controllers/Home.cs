using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using SEAR_DataContract.Misc;
using SEAR_WEB.RedirectViewModels;
using SEAR_WEB.Session;

namespace SEAR_WEB.Controllers
{
    public class Home : Controller
    {
        private readonly SessionCache _sessionCache;
        public Home(SessionCache sessionCache)
        {
            _sessionCache = sessionCache;
        }
        //Call API Method and return model to Index.cshtml
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            var exception = exceptionHandlerPathFeature?.Error;
            var uuid = Activity.Current?.Id;
            ShowExceptionMessage display = new ShowExceptionMessage();
            if (exception != null)
            {
                display = SEAR_DataContract.Misc.Misc.LogException(exception, "SEAR WEB", uuid);
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
        public IActionResult SubmitExceptionSteps(ErrorViewModel model)
        {
            SEAR_DataContract.Misc.Misc.UpdateLogExceptionWithSteps(model.UUID!, model.ErrorSteps!);
            return RedirectToAction("Index", "Home");
        }
    }
}