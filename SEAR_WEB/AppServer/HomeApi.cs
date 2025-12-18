using SEAR_DataContract;

namespace SEAR_WEB.AppServer
{
    public class HomeApi
    {
        private readonly ApiCaller ApiCaller;
        public HomeApi(ApiCaller ApiCaller)
        {
            this.ApiCaller = ApiCaller;
        }

        public Task<HomeDto> GetTEName()
        {
            return ApiCaller.CallApi<HomeDto>("api/ApiHome/GetTEName", null);
        }

        //Call Api with correct parameters
        //how to use
        //public Task<Return class type> MethodName(Parameter Model)
        //return ApiCaller.CallApi<Return class type>("api url", Parameter Model);
        public Task<List<NeedJSON>> GetNeedJsonList(RequestGetWithJSONList RequestGetWithJSONList)
        {
            return ApiCaller.CallApi<List<NeedJSON>>("api/ApiHome/GetWithJSONList", RequestGetWithJSONList);
        }
    }
}