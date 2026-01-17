namespace SEAR_WEB.Session
{
    public class SessionCache
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public SessionCache(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public void SetSession(string cacheName, string cacheString)
        {
            _httpContextAccessor.HttpContext!.Session.SetString(cacheName, cacheString);
        }
        public string GetSession(string cacheName)
        {
            return _httpContextAccessor.HttpContext!.Session.GetString(cacheName)!;
        }
    }
}