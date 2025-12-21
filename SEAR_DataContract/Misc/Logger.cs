using System.Diagnostics;

namespace SEAR_DataContract.Misc
{
    public class Logger
    {
        private readonly string environment;

        public Logger()
        {
            environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
        }

        public void LogInformation(string message)
        {
            Write("INFO", message);
        }

        public void LogWarning(string message)
        {
            Write("WARN", message);
        }

        public void LogError(string message)
        {
            Write("ERROR", message);
        }

        private void Write(string level, string message)
        {
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var logLine = $"[{level}] [{environment}] [{timestamp}] {message}";

            Console.WriteLine(logLine);
        }
    }
}