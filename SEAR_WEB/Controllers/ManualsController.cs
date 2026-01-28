using Microsoft.AspNetCore.Mvc;
using SEAR_WEB.Session;

namespace SEAR_WEB.Controllers
{
    public class ManualsController : Controller
    {
        private readonly SessionCache _sessionCache;
        public ManualsController(SessionCache sessionCache)
        {
            _sessionCache = sessionCache;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
