namespace SEAR_WEB.RedirectViewModels
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
        public string? UUID { get; set; }
        public string? ErrorType { get; set; }
        public string? ErrorSteps { get; set; }
    }
}