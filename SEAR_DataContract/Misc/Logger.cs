using System.Diagnostics;

namespace SEAR_DataContract.Misc
{
    public static class Logger
    {
        //public Logger()
        //{
        //    environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
        //}

        public static void LogInformation(string message)
        {
            Write("INFO", message);
        }

        public static void LogWarning(string message)
        {
            Write("WARN", message);
        }

        public static void LogError(string message)
        {
            Write("ERROR", message);
        }

        private static void Write(string level, string message)
        {
            string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var logLine = $"[{level}] [{environment}] [{timestamp}] {message}";

            Console.WriteLine(logLine);
        }
    }
}