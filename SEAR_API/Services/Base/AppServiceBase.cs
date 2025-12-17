using SEAR_API.Services.Interface;

namespace SEAR_API.Services.Base
{
    public abstract class AppServiceBase<T> : IAppService<T>
    {
        public abstract T Get();
    }
}