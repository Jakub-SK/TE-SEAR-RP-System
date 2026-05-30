using System.Text.Json.Serialization;

namespace SEAR_DataContract.Models
{
    public class ShowExceptionMessage
    {
        public string UUID { get; set; } = "Unable to get UUID";
        public bool IsApi500 { get; set; }
        public string ExceptionType { get; set; } = "Unknown";
    }
    public class ApiErrorModel
    {
        [JsonPropertyName("uuid")]
        public string UUID { get; set; } = "Unable to get UUID";
        [JsonPropertyName("message")]
        public string? Message { get; set; }
        [JsonPropertyName("stackTrace")]
        public string? StackTrace { get; set; }
    }
    public class ExceptionTypeModel
    {
        public bool IsApi500 { get; set; } = false;
        public string ExceptionType { get; set; } = "Unknown";
    }
    public class LogExceptionParameters
    {
        public required string ExceptionMessage { get; set; }
        public required string ExceptionStackTrace { get; set; }
        public string AppType { get; set; } = "Unknown";
        public string? UUID { get; set; }
    }
    public class SubmitExceptionStepsParameters
    {
        public required string UUID { get; set; }
        public required string StepsToReproduce { get; set; }
    }
}