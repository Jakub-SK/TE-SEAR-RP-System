using SEAR_DataContract.Models;
using SEAR_WEB.Misc;

namespace SEAR_WEB.Models
{
    public class FileModel
    {
        public static async Task<bool> SaveFileToDatabase(string fileName, string contentType, byte[] fileBytes)
        {
            ReturnSaveFileToDatabase response = await ApiCaller.CallApiAsync<ReturnSaveFileToDatabase>("Api/ApiFile/SaveFileToDatabase", new SaveFileToDatabaseParameters
            {
                FileName = fileName,
                ContentType = contentType,
                FileBytes = fileBytes
            });
            return response.IsSuccessful;
        }
        public static async Task<ReturnDownloadFile> GetFileFromDb(int id)
        {
            return await ApiCaller.CallApiAsync<ReturnDownloadFile>("Api/ApiFile/DownloadFile", new DownloadFileParameters
            {
                FileId = id
            });
        }
    }
}