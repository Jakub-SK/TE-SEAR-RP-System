using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SEAR_WEB.RedirectViewModels;
using SEAR_WEB.AppServer;
using SEAR_DataContract;

namespace SEAR_WEB.Controllers
{
    public class HomeController : Controller
    {
        private readonly HomeApi homeApi;
        public HomeController(HomeApi homeApi)
        {
            this.homeApi = homeApi;
        }

        //Call API Method and return model to Index.cshtml
        public async Task<IActionResult> Index()
        {
            HomeDto TEName = await homeApi.GetTEName();
            ViewData["ProjectName"] = TEName.Name;
            ViewData["Id"] = TEName.Id;

            //create parameter model
            RequestGetWithJSONList requestParameter = new RequestGetWithJSONList();
            requestParameter.Name = "SomethingName";
            requestParameter.Id = 123;

            //create new model
            JsonList model = new JsonList();
            model.JSONList = await homeApi.GetNeedJsonList(requestParameter);
            return View(model);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
