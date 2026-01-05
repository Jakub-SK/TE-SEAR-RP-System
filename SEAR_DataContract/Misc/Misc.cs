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
        public static void LogExceptionToDatabase(Exception ex)
        {
            DBHelper.ExecuteDatabaseNonQuery(
                $"INSERT INTO Log_Exception" +
                $""
            );
        }
    }
}
