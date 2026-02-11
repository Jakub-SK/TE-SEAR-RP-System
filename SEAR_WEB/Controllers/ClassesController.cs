using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEAR_WEB.Session;

namespace SEAR_WEB.Controllers
{
    public class ClassesController : Controller
    {
        private readonly SessionCache _sessionCache;
        public ClassesController(SessionCache sessionCache)
        {
            _sessionCache = sessionCache;
        }
    }
}