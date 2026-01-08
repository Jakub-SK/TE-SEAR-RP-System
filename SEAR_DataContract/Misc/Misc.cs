using Npgsql;

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
        public static string LogException(Exception ex, string? uuid = null)
        {
            return DBHelper.ExecuteLogException(ex, uuid);
        }
        public static int UpdateLogExceptionWithSteps(string uuid, string steps)
        {
            return DBHelper.ExecuteUpdateLogExceptionWithSteps(uuid, steps);
        }
    }
}
