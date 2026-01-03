using SEAR_DataContract;
using SEAR_WEB.ApiServer;

namespace SEAR_WEB.Models
{
    public static class HomeModel
    {
        public static HomeDto GetTEName()
        {
            return ApiCaller.CallApi<HomeDto>("api/ApiHome/GetTEName");
        }

        //Call Api with correct parameters
        //how to use
        //public (Return class type) MethodName(Parameter Model)
        //ApiCaller apiCaller = new ApiCaller();
        //return apiCaller.CallApi<Return class type>("api url", Parameter Model);
        public static List<NeedJSON> GetNeedJsonList(RequestGetWithJSONList RequestGetWithJSONList)
        {
            return ApiCaller.CallApi<List<NeedJSON>>("api/ApiHome/GetWithJSONList", RequestGetWithJSONList);
        }
        public static List<DatabaseUsers> GetDatabaseUsersList()
        {
            return ApiCaller.CallApi<List<DatabaseUsers>>("api/ApiHome/GetDatabaseUsersList");
        }
    }
}