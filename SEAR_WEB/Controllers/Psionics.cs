using Microsoft.AspNetCore.Mvc;
using SEAR_WEB.Session;

namespace SEAR_WEB.Controllers
{
    public class Psionics : Controller
    {
        private readonly SessionCache _sessionCache;
        public Psionics(SessionCache sessionCache)
        {
            _sessionCache = sessionCache;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
