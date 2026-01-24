using Microsoft.AspNetCore.Mvc;
using SEAR_WEB.Session;

namespace SEAR_WEB.Controllers
{
    public class Enemies : Controller
    {
        private readonly SessionCache _sessionCache;
        public Enemies(SessionCache sessionCache)
        {
            _sessionCache = sessionCache;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
