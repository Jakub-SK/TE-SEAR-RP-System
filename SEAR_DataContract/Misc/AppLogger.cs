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