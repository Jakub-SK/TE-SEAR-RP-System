namespace SEAR_DataContract.Models
{
    public class ShowExceptionMessage
    {
        public ShowExceptionMessage()
        {
            UUID = "Unable to get UUID";
            ExceptionType = "Unknown";
        }
        public string UUID { get; set; }
        public string ExceptionType { get; set; }
    }
}