using SEAR_API.Services.Base;

namespace SEAR_API.Services
{
    public class ProjectService : AppServiceBase<string>
    {
        public override string Get()
        {
            return "Reusable project data";
        }
    }
}