using Microsoft.AspNetCore.Mvc;
using SEAR_API.Services.Interface;

namespace SEAR_API.Controllers.Base
{
    [ApiController]
    public abstract class ApiControllerBase<TService, TResult> : ControllerBase
        where TService : IAppService<TResult>
    {
        protected readonly TService Service;

        protected ApiControllerBase(TService service)
        {
            Service = service;
        }

        [HttpGet("Get")]
        public virtual IActionResult Get()
        {
            return Ok(Service.Get());
        }
    }
}
