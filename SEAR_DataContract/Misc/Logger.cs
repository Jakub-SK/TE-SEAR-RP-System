//namespace SEAR_DataContract.Misc
//{
//    public static class Logger
//    {
//        public static void LogInformation(string message)
//        {
//            Write("INFO", message);
//        }
//        public static void LogWarning(string message)
//        {
//            Write("WARN", message);
//        }
//        public static void LogError(string message)
//        {
//            Write("ERROR", message);
//        }
//        private static void Write(string level, string message)
//        {
//            string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
//            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
//            var logLine = $"[{level}] [{environment}] [{timestamp}] {message}";

//            Console.WriteLine(logLine);
//        }
//    }
//}
using Microsoft.Extensions.Logging;

namespace SEAR_DataContract.Misc
{
    public static class AppLogger
    {
        private static ILogger? _logger;
        public static void Initialize(ILogger logger)
        {
            _logger = logger;
        }
        public static void LogInformation(string message)
        {
            _logger?.LogInformation(message);
        }
        public static void LogWarning(string message)
        {
            _logger?.LogWarning(message);
        }
        public static void LogError(string message)
        {
            _logger?.LogError(message);
        }
    }
}