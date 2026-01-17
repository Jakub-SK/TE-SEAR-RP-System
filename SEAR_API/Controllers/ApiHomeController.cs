using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SEAR_API.Models;
using SEAR_DataContract;
using SEAR_DataContract.Misc;
using System.Diagnostics;

namespace SEAR_API.Controllers
{
    [ApiController]
    [Route("api/ApiHome")]
    public class ApiHomeController : ControllerBase
    {
        [HttpPost("GetTEName")]
        public HomeDto GetProject()
        {
            HomeDto homeDto = new HomeDto();
            homeDto.Name = "Truth Enforcers";
            homeDto.Id = 29;

            return homeDto;
        }
        [HttpPost("GetWithJSONList")]
        public List<NeedJSON> GetWithJSONList([FromBody] RequestGetWithJSONList request)
        {
            List<NeedJSON> jsonListResponse = new List<NeedJSON>();
            NeedJSON jsonResponse = new NeedJSON();

            jsonResponse.Name = request.Name;
            jsonResponse.MeowId = request.Id;
            jsonListResponse.Add(jsonResponse);

            return jsonListResponse;
        }
        [HttpPost("GetDatabaseUsersList")]
        public List<DatabaseUsers> GetDatabaseUsersList()
        {
            List<DatabaseUsers> databaseUsersList = new List<DatabaseUsers>();
            databaseUsersList = HomeModel.GetDatabaseUsersList();
            return databaseUsersList;
        }
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