using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using SEAR_DataContract.Misc;

namespace SEAR_API.Controllers
{
    [ApiController]
    [Route("api/ApiHome")]
    public class ApiHomeController : ControllerBase
    {
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public void Error()
        {
            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            var exception = exceptionHandlerPathFeature?.Error;
            var uuid = Activity.Current?.Id;
            Misc.LogException(exception!, "SEAR API", uuid);
        }
    }
}