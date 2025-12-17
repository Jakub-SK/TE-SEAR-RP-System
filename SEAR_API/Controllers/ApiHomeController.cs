using SEAR_API.Controllers.Base;
using SEAR_API.Services;

namespace SEAR_API.Controllers
{
    public class ApiProjectController
        : ApiControllerBase<ProjectService, string>
    {
        public ApiProjectController(ProjectService service)
            : base(service) { }
    }
}
