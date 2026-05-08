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
}