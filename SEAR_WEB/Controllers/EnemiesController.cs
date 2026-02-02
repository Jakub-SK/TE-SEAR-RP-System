using Microsoft.AspNetCore.Mvc;
using SEAR_WEB.Session;

namespace SEAR_WEB.Controllers
{
    public class EnemiesController : Controller
    {
        private readonly SessionCache _sessionCache;
        public EnemiesController(SessionCache sessionCache)
        {
            _sessionCache = sessionCache;
        }
        public async Task<IActionResult> Index()
        {
            return View();
        }
    }
}
