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
        public static ExceptionTypeModel GetExceptionType(string exceptionMessage)
        {
            ExceptionTypeModel model = new ExceptionTypeModel();
            //API 404 Not Found
            if (exceptionMessage.Contains("Response: 404"))
                model.ExceptionType = "API-404";
            //Internal API Server Error
            if (exceptionMessage.Contains("Response: 500"))
                model.IsApi500 = true;
            //Unable to connect to Database
            if (exceptionMessage.Contains("Unable to establish connection to database"))
                model.ExceptionType = "DB-001";
            //Permission Denied when executing SQL
            if (exceptionMessage.Contains("42501"))
                model.ExceptionType = "DB-42501";
            //SQL column reference is ambiguous
            if (exceptionMessage.Contains("42702"))
                model.ExceptionType = "DB-42702";
            //SQL column does not exist
            if (exceptionMessage.Contains("42703"))
                model.ExceptionType = "DB-42703";

            return model;
        }
    }
}