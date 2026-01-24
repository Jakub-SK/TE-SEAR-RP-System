using Microsoft.AspNetCore.Mvc;
using SEAR_WEB.Session;

namespace SEAR_WEB.Controllers
{
    public class Manuals : Controller
    {
        private readonly SessionCache _sessionCache;
        public Manuals(SessionCache sessionCache)
        {
            _sessionCache = sessionCache;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
