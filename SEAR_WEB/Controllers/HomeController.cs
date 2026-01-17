using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SEAR_DataContract;
using SEAR_DataContract.Misc;
using SEAR_WEB.Models;
using SEAR_WEB.RedirectViewModels;
using SEAR_WEB.Session;
using System.Diagnostics;

namespace SEAR_WEB.Controllers
{
    public class HomeController : Controller
    {
        private readonly SessionCache sessionCache;
        public HomeController(SessionCache sessionCache)
        {
            this.sessionCache = sessionCache;
        }
        //Call API Method and return model to Index.cshtml
        public IActionResult Index()
        {
            HomeDto TEName = HomeModel.GetTEName();
            ViewData["ProjectName"] = TEName.Name;
            ViewData["Id"] = TEName.Id;

            //create parameter model
            RequestGetWithJSONList requestParameter = new RequestGetWithJSONList();
            requestParameter.Name = "SomethingName";
            requestParameter.Id = 123456;

            //create new model
            JsonList model = new JsonList();
            model.JSONList = HomeModel.GetNeedJsonList(requestParameter);

            ViewData["DatabaseUsersList"] = HomeModel.GetDatabaseUsersList();

            return View(model);
        }
        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult LogExceptionTest()
        {
            Exception ex = new Exception("TestExceptionJust a message");
            Misc.LogException(ex, "SEAR WEB", null!);
            return RedirectToAction("Index", "Home");
        }
        public IActionResult SubmitExceptionMessage(JsonList model)
        {
            if (Misc.CheckIsDevelopmentEnviroment())
            {
                throw new Exception($"This is for Production Enviroment\n Exception message: {model.ExceptionMessage}");
            }
            throw new Exception(model.ExceptionMessage);
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            var exception = exceptionHandlerPathFeature?.Error;
            var uuid = Activity.Current?.Id;
            ShowExceptionMessage display = new ShowExceptionMessage();
            display = Misc.LogException(exception!, "SEAR WEB", uuid);

            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                UUID = display.UUID,
                ErrorType = display.ErrorType
            });
        }
        public IActionResult SubmitExceptionSteps(ErrorViewModel model)
        {
            Misc.UpdateLogExceptionWithSteps(model.UUID!, model.ErrorSteps!);
            return RedirectToAction("Index", "Home");
        }
    }
}