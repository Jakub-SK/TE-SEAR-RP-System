using SEAR_DataContract.Models;

namespace SEAR_DataContract.Misc
{
    public static class Misc
    {
        public static bool CheckIsDevelopmentEnvironment()
        {
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                return true;
            return false;
        }
        public static async Task<ShowExceptionMessage> LogException(Exception ex, string appType, string? uuid = null)
        {
            return await DBHelper.LogException(ex, GetExceptionType(ex), appType, uuid, ex.StackTrace);
        }
        public static async void UpdateLogExceptionWithSteps(string uuid, string steps)
        {
            DBHelper.UpdateLogExceptionWithSteps(uuid, steps);
        }
        public static string GetDomainUrl()
        {
            if (CheckIsDevelopmentEnvironment())
                return "localhost";
            return "sessvirtus.org";
        }
        public static string GetWebsiteUrl()
        {
            if (CheckIsDevelopmentEnvironment())
                return "https://localhost:5002";
            return "https://sessvirtus.org";
        }
        private static string GetExceptionType(Exception ex)
        {
            return ex.Message switch
            {
                //API 404 Not Found
                "API-404" => "API-404",
                //Internal API Server Error
                "API-500" => "API-500",
                //Unable to connect to Database
                "DB-001" => "DB-001",
                //SQL column reference is ambiguous
                "DB-42702" => "DB-42702",
                //SQL column does not exist
                "DB-42703" => "DB-42703",
                _ => "Unknown"
            };
        }
    }
}