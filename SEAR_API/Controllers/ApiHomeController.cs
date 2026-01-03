using Microsoft.AspNetCore.Mvc;
using SEAR_API.Models;
using SEAR_DataContract;

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
    }
}