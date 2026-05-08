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
            return await DBHelper.LogException(ex, GetExceptionType(ex), appType, uuid);
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
        private static ExceptionTypeModel GetExceptionType(Exception ex)
        {
            ExceptionTypeModel model = new ExceptionTypeModel();
            //API 404 Not Found
            if (ex.Message.Contains("Response: 404"))
                model.ExceptionType = "API-404";
            //Internal API Server Error
            if (ex.Message.Contains("Response: 500"))
                model.IsApi500 = true;
            //Unable to connect to Database
            if (ex.Message.Contains("Unable to establish connection to database"))
                model.ExceptionType = "DB-001";
            //Permission Denied when executing SQL
            if (ex.Message.Contains("42501"))
                model.ExceptionType = "DB-42501";
            //SQL column reference is ambiguous
            if (ex.Message.Contains("42702"))
                model.ExceptionType = "DB-42702";
            //SQL column does not exist
            if (ex.Message.Contains("42703"))
                model.ExceptionType = "DB-42703";

            return model;
        }
    }
}