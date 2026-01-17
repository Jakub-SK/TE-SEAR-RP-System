namespace SEAR_DataContract.Misc
{
    public static class Misc
    {
        public static bool CheckIsDevelopmentEnviroment()
        {
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                return true;
            }
            return false;
        }
        public static ShowExceptionMessage LogException(Exception ex, string appType, string ? uuid = null)
        {
            return DBHelper.ExecuteLogException(ex, GetExceptionType(ex), appType, uuid);
        }
        public static int UpdateLogExceptionWithSteps(string uuid, string steps)
        {
            return DBHelper.ExecuteUpdateLogExceptionWithSteps(uuid, steps);
        }
        private static string GetExceptionType(Exception ex)
        {
            //Internal API Server Error
            if (ex.Message.Contains("Response: 500 Internal Server Error"))
            {
                return "API-500";
            }
            //Unable to connect to Database
            if (ex.Message.Contains("Failed to connect to"))
            {
                return "DB-001";
            }
            return "Undefined";
        }
    }
}