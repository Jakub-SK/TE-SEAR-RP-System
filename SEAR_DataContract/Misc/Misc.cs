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
            return DBHelper.ExecuteToLogException(ex);
        }
        public static int UpdateLogExceptionWithSteps(string uuid, string steps)
        {
            string sql = @"
                UPDATE log_exception
                SET steps = @Steps
                WHERE uuid = @UUID
            ;";

            List<NpgsqlParameter> parameters = new List<NpgsqlParameter>();
            parameters.Add(new NpgsqlParameter("Steps", steps));
            parameters.Add(new NpgsqlParameter("UUID", uuid));

            return DBHelper.ExecuteDatabaseNonQuery(sql, parameters);
        }
    }
}
