namespace SEAR_WEB.RedirectViewModels
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
        public string? UUID { get; set; }
        public bool ShowUUID => !string.IsNullOrEmpty(UUID);
        public string? ExceptionType { get; set; }
        public string? ErrorSteps { get; set; }
        public string? StackTrace { get; set; }
    }
}