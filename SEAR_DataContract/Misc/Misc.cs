namespace SEAR_DataContract.Misc
{
    public static class Misc
    {
        public static bool CheckIsDevelopmentEnviroment()
        {
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                return true;
            return false;
        }
        public static ShowExceptionMessage LogException(Exception ex, string appType, string ? uuid = null)
        {
            return DBHelper.LogException(ex, GetExceptionType(ex), appType, uuid);
        }
        public static int UpdateLogExceptionWithSteps(string uuid, string steps)
        {
            return DBHelper.UpdateLogExceptionWithSteps(uuid, steps);
        }
        public static string GetWebsiteUrl()
        {
            if (CheckIsDevelopmentEnviroment())
                return "https://localhost:5002";
            return "https://tesear.noobxryan.org";
        }
        private static string GetExceptionType(Exception ex)
        {
            //API 404 Not Found
            if (ex.Message.Contains("Response: 404"))
                return "API-404";
            //Internal API Server Error
            if (ex.Message.Contains("Response: 500"))
                return "API-500";
            //Unable to connect to Database
            if (ex.Message.Contains("Failed to connect to"))
                return "DB-001";
            //SQL column reference is ambiguous
            if (ex.Message.Contains("42702"))
                return "DB-42702";
            //SQL column does not exist
            if (ex.Message.Contains("42703"))
                return "DB-42703";
            
            return "Unknown";
        }
    }
}