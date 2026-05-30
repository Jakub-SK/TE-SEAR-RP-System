using SEAR_DataContract.Models;
using SEAR_WEB.Misc;

namespace SEAR_WEB.Models
{
    public static class ErrorModel
    {
        public static async Task<ShowExceptionMessage> LogException(Exception ex, string appType, string? uuid = null)
        {
            ShowExceptionMessage response = await ApiCaller.CallBackObjectApiAsync<ShowExceptionMessage>("ApiError/LogException", new LogExceptionParameters
            {
                ExceptionMessage = ex.Message,
                ExceptionStackTrace = ex.StackTrace!,
                AppType = appType,
                UUID = uuid ?? Guid.CreateVersion7().ToString()
            });
            return response;
        }
        public static async void UpdateLogExceptionWithSteps(string uuid, string stepsToReproduce)
        {
            ApiCaller.CallBackApiAsync("ApiError/SubmitExceptionSteps", new SubmitExceptionStepsParameters
            {
                UUID = uuid,
                StepsToReproduce = stepsToReproduce
            });
        }
    }
}