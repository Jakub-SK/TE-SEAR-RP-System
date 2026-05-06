using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SEAR_DataContract.Models;
using SEAR_WEB.RedirectViewModels;
using SEAR_WEB.Session;
using System.Diagnostics;

namespace SEAR_WEB.Controllers
{
    public class ErrorController : Controller
    {
        private readonly SessionCache _sessionCache;
        public ErrorController(SessionCache sessionCache)
        {
            _sessionCache = sessionCache;
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> ErrorException()
        {
            var exception = HttpContext.Features.Get<IExceptionHandlerPathFeature>()?.Error;
            var uuid = Activity.Current?.Id;
            if (exception != null)
            {
                ShowExceptionMessage display = await SEAR_DataContract.Misc.Misc.LogException(exception, "SEAR WEB", uuid);
                return View(new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                    UUID = display.UUID,
                    ExceptionType = display.ExceptionType,
                    StackTrace = exception.StackTrace
                });
            }
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
            });
        }
        [HttpPost]
        public async Task<IActionResult> SubmitExceptionSteps(ErrorViewModel model)
        {
            SEAR_DataContract.Misc.Misc.UpdateLogExceptionWithSteps(model.UUID!, model.ErrorSteps!);
            return RedirectToAction("Index", "Home");
        }
        public IActionResult Error404()
        {
            return View();
        }
    }
}