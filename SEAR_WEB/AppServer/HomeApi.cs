using SEAR_DataContract;

namespace SEAR_WEB.AppServer
{
    public class HomeApi
    {
        public HomeDto GetTEName()
        {
            ApiCaller apiCaller = new ApiCaller();
            return apiCaller.CallApi<HomeDto>("api/ApiHome/GetTEName", null);
        }

        //Call Api with correct parameters
        //how to use
        //public (Return class type) MethodName(Parameter Model)
        //ApiCaller apiCaller = new ApiCaller();
        //return apiCaller.CallApi<Return class type>("api url", Parameter Model);
        public List<NeedJSON> GetNeedJsonList(RequestGetWithJSONList RequestGetWithJSONList)
        {
            ApiCaller apiCaller = new ApiCaller();
            return apiCaller.CallApi<List<NeedJSON>>("api/ApiHome/GetWithJSONList", RequestGetWithJSONList);
        }
    }
}