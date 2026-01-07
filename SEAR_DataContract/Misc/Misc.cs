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
        public static string LogException(Exception ex)
        {
            return DBHelper.ExecuteLogException(ex);
        }
        public static int UpdateLogExceptionWithSteps(string uuid, string steps)
        {
            return DBHelper.ExecuteUpdateLogExceptionWithSteps(uuid, steps);
        }
    }
}
