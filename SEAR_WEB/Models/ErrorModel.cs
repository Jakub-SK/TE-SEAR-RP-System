using SEAR_DataContract.Models;
using SEAR_WEB.Misc;

namespace SEAR_WEB.Models
{
    public static class ErrorModel
    {
        public static async Task<ShowExceptionMessage> LogException(Exception ex, string appType, string? uuid = null)
        {
            ShowExceptionMessage response = await ApiCaller.CallBackObjectApiAsync<ShowExceptionMessage>("Api/Error/LogException", new LogExceptionParameters
            {
                Exception = ex,
                AppType = appType,
                UUID = uuid ?? Guid.CreateVersion7().ToString()
            });
            return response;
        }
        public static async void UpdateLogExceptionWithSteps(string uuid, string stepsToReproduce)
        {
            await ApiCaller.CallBackObjectApiAsync<SubmitExceptionStepsParameters>("Api/Error/SubmitExceptionSteps", new SubmitExceptionStepsParameters
            {
                UUID = uuid,
                StepsToReproduce = stepsToReproduce
            });
        }
    }
}