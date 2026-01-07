using Microsoft.AspNetCore.Http;

namespace SEAR_WEB.Session
{
    public class SessionCache
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        public SessionCache(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }
        public void SetSession(string cacheName, string cacheString)
        {
            httpContextAccessor.HttpContext.Session.SetString(cacheName, cacheString);
        }
        public string GetSession(string cacheName)
        {
            return httpContextAccessor.HttpContext.Session.GetString(cacheName);
        }
    }
}