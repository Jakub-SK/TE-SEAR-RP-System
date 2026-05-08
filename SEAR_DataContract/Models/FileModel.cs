namespace SEAR_DataContract.Models
{
    public class SaveFileToDatabaseParameters
    {
        public required string FileName { get; set; }
        public required string ContentType { get; set; }
        public required byte[] FileBytes { get; set; }
    }
    public class ReturnSaveFileToDatabase
    {
        public bool IsSuccessful { get; set; }
    }
    public class DownloadFileParameters
    {
        public required int FileId {  get; set; }
    }
    public class ReturnDownloadFile
    {
        public required int Id { get; set; }
        public required string FileName { get; set; }
        public required string ContentType { get; set; }
        public required byte[] FileBytes { get; set; } = Array.Empty<byte>();
    }
}