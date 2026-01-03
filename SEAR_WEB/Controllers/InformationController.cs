using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SEAR_WEB.Misc;
using SEAR_DataContract;

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

            TextBoxForModel model = new TextBoxForModel();
            if (!String.IsNullOrEmpty(sessionCache.GetSession("TextBoxFor")))
            {
                string TextBoxFor = sessionCache.GetSession("TextBoxFor");
                model.TextBoxFor = TextBoxFor;
            }

            List<SelectListItem> InsertSelectable = new List<SelectListItem>();
            InsertSelectable.Add(new SelectListItem { Text = "Meow1", Value = "0" });
            InsertSelectable.Add(new SelectListItem { Text = "Meow2", Value = "1", Selected = (ChosenValue == "1") });
            InsertSelectable.Add(new SelectListItem { Text = "Meow3", Value = "2", Selected = (ChosenValue == "2") });
            ViewData["SusDropDownList"] = InsertSelectable;
            return View(model);
        }

        [HttpPost]
        public IActionResult GetForm(IFormCollection formValues, TextBoxForModel model)
        {
            string value = formValues["SusDropDownList"];
            sessionCache.SetSession("Choosed", value);

            if (String.IsNullOrEmpty(model.TextBoxFor))
            {
                model.TextBoxFor = "";
            }
            string value2 = model.TextBoxFor;
            sessionCache.SetSession("TextBoxFor", value2);
            return RedirectToAction("Index", "Information");
        }
    }
}
