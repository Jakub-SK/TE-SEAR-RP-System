using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SEAR_WEB.Misc;

namespace SEAR_WEB.Controllers
{
    public class InformationController : Controller
    {
        private readonly SessionCache sessionCache;

        public InformationController(SessionCache sessionCache)
        {
            this.sessionCache = sessionCache;
        }

        public IActionResult Index()
        {
            string ChosenValue = "";
            if (!String.IsNullOrEmpty(sessionCache.GetSession("Choosed")))
            {
                ChosenValue = sessionCache.GetSession("Choosed");
            }

            List<SelectListItem> InsertSelectable = new List<SelectListItem>();
            InsertSelectable.Add(new SelectListItem { Text = "Meow1", Value = "0" });
            InsertSelectable.Add(new SelectListItem { Text = "Meow2", Value = "1", Selected = (ChosenValue == "1") });
            InsertSelectable.Add(new SelectListItem { Text = "Meow3", Value = "2", Selected = (ChosenValue == "2") });
            ViewData["SusDropDownList"] = InsertSelectable;
            return View();
        }

        [HttpPost]
        public IActionResult GetForm(IFormCollection formValues)
        {
            string value = formValues["SusDropDownList"];
            sessionCache.SetSession("Choosed", value);

            return RedirectToAction("Index", "Information");
        }
    }
}
